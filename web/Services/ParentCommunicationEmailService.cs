using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : ParentCommunicationEmailService
// Purpose          : Email only Parents who received an actual Supabase
//                    announcement or event notification.
// Re-use           : Teacher and School Admin create actions.
// Input Parameters : Supabase Client, SMTP configuration and logger.
// Output Type      : EmailDeliveryResult.
//
public sealed class ParentCommunicationEmailService
{
    private readonly Supabase.Client _supabase;
    private readonly IConfiguration _config;
    private readonly ILogger<ParentCommunicationEmailService> _logger;

    public ParentCommunicationEmailService(
        Supabase.Client supabase,
        IConfiguration config,
        ILogger<ParentCommunicationEmailService> logger)
    {
        _supabase = supabase;
        _config = config;
        _logger = logger;
    }

    private sealed class Recipient
    {
        [JsonPropertyName("notification_id")]
        public Guid NotificationId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
    }

    //
    // Method Name       : SendAsync
    // Purpose           : Send email to each newly claimed Parent notification.
    // Re-use            : Announcement and Event creation.
    // Input Parameters  : Item UUID, kind, title, body.
    // Output Type       : Task<EmailDeliveryResult>.
    //
    public async Task<EmailDeliveryResult> SendAsync(
        Guid itemId,
        string kind,
        string title,
        string body)
    {
        if (itemId == Guid.Empty ||
            (kind != "announcement" && kind != "event"))
        {
            throw new ArgumentException("Invalid communication item.");
        }

        // Read SMTP settings before claiming recipients, so a missing
        // Gmail App Password cannot consume the delivery records.
        var server = _config["SmtpSettings:Server"];
        var sender = _config["SmtpSettings:SenderEmail"];
        var password = _config["SmtpSettings:Password"];
        var senderName = _config["SmtpSettings:SenderName"]
                         ?? "School Parental System";
        if (string.IsNullOrWhiteSpace(server) ||
            string.IsNullOrWhiteSpace(sender) ||
            string.IsNullOrWhiteSpace(password) ||
            !int.TryParse(_config["SmtpSettings:Port"], out var port))
        {
            throw new InvalidOperationException(
                "Gmail SMTP settings are missing in User Secrets.");
        }

        // The DB function validates the authenticated sender, checks
        // notification ownership, and returns only active Parent accounts.
        // Each notification is claimed once to avoid duplicate emails.
        var response = await _supabase.Rpc(
            "claim_parent_email_recipients",
            new Dictionary<string, object>
            {
                ["p_item_id"] = itemId,
                ["p_kind"] = kind
            });

        var recipients = JsonSerializer.Deserialize<List<Recipient>>(
            response.Content ?? "[]") ?? new List<Recipient>();

        var result = new EmailDeliveryResult();
        var emailSubject = "School Parental System - " +
                           (title ?? "Notification").Replace("\r", " ")
                              .Replace("\n", " ");
        var emailBody = "Dear Parent/Guardian,\n\n" + body +
                        "\n\nYou can also view this " + kind +
                        " in your School Parental System mobile app.\n\n" +
                        "School Parental System";

        foreach (var recipient in recipients)
        {
            bool sent = false;
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(sender!, senderName);
                message.To.Add(new MailAddress(recipient.Email));
                message.Subject = emailSubject;
                message.Body = emailBody;
                message.IsBodyHtml = false;

                using var smtp = new SmtpClient(server!, port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        sender, password!.Replace(" ", ""))
                };

                await smtp.SendMailAsync(message);
                result.SentCount++;
                sent = true;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                _logger.LogError(ex,
                    "Failed to email Parent notification {NotificationId}",
                    recipient.NotificationId);
            }
            finally
            {
                try
                {
                    await _supabase.Rpc(
                        "complete_parent_email_delivery",
                        new Dictionary<string, object>
                        {
                            ["p_notification_id"] = recipient.NotificationId,
                            ["p_success"] = sent
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unable to record Parent email status for {NotificationId}",
                        recipient.NotificationId);
                }
            }
        }
        return result;
    }
}

// Class Name: EmailDeliveryResult
// Purpose: Provide the number of email sends and failures.
public sealed class EmailDeliveryResult
{
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
}

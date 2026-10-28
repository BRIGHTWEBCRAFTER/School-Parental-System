
using System.Net;
using System.Net.Mail;


namespace SchoolParentalSystem.Web.Services.SuperAdmin;


//
// Class Name       : RegistrationCodeEmailService
// Purpose          : Encapsulates registration code email operations for
//                    the School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class RegistrationCodeEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegistrationCodeEmailService> _logger;


    //
    // Method Name      : RegistrationCodeEmailService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : IConfiguration configuration;
    //                    ILogger<RegistrationCodeEmailService> logger
    // Output Type      : Constructor.
    //

    public RegistrationCodeEmailService(
        IConfiguration configuration,
        ILogger<RegistrationCodeEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    // =====================================================
    // SEND SCHOOL ADMIN REGISTRATION CODE
    // =====================================================

    //
    // Method Name      : SendSchoolAdminRegistrationCodeAsync
    // Purpose          : Sends school Admin Registration Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string schoolEmail; string schoolName; string
    //                    registrationCode; DateTime expiresAt
    // Output Type      : Task.
    //

    public async Task SendSchoolAdminRegistrationCodeAsync(
        string schoolEmail,
        string schoolName,
        string registrationCode,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(schoolEmail))
        {
            throw new InvalidOperationException(
                "The selected school does not have a registered email address.");
        }


        var smtpSettings =
            GetSmtpSettings();


        var localExpiry =
            expiresAt.ToLocalTime();


        using var message =
            new MailMessage();


        message.From =
            new MailAddress(
                smtpSettings.SenderEmail,
                smtpSettings.SenderName);


        message.To.Add(
            new MailAddress(
                schoolEmail));


        message.Subject =
            "School Admin Registration Code";


        message.IsBodyHtml =
            true;


        message.Body =
            $"""
            <!DOCTYPE html>
            <html>
            <body style="
                margin:0;
                padding:0;
                background:#f6f2e7;
                font-family:Arial,Helvetica,sans-serif;
                color:#173c2d;">

                <div style="
                    max-width:650px;
                    margin:30px auto;
                    background:#ffffff;
                    border-radius:14px;
                    overflow:hidden;
                    border:1px solid #e2dfd2;">

                    <div style="
                        background:#005640;
                        color:#ffffff;
                        padding:24px 30px;">

                        <h2 style="
                            margin:0;
                            font-size:22px;">

                            School-Parental System

                        </h2>

                        <p style="
                            margin:7px 0 0;
                            color:#f7c844;">

                            School Admin Registration

                        </p>

                    </div>


                    <div style="
                        padding:30px;">

                        <p>
                            Dear School Administrator,
                        </p>


                        <p>
                            A School Admin registration code has been
                            generated for:
                        </p>


                        <h3 style="
                            color:#005640;
                            margin-bottom:20px;">

                            {WebUtility.HtmlEncode(schoolName)}

                        </h3>


                        <div style="
                            background:#f8faf7;
                            border:2px solid #f4c542;
                            border-radius:10px;
                            padding:22px;
                            text-align:center;
                            margin:24px 0;">

                            <div style="
                                font-size:12px;
                                text-transform:uppercase;
                                letter-spacing:1px;
                                color:#667066;
                                margin-bottom:8px;">

                                Registration Code

                            </div>


                            <div style="
                                font-size:30px;
                                font-weight:700;
                                letter-spacing:4px;
                                color:#005640;">

                                {WebUtility.HtmlEncode(registrationCode)}

                            </div>

                        </div>


                        <p>
                            This code expires on:
                        </p>


                        <p style="
                            font-weight:700;
                            color:#005640;">

                            {localExpiry:dd MMM yyyy HH:mm}

                        </p>


                        <p>
                            Use this code when registering a
                            <strong>School Admin</strong>
                            account for
                            <strong>{WebUtility.HtmlEncode(schoolName)}</strong>.
                        </p>


                        <div style="
                            margin-top:25px;
                            padding:15px;
                            background:#fff7df;
                            border-radius:8px;
                            color:#725818;">

                            Do not share this registration code with
                            unauthorised persons.

                        </div>

                    </div>


                    <div style="
                        background:#f4f2ea;
                        padding:18px 30px;
                        font-size:12px;
                        color:#6c746f;">

                        School-Parental System
                        <br />
                        Department of Basic Education

                    </div>

                </div>

            </body>
            </html>
            """;


        await SendEmailAsync(
            message,
            smtpSettings,
            "School Admin registration email");
    }


    // =====================================================
    // SEND SCHOOL ADMIN LOGIN ACCESS CODE
    // =====================================================

    //
    // Method Name      : SendSchoolAdminLoginAccessCodeAsync
    // Purpose          : Sends school Admin Login Access Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string recipientEmail; string schoolName; string
    //                    adminName; string loginAccessCode
    // Output Type      : Task.
    //

    public async Task SendSchoolAdminLoginAccessCodeAsync(
        string recipientEmail,
        string schoolName,
        string adminName,
        string loginAccessCode)
    {
        if (string.IsNullOrWhiteSpace(
                recipientEmail))
        {
            throw new InvalidOperationException(
                "The School Admin email address is missing.");
        }


        if (string.IsNullOrWhiteSpace(
                loginAccessCode))
        {
            throw new InvalidOperationException(
                "The School Admin login access code is missing.");
        }


        var smtpSettings =
            GetSmtpSettings();


        using var message =
            new MailMessage();


        message.From =
            new MailAddress(
                smtpSettings.SenderEmail,
                smtpSettings.SenderName);


        message.To.Add(
            new MailAddress(
                recipientEmail));


        message.Subject =
            "Your School Admin Login Access Code";


        message.IsBodyHtml =
            true;


        message.Body =
            $"""
            <!DOCTYPE html>
            <html>
            <body style="
                margin:0;
                padding:0;
                background:#f6f2e7;
                font-family:Arial,Helvetica,sans-serif;
                color:#173c2d;">

                <div style="
                    max-width:650px;
                    margin:30px auto;
                    background:#ffffff;
                    border-radius:14px;
                    overflow:hidden;
                    border:1px solid #e2dfd2;">

                    <!-- HEADER -->

                    <div style="
                        background:#005640;
                        color:#ffffff;
                        padding:24px 30px;">

                        <h2 style="
                            margin:0;
                            font-size:22px;">

                            School-Parental System

                        </h2>


                        <p style="
                            margin:7px 0 0;
                            color:#f7c844;">

                            School Admin Login Access

                        </p>

                    </div>


                    <!-- CONTENT -->

                    <div style="
                        padding:30px;">

                        <p>
                            Dear
                            <strong>
                                {WebUtility.HtmlEncode(
                                    string.IsNullOrWhiteSpace(adminName)
                                        ? "School Administrator"
                                        : adminName)}
                            </strong>,
                        </p>


                        <p>
                            Your School Admin account has been
                            successfully created for:
                        </p>


                        <h3 style="
                            color:#005640;
                            margin-bottom:20px;">

                            {WebUtility.HtmlEncode(schoolName)}

                        </h3>


                        <p>
                            Use the following access code when
                            signing in as a
                            <strong>School Admin</strong>.
                        </p>


                        <!-- LOGIN ACCESS CODE -->

                        <div style="
                            background:#f8faf7;
                            border:2px solid #f4c542;
                            border-radius:10px;
                            padding:22px;
                            text-align:center;
                            margin:24px 0;">

                            <div style="
                                font-size:12px;
                                text-transform:uppercase;
                                letter-spacing:1px;
                                color:#667066;
                                margin-bottom:8px;">

                                Login Access Code

                            </div>


                            <div style="
                                font-size:30px;
                                font-weight:700;
                                letter-spacing:4px;
                                color:#005640;">

                                {WebUtility.HtmlEncode(loginAccessCode)}

                            </div>

                        </div>


                        <p>
                            When logging in, enter:
                        </p>


                        <div style="
                            background:#f4f7f5;
                            border-radius:8px;
                            padding:16px;
                            line-height:1.8;">

                            <strong>Role:</strong>
                            School Admin

                            <br />

                            <strong>Email:</strong>
                            {WebUtility.HtmlEncode(recipientEmail)}

                            <br />

                            <strong>Password:</strong>
                            Your account password

                            <br />

                            <strong>Access Code:</strong>
                            {WebUtility.HtmlEncode(loginAccessCode)}

                        </div>


                        <div style="
                            margin-top:25px;
                            padding:15px;
                            background:#fff7df;
                            border-radius:8px;
                            color:#725818;">

                            <strong>Important:</strong>
                            Keep this login access code private.
                            Do not share it with unauthorised persons.

                        </div>


                        <p style="
                            margin-top:25px;">

                            If email verification is enabled,
                            verify your email address before
                            attempting to sign in.

                        </p>

                    </div>


                    <!-- FOOTER -->

                    <div style="
                        background:#f4f2ea;
                        padding:18px 30px;
                        font-size:12px;
                        color:#6c746f;">

                        School-Parental System
                        <br />
                        Department of Basic Education

                    </div>

                </div>

            </body>
            </html>
            """;


        await SendEmailAsync(
            message,
            smtpSettings,
            "School Admin login access-code email");
    }


    // =====================================================
    // LOAD SMTP SETTINGS
    // =====================================================

    //
    // Method Name      : GetSmtpSettings
    // Purpose          : Retrieves smtp Settings.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : SmtpConfiguration.
    //

    private SmtpConfiguration GetSmtpSettings()
    {
        var smtpServer =
            _configuration[
                "SmtpSettings:Server"];


        var smtpPortText =
            _configuration[
                "SmtpSettings:Port"];


        var senderName =
            _configuration[
                "SmtpSettings:SenderName"];


        var senderEmail =
            _configuration[
                "SmtpSettings:SenderEmail"];


        var senderPassword =
            _configuration[
                "SmtpSettings:Password"];


        if (string.IsNullOrWhiteSpace(
                smtpServer) ||
            string.IsNullOrWhiteSpace(
                smtpPortText) ||
            string.IsNullOrWhiteSpace(
                senderEmail) ||
            string.IsNullOrWhiteSpace(
                senderPassword))
        {
            throw new InvalidOperationException(
                "SMTP email settings are incomplete.");
        }


        if (!int.TryParse(
                smtpPortText,
                out var smtpPort))
        {
            throw new InvalidOperationException(
                "SMTP port configuration is invalid.");
        }


        return new SmtpConfiguration
        {
            Server =
                smtpServer,

            Port =
                smtpPort,

            SenderName =
                string.IsNullOrWhiteSpace(
                    senderName)
                    ? "School-Parental System"
                    : senderName,

            SenderEmail =
                senderEmail,

            Password =
                senderPassword
        };
    }


    // =====================================================
    // SEND EMAIL
    // =====================================================

    //
    // Method Name      : SendEmailAsync
    // Purpose          : Sends email.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : MailMessage message; SmtpConfiguration settings;
    //                    string emailDescription
    // Output Type      : Task.
    //

    private async Task SendEmailAsync(
        MailMessage message,
        SmtpConfiguration settings,
        string emailDescription)
    {
        using var smtpClient =
            new SmtpClient(
                settings.Server,
                settings.Port);


        smtpClient.EnableSsl =
            true;


        smtpClient.UseDefaultCredentials =
            false;


        smtpClient.Credentials =
            new NetworkCredential(
                settings.SenderEmail,
                settings.Password);


        try
        {
            await smtpClient
                .SendMailAsync(
                    message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send {EmailDescription} to {Recipient}.",
                emailDescription,
                message.To.FirstOrDefault()?.Address);


            throw new InvalidOperationException(
                "The email could not be sent.");
        }
    }


    // =====================================================
    // SMTP CONFIGURATION
    // =====================================================

    //
    // Class Name       : SmtpConfiguration
    // Purpose          : Holds smtp configuration data used by the
    //                    application.
    // Re-use           : Used to configure the corresponding service.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    private sealed class SmtpConfiguration
    {
        public string Server { get; set; } = "";

        public int Port { get; set; }

        public string SenderName { get; set; } = "";

        public string SenderEmail { get; set; } = "";

        public string Password { get; set; } = "";
    }
}

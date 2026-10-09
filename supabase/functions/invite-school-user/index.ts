import "jsr:@supabase/functions-js/edge-runtime.d.ts";
import { createClient } from "npm:@supabase/supabase-js@2";

function adminClient() {
  const modern = Deno.env.get("SUPABASE_SECRET_KEYS");
  let key = "";
  if (modern) {
    const parsed = JSON.parse(modern);
    key = parsed.default ?? "";
  }
  if (!key) key = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY") ?? "";
  return createClient(Deno.env.get("SUPABASE_URL")!, key, {
    auth: { persistSession: false, autoRefreshToken: false },
  });
}

Deno.serve(async (req) => {
  if (req.method !== "POST") return new Response("Method not allowed", { status: 405 });

  try {
    const token = (req.headers.get("Authorization") ?? "").replace(/^Bearer\s+/i, "");
    if (!token) return Response.json({ error: "Authentication required" }, { status: 401 });

    const supabase = adminClient();
    const { data: authData, error: authError } = await supabase.auth.getUser(token);
    if (authError || !authData.user) {
      return Response.json({ error: "Invalid session" }, { status: 401 });
    }

    const { data: caller } = await supabase
      .from("users")
      .select("id, role, school_id, is_active")
      .eq("id", authData.user.id)
      .maybeSingle();

    if (!caller || caller.is_active !== true || !["Super Admin", "School Admin"].includes(caller.role)) {
      return Response.json({ error: "Only an active administrator may add users" }, { status: 403 });
    }

    const body = await req.json();
    const email = body.email ? String(body.email).trim().toLowerCase() : null;
    const phone = body.phone ? String(body.phone).trim() : null;
    const fullName = String(body.full_name ?? "").trim();
    const role = String(body.role ?? "").trim();
    const requestedSchool = body.school_id ? String(body.school_id).trim() : null;

    if (!fullName || !["Teacher", "Parent"].includes(role)) {
      return Response.json({ error: "full_name and role Teacher/Parent are required" }, { status: 400 });
    }
    if (!email && !phone) {
      return Response.json({ error: "Email or phone is required" }, { status: 400 });
    }
    if (role === "Teacher" && !email) {
      return Response.json({ error: "Teachers require an email invitation" }, { status: 400 });
    }

    const schoolId = caller.role === "School Admin" ? caller.school_id : requestedSchool;
    if (!schoolId) return Response.json({ error: "A school is required" }, { status: 400 });

    const { data: school } = await supabase.from("schools").select("id").eq("id", schoolId).maybeSingle();
    if (!school) return Response.json({ error: "School not found" }, { status: 404 });

    if (role === "Teacher" && !String(body.employee_id ?? "").trim()) {
      return Response.json({ error: "employee_id is required for Teacher" }, { status: 400 });
    }
    if (role === "Parent" && !String(body.guardian_number ?? "").trim()) {
      return Response.json({ error: "guardian_number is required for Parent" }, { status: 400 });
    }

    let userId = "";

    if (email) {
      const { data: inviteData, error: inviteError } = await supabase.auth.admin.inviteUserByEmail(email, {
        data: { full_name: fullName, role, school_id: schoolId },
      });
      if (inviteError || !inviteData.user) {
        return Response.json({ error: inviteError?.message ?? "Unable to invite user" }, { status: 400 });
      }
      userId = inviteData.user.id;
    } else {
      const { data: created, error: createError } = await supabase.auth.admin.createUser({
        phone: phone!,
        phone_confirm: false,
        user_metadata: { full_name: fullName },
        app_metadata: { role, school_id: schoolId },
      });
      if (createError || !created.user) {
        return Response.json({ error: createError?.message ?? "Unable to create phone user" }, { status: 400 });
      }
      userId = created.user.id;
    }

    const { error: profileError } = await supabase.from("users").insert({
      id: userId,
      email,
      phone,
      full_name: fullName,
      role,
      school_id: schoolId,
      is_active: true,
    });

    if (profileError) {
      await supabase.auth.admin.deleteUser(userId);
      return Response.json({ error: "Unable to create application user profile" }, { status: 500 });
    }

    if (role === "Teacher") {
      const { error } = await supabase.from("teachers").insert({
        user_id: userId,
        school_id: schoolId,
        employee_id: String(body.employee_id).trim(),
        specialization: body.specialization ? String(body.specialization).trim() : null,
        joining_date: body.joining_date ?? null,
        alternative_phone: body.alternative_phone ? String(body.alternative_phone).trim() : null,
        address: body.address ? String(body.address).trim() : null,
        gender: body.gender ? String(body.gender).trim() : null,
        date_of_birth: body.date_of_birth ?? null,
        is_active: true,
      });
      if (error) {
        await supabase.from("users").delete().eq("id", userId);
        await supabase.auth.admin.deleteUser(userId);
        return Response.json({ error: "Unable to create teacher profile" }, { status: 500 });
      }
    } else {
      const { error } = await supabase.from("parents").insert({
        user_id: userId,
        guardian_number: String(body.guardian_number).trim(),
        alternative_phone: body.alternative_phone ? String(body.alternative_phone).trim() : null,
        address: body.address ? String(body.address).trim() : null,
        gender: body.gender ? String(body.gender).trim() : null,
        is_active: true,
      });
      if (error) {
        await supabase.from("users").delete().eq("id", userId);
        await supabase.auth.admin.deleteUser(userId);
        return Response.json({ error: "Unable to create parent profile" }, { status: 500 });
      }
    }

    return Response.json({
      message: email
        ? role + " invited successfully by email"
        : "Parent phone account created. They can verify/sign in with SMS OTP once Phone Auth is configured.",
      user: { id: userId, email, phone, role, school_id: schoolId },
    }, { status: 201 });
  } catch {
    return Response.json({ error: "Invalid request" }, { status: 400 });
  }
});
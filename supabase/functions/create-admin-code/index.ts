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

async function sha256Hex(value: string) {
  const hash = await crypto.subtle.digest("SHA-256", new TextEncoder().encode(value));
  return Array.from(new Uint8Array(hash))
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("");
}

function randomCode() {
  const bytes = new Uint8Array(16);
  crypto.getRandomValues(bytes);
  return "ADM-" + Array.from(bytes)
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("")
    .toUpperCase();
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
      .select("id, role, is_active")
      .eq("id", authData.user.id)
      .maybeSingle();

    if (!caller || caller.role !== "Super Admin" || caller.is_active !== true) {
      return Response.json({ error: "Only an active Super Admin can create codes" }, { status: 403 });
    }

    const body = await req.json();
    const adminRole = String(body.admin_role ?? "").trim();
    const schoolId = body.school_id ? String(body.school_id).trim() : null;
    let hours = Number(body.expires_in_hours ?? 72);
    if (!Number.isFinite(hours) || hours < 1) hours = 72;
    if (hours > 168) hours = 168;

    if (!["Super Admin", "School Admin"].includes(adminRole)) {
      return Response.json({ error: "Invalid admin role" }, { status: 400 });
    }
    if (adminRole === "School Admin" && !schoolId) {
      return Response.json({ error: "school_id is required" }, { status: 400 });
    }
    if (adminRole === "Super Admin" && schoolId) {
      return Response.json({ error: "Super Admin code cannot have school_id" }, { status: 400 });
    }

    if (schoolId) {
      const { data: school } = await supabase.from("schools").select("id").eq("id", schoolId).maybeSingle();
      if (!school) return Response.json({ error: "School not found" }, { status: 404 });
    }

    const code = randomCode();
    const codeHash = await sha256Hex(code);
    const expiresAt = new Date(Date.now() + hours * 60 * 60 * 1000).toISOString();

    const { data, error } = await supabase
      .from("admin_registration_codes")
      .insert({
        code_hash: codeHash,
        admin_role: adminRole,
        school_id: adminRole === "School Admin" ? schoolId : null,
        expires_at: expiresAt,
        is_active: true,
        created_by: caller.id,
      })
      .select("id, admin_role, school_id, expires_at")
      .single();

    if (error) return Response.json({ error: "Could not create registration code" }, { status: 500 });

    return Response.json({ code, registration: data }, { status: 201 });
  } catch {
    return Response.json({ error: "Invalid request" }, { status: 400 });
  }
});
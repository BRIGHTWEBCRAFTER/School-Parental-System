
create or replace function public.claim_admin_registration(
  p_secret_code text,
  p_full_name text,
  p_phone text default null
)
returns public.users
language plpgsql
security definer
set search_path = ''
as $$
declare
  v_uid uuid := auth.uid();
  v_email text;
  v_phone text;
  v_code public.admin_registration_codes%rowtype;
  v_profile public.users%rowtype;
  v_hash text;
begin
  if v_uid is null then
    raise exception 'Authentication required';
  end if;

  if exists (select 1 from public.users u where u.id = v_uid) then
    raise exception 'User profile already exists';
  end if;

  select au.email, au.phone
    into v_email, v_phone
  from auth.users au
  where au.id = v_uid;

  if v_email is null and v_phone is null then
    raise exception 'Authenticated user must have email or phone';
  end if;

  v_hash := encode(extensions.digest(p_secret_code, 'sha256'), 'hex');

  select arc.*
    into v_code
  from public.admin_registration_codes arc
  where arc.code_hash = v_hash
    and arc.is_active = true
    and arc.used_at is null
    and (arc.expires_at is null or arc.expires_at > now())
  for update
  limit 1;

  if not found then
    raise exception 'Invalid, expired, or already-used registration code';
  end if;

  insert into public.users (
    id, email, full_name, role, school_id, phone, is_active
  )
  values (
    v_uid,
    lower(v_email),
    trim(p_full_name),
    v_code.admin_role,
    v_code.school_id,
    coalesce(nullif(trim(p_phone), ''), v_phone),
    true
  )
  returning * into v_profile;

  update public.admin_registration_codes
  set is_active = false,
      used_at = now(),
      used_by = v_uid
  where id = v_code.id;

  return v_profile;
end;
$$;

revoke all on function public.claim_admin_registration(text, text, text) from public, anon;
grant execute on function public.claim_admin_registration(text, text, text) to authenticated;

create or replace function private.protect_announcement_recipient_identity()
returns trigger
language plpgsql
set search_path = ''
as $$
begin
  if new.announcement_id is distinct from old.announcement_id
     or new.user_id is distinct from old.user_id
     or new.created_at is distinct from old.created_at then
    raise exception 'Announcement recipient identity fields cannot be changed';
  end if;
  return new;
end;
$$;

create trigger trg_protect_announcement_recipient_identity
before update on public.announcement_recipients
for each row execute function private.protect_announcement_recipient_identity();

create or replace function private.protect_notification_identity()
returns trigger
language plpgsql
set search_path = ''
as $$
begin
  if new.user_id is distinct from old.user_id
     or new.announcement_id is distinct from old.announcement_id
     or new.event_id is distinct from old.event_id
     or new.grade_id is distinct from old.grade_id
     or new.attendance_id is distinct from old.attendance_id
     or new.title is distinct from old.title
     or new.message is distinct from old.message
     or new.notification_type is distinct from old.notification_type
     or new.created_at is distinct from old.created_at then
    raise exception 'Notification source fields cannot be changed';
  end if;
  return new;
end;
$$;

create trigger trg_protect_notification_identity
before update on public.notifications
for each row execute function private.protect_notification_identity();
;

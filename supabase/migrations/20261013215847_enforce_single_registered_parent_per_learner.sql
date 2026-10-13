
alter table public.parent_learners
  add constraint uq_parent_learners_learner
  unique (learner_id);

comment on constraint uq_parent_learners_learner
on public.parent_learners is
  'Each learner may have only one registered parent or guardian. A parent may be linked to many learners.';
;

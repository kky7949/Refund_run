# main Workspace Rules

This file contains local rules for work under this folder.

## Communication

- Start with the cause, impact, and fix when debugging.
- If a request is ambiguous, make a conservative assumption and state it briefly.
- Ask before destructive changes, broad refactors, or regeneration of existing scene/prefab content.

## Architecture

- Prefer the existing project pattern over introducing new abstractions.
- Keep each script responsible for one clear job.
- Add managers only when one object must coordinate several independent objects.
- Use Inspector-exposed fields for values that designers will tune.

## Unity Editing

- Treat existing Inspector and Scene-view values as source of truth.
- Editor tools should preserve manual Transform, Sprite, Collider, Animator, and serialized field values.
- Builders should add missing pieces, not rebuild working content.
- Use clear child names like `Visual`, `Collider`, `Sensors`, `UI`, and `Debug`.


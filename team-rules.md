# Team Rules

## Branches

- Work on a feature branch.
- Make a checkpoint commit before risky Unity operations:
  - scene-wide changes
  - asset refresh with compile
  - play-mode verification after large edits
  - merges

## Unity Scenes

- Decide scene ownership before multiple people edit the same scene.
- If a scene is dirty, do not refresh, enter Play Mode, or save automatically without checking intent.
- Do not regenerate scene content that a person manually positioned unless explicitly asked.

## Assets

- Do not raw-edit `.unity`, `.prefab`, `.asset`, or `.mat` unless the change is targeted and followed by reserialize.
- Prefer non-destructive editor tools that fill missing references or add missing helper components.

## Merge Checks

Run after merges or large Unity changes:

```powershell
unity-cli scene_status
unity-cli safe_refresh --compile true
unity-cli console --type error
```


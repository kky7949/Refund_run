# Team Rules

## Branches

- Current project workflow is direct work on `main` unless the team explicitly asks for a feature branch.
- Personal work files go under `Assets/_WIP/<name>`; Yongwoo's folder is `Assets/_WIP/yongwoo`.
- Make a checkpoint commit before risky Unity operations:
  - scene-wide changes
  - asset refresh with compile
  - play-mode verification after large edits
  - merges

## Unity Scenes

- Decide scene ownership before multiple people edit the same scene.
- Each person works in their own scene. Do not edit another person's scene directly.
- Put owned draft scenes, helper objects, and local notes in the matching personal `_WIP` folder when they are not ready to be shared.
- Treat `Stage1` as the reference/base scene for copying layout, player setup, movement feel, map structure, traps, and quiz placement ideas.
- To build a new map, duplicate/copy from `Stage1` into a new owned scene first, then decorate the map and add traps, quiz triggers, or puzzle hookups in that owned scene.
- Existing shared/reference scenes, including `Stage1`, are read-only unless the scene owner explicitly asks for a direct edit.
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


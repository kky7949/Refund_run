# Refund_run AI 작업 진입 안내

너는 나와 Unity 프로젝트를 같이 만드는 동료 개발자다.

이 프로젝트의 진입점은 이 파일(`AGENTS.md`)이다. `CLAUDE.md`는 하네스 호환용 1줄 stub.

---

## 세션 시작 시 읽는 순서

1. **`AGENTS.md`** — 도구와 진입 규칙
2. **`STATUS.md`** — 지금 어디까지 왔는지, 다음 무엇을 할지
3. **`team-rules.md`** — 브랜치, 머지, 씬 분담 규칙
4. 작업 폴더에 더 가까운 **`AGENTS.md`** 가 있으면 추가로 읽는다

## 세션 종료 시

`STATUS.md`를 갱신한다. 갱신 규칙은 그 파일 하단을 따른다.

## 문서 추가 규칙

- 새 `.md`를 만들기 전에 `AGENTS.md`, `STATUS.md`, `team-rules.md` 중 하나에 흡수할 수 있는지 먼저 검토한다.
- 새 문서가 필요하면 왜 별도 문서여야 하는지 짧게 남긴다.

## 사고 과정 출력 규칙

여러 선택지 중에서 결정하거나, 파일 수정 / 위험한 도구 호출을 할 때 **왜 이걸 선택했는지 한 줄을 행동 직전에 먼저 출력한다.**

- 단순 확인 작업은 생략 가능
- 추측이 들어간 결정은 추측이라고 명시
- 한 줄로 끝낸다

---

# Unity Workspace Rules

## 기본 원칙

- 기존 구조와 로컬 패턴을 먼저 확인한다.
- 같은 기능이 이미 있는지 확인한 뒤 새 코드를 만든다.
- "안 된다"는 요청에는 바로 수정하지 말고 원인부터 확인한다.
- 사람이 Inspector/Scene에서 조정한 값은 현재 source of truth로 취급한다.
- 되돌리기 어려운 변경은 먼저 확인한다.

## unity-cli

Unity 에디터 제어는 `unity-cli`를 사용한다.

### 기본 명령

- `unity-cli status` — 연결 확인
- `unity-cli list` — 사용 가능한 도구 목록 + 파라미터 스키마 확인
- `unity-cli scene_status` — 열린 씬 dirty/play/compile 상태 확인
- `unity-cli safe_refresh --compile true` — dirty/play/compile 가드가 있는 refresh
- `unity-cli console --type error` — 에러 로그 확인
- `unity-cli exec "<C# code>"` — C# 코드 실행
- `unity-cli editor play --wait` — 플레이 모드 진입
- `unity-cli reserialize <경로>` — 텍스트 편집한 Unity 자산 재직렬화

### 작업 시작 시

- 세션 시작 시 `unity-cli list`를 먼저 실행해 사용 가능한 도구와 스키마를 확인한다.
- 모르는 도구 이름이나 파라미터는 추측해서 호출하지 않는다.

### 안전 가드

- `editor refresh`, `safe_refresh`, `editor play` 전에는 `unity-cli scene_status`로 dirty/play/compile 상태를 확인한다.
- 저장 안 된 씬이 있으면 멈추고 사용자에게 저장 의사를 묻는다.
- 플레이 모드 중에는 refresh, 에셋 수정, 컴파일 트리거를 하지 않는다.
- `.prefab`, `.unity`, `.asset`, `.mat`을 텍스트로 직접 편집했으면 `unity-cli reserialize <경로>`로 마무리한다.

### exec 사용 시

- 한 줄 단순 코드만 인라인 `"..."` 형식으로 쓴다.
- 복잡한 코드는 stdin 파이프를 사용한다.

```powershell
'var go = new UnityEngine.GameObject("X"); return go.name;' | unity-cli exec
```

## unity-scanner

Unity 프로젝트 파일의 정적 분석은 가능하면 `unity-scanner`를 사용한다.

### 기본 명령

- `unity-scanner list -p <project> [path]`
- `unity-scanner read -p <project> <asset>`
- `unity-scanner search -p <project> [path] [filters]`
- `unity-scanner refs -p <project> <asset-or-guid>`

### 사용 규칙

- `.unity`, `.prefab`, `.asset` 파일은 먼저 `unity-scanner read`로 본다.
- 디렉토리 탐색은 `unity-scanner list`를 우선한다.
- 자산 검색은 `unity-scanner search`를 우선한다.
- 참조 추적은 `unity-scanner refs`를 우선한다.

## 토큰 절약

다음 폴더는 읽지 않는다:

- `Library/`
- `Temp/`
- `Logs/`
- `Build/`
- `Builds/`
- `obj/`
- `UserSettings/`

다음 파일은 통째로 읽지 않는다:

- `.unity`
- `.prefab`
- `.asset`
- `.mat`

## 선택 우선 검사

사용자가 "선택한 오브젝트", "이 에셋", "방금 클릭한 것"처럼 Unity 선택 상태를 기준으로 말하면 이름이나 폴더를 추측하지 말고 먼저 읽기 전용 도구를 사용한다.

- `unity-cli inspect_selection`
- `unity-cli inspect_selection_set`
- `unity-cli inspect_selected_asset_refs`

## Editor Tool 원칙

- `AIInspectionTools`는 읽기 전용 진단 도구다.
- 읽기 도구에는 `SetDirty`, `SaveAssets`, `SaveScene`, `AddComponent`, `Destroy`, `MoveAsset` 같은 수정 동작을 넣지 않는다.
- 자동화가 필요하면 없는 컴포넌트/참조만 채우는 비파괴 갱신을 우선한다.


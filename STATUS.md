# 작업 상태

> 다음 세션 시작 시 먼저 읽는다. 세션 끝에 갱신한다.

## 지금 어디까지

- `setup/unity-harness` 브랜치에서 Unity AI harness 적용 완료.
- `Packages/manifest.json`에 `com.youngwoocho02.unity-cli-connector` 추가.
- `Assets/AIHarness/Editor`에 AI inspection/safety editor tools 설치.
- `unity-cli` 검증 통과: `list`, `scene_status`, `safe_refresh --compile true`, `console --type error`.
- 기획안 PDF를 `Assets/Docs/환불런 (Refund Run) 게임기획안.pdf`로 복사.
- 저울 무게추 퍼즐 씬 `Assets/Scenes/ScalePuzzle.unity` 추가.
- `ScalePuzzleController`를 레이튼식 무게추 추리 퍼즐로 변경: 랜덤 템플릿, 제한 저울질, 좌/우 접시 드래그 배치, 기록, 정답 후보 선택, 결정 처리.
- 퍼즐 씬은 UI 전용 보드로 재구성: 문제 카드, 저울 접시, 정답 트레이, 추 풀, 행동 버튼을 한 화면에서 조작.
- 추는 `ScalePuzzleWeightItem`으로 드래그하고, `ScalePuzzleDropZone`이 좌/우 접시와 정답 트레이 드롭을 처리한다.
- 드롭 영역 hover 하이라이트와 드래그 중 추 확대 피드백 추가.
- 현재 템플릿: 8개/9개/12개 추 중 하나가 가볍거나 무거운 문제를 랜덤 출제.
- Play Mode에서 EventSystem 드래그/드롭으로 추를 왼쪽 접시에 올리는 경로 확인.
- `ScalePuzzleWeightItem`의 `RectTransform`/`CanvasGroup` 캐시를 lazy init으로 보강해 `Controller.Awake()`가 먼저 실행될 때 나는 NullReference 수정.
- ScreenSpaceCamera 캔버스에서 드래그 좌표가 틀어지던 문제 수정: 포인터 스크린 좌표를 `RectTransformUtility.ScreenPointToWorldPointInRectangle`로 변환해 추 위치에 반영.
- 검증 스크린샷: `Temp/scale_puzzle_drag_ui_polished.png`.
- 콘솔 에러 없음 확인.
- `Stage1`은 다른 사람 작업 씬으로 보고 변경하지 않도록 원본 상태로 복구했다.
- 새 플랫포머 씬 `Assets/Scenes/RefundRun_Platformer.unity` 추가.
- `RefundRun_Platformer`는 `Palmov Island/Low Poly Atmospheric Locations Pack`의 downtown 환경 프리팹과 플랫폼 큐브로 맵을 구성한다.
- `Mini Simple Characters Demo` 캐릭터 프리팹 루트에 기존 `NewMoveCS`를 붙여 `Stage1`과 같은 Player 구조/이동 방식을 사용한다.
- `Quiz Chest`에 `ScalePuzzleTrigger`를 붙여 접촉 시 `ScalePuzzle` 씬을 additive overlay로 로드한다.
- `ScalePuzzleController`는 정답 결정 시 `SolvedCorrectly` 이벤트를 발생시키고, 오답은 퍼즐을 닫지 않도록 변경했다.
- 정답 완료 후 `ScalePuzzle` 씬 unload, 플레이어 조작 복구, 퀴즈 오브젝트 비활성화 흐름을 Play Mode에서 확인했다.
- `ProjectSettings/EditorBuildSettings.asset`에 `RefundRun_Platformer`와 `ScalePuzzle` 씬이 등록되어 런타임 additive 로드가 가능하다.
- `Palmov Island` 공용 머티리얼 `mat main`의 Built-in `Standard` 셰이더를 `Universal Render Pipeline/Lit`로 변환해 마젠타 렌더링 문제를 수정했다.
- 새 씬의 플레이어 이동이 별도 2.5D 컨트롤러라 `Stage1`과 다르게 느껴지던 문제를 수정했다. `PlatformerPlayerController`는 제거하고 `NewMoveCS` 기반으로 재빌드했다.
- `ScalePuzzle` 저울 기울기 방향을 실제 무거운 접시 쪽이 내려가도록 수정했다.
- `ScalePuzzle` UI 문구를 한글로 바꾸고 `NotoSansKR-VF` 폰트를 `Assets/Fonts`에 추가해 한글 네모 표시를 피했다.

## 핵심 설계 결정

- `AGENTS.md`를 AI 작업 진입점으로 사용한다.
- Unity 에디터 제어는 `unity-cli`를 사용한다.
- Unity 자산 구조 확인은 가능하면 `unity-scanner`를 우선한다.
- 사람이 Inspector/Scene에서 조정한 값은 현재 source of truth로 본다.
- 다른 사람이 만든 씬은 직접 확장하지 않고, 참고만 한 뒤 새 씬을 만들어 작업한다.
- 저울 퀴즈는 새 UI를 만들지 않고 `ScalePuzzle` 씬을 additive overlay로 재사용한다.
- 캐릭터 이동은 새 컨트롤러를 만들기보다 기존 씬의 검증된 `NewMoveCS` 구조를 우선 재사용한다.

## 남은 작업

- [ ] 프로젝트 컨셉/핵심 루프 정리
- [ ] 입력 매핑 정리
- [x] 주요 씬/프리팹 구조 정리
- [x] 검증 명령 통과 확인
- [x] 새 플랫포머 씬과 저울 퍼즐 연동 구현

## 알려진 미해결

- 하네스 install 스크립트가 Windows PowerShell에서 `manifest.json`을 BOM 포함 UTF-8로 저장해 Unity Package Manager가 한 번 JSON 오류를 냈다. 이 프로젝트의 `manifest.json`은 BOM 없는 UTF-8로 수정 완료.
- 현재 퍼즐은 임시 아트 기반 UI다. 추/저울 전용 스프라이트를 넣으면 같은 드래그 로직 위에서 시각 품질만 교체 가능.
- `RefundRun_Platformer`의 플랫폼은 아직 블록아웃 성격이다. 실제 레벨 디자인은 Inspector/Scene에서 위치와 간격을 추가 조정하면 된다.

## 세션 종료 시 갱신 규칙

1. **지금 어디까지** 갱신
2. **핵심 설계 결정** 추가
3. 완료한 **남은 작업** 체크
4. 다음 세션이 볼 **알려진 미해결** 갱신

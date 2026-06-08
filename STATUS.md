# 작업 상태

> 다음 세션 시작 시 먼저 읽는다. 세션 끝에 갱신한다.

## 지금 어디까지

- `setup/unity-harness` 브랜치에서 Unity AI harness 적용 완료.
- `Packages/manifest.json`에 `com.youngwoocho02.unity-cli-connector` 추가.
- `Assets/AIHarness/Editor`에 AI inspection/safety editor tools 설치.
- `unity-cli` 검증 통과: `list`, `scene_status`, `safe_refresh --compile true`, `console --type error`.
- 기획안 PDF를 `Assets/Docs/환불런 (Refund Run) 게임기획안.pdf`로 복사.
- 저울 무게추 퍼즐 씬 `Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity` 추가.
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
- 새 플랫포머 씬 `Assets/_WIP/yongwoo/Scenes/RefundRun_Platformer.unity` 추가.
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
- 기존 `Assets/_WIP/main` 개인 작업 폴더를 `Assets/_WIP/yongwoo`로 옮겨 메인 브랜치 이름과 개인 작업공간 이름이 섞이지 않게 했다.
- 우리가 만든 `RefundRun_Platformer`, `ScalePuzzle`, 관련 트리거/퍼즐/빌더 스크립트를 `Assets/_WIP/yongwoo` 아래로 옮겼다.
- 복구 기준 커밋 `3956542`에서 개인 작업 브랜치 `yongwoo`를 새로 만들었다.
- `ScalePuzzle` 저사양 최적화 1차 적용: ScreenSpaceOverlay 전환, 퍼즐 씬 내부 Camera/AudioListener 비활성, Shadow/Outline 제거, 불필요한 UI raycastTarget 축소.
- `ScalePuzzleController`의 무게추 갱신을 전체 12개 재배치 방식에서 이동한 추 중심 갱신으로 줄였다.
- `ScalePuzzle.unity`를 저사양 UI 빌더 설정으로 재생성해 컴포넌트 수를 310개에서 270개로 줄였다.
- 검증: `unity-cli safe_refresh --compile true`, `unity-cli console --type error`, ScalePuzzle 단독 Play Mode 진입/종료 에러 없음.
- `origin/byeong`의 Stage1을 직접 병합하지 않고 `Assets/_WIP/yongwoo/Reference/Byeong_Stage1.unity` 참고 씬으로 가져왔다.
- `byeong` Stage1 참고를 위해 `MovingTrap`, `FallingTrap`, `MemoryPuzzle`, 배경/TMP 한글 폰트 에셋을 가져왔다.
- 플레이어/카메라 감각을 `byeong` Stage1 계열로 맞추기 위해 `NewMoveCS cp.cs`와 `CameraFollow.cs`를 갱신했고, `PlatformerStageBuilder`의 카메라 필드명을 새 `fixedZ` 구조에 맞췄다.
- `RefundRun_Platformer`를 도로 위 회피 스테이지로 재구성했다. 긴 도로, 인도/차선/안전지대, 움직이는 차 2개, 움직이는 나무 1개, 훼이크 차/나무, 끝 지점 저울 퍼즐 상자를 생성한다.
- 함정 피격 연출은 `NewMoveCS`의 장애물 충돌 시 Rigidbody 제약 해제, 랜덤 힘/토크, 리스폰 후 함정 reset 흐름을 사용한다.
- 검증: 도로 스테이지 빌더 실행, `unity-cli console --type error` 에러 없음, `RefundRun_Platformer` Play Mode 진입/종료 에러 없음.
- 도로 스테이지에서 큰 건물/장식 나무가 플레이어를 가리던 문제를 수정했다. `downtown with environment` 배경과 전경 장식 나무를 빌더에서 제거했다.
- 점프/이동 스크립트는 우리가 참고용으로 가져온 `byeong` 상태를 유지한다. `NewMoveCS`에 추가했던 별도 착지/idle Y 안정화 수정은 공용 스크립트 영향 때문에 제거했다.
- `RefundRun_Platformer`의 플레이어 배치를 `Byeong_Stage1` 참고값에 다시 맞췄다. 시작 위치 Y, 회전, 이동 속도, 점프 힘, Rigidbody 제약, CapsuleCollider 값을 기준 씬과 동일하게 정리했다.
- 도로 장애물도 참고씬의 2D식 라인 기준으로 정리했다. 차/나무통은 모두 `Z=0` 라인에서 움직이고, 프리팹 자식 콜라이더는 끈 뒤 루트 BoxCollider 1개만 활성화해 보이는 위치와 피격 판정이 크게 어긋나지 않게 했다.
- 나무통 장애물은 회전 때문에 콜라이더가 세로로 과하게 커지던 문제를 수정했다. 실제 판정은 도로 위 낮은 장애물 높이로 조정했다.
- 중간 도로 구간에 움직이는 `Timing Cross Stone` 십자돌 플랫폼을 추가해 타이밍을 맞춰 건너는 점프/이동 구간을 넣었다.
- 플레이어가 가만히 있을 때 위로 떠오르던 문제를 수정했다. 캡슐 발끝이 도로 안에 약간 박혀 있어 Unity 물리가 위로 밀어 올리던 것이 원인이었고, `PlayerSpawnY`를 도로 윗면/캡슐 높이/캡슐 중심/접촉 여백으로 계산하도록 바꿨다.
- 시작/퀴즈 안전 구역과 차선 표시 큐브를 도로 표면에 매우 얇게 붙여, 표시 오브젝트가 플레이어 발을 덮어 떠 보이던 시각 문제도 줄였다.
- 공용 플레이어 스크립트 영향이 커서 `NewMoveCS cp.cs`에 추가했던 idle Y 안정화 수정은 제거했다. 파일은 우리가 참고용으로 가져온 `Refund_run_byeong_reference`의 `88aeb69 backup to byeong` 상태와 동일하게 맞췄다.
- 퍼즐 직전 필수 구간으로 Z축 `Cross Traffic Road`를 추가했다. `Cross Traffic Car North/South`는 `YongwooDepthTrafficObstacle`로 Z축을 따라 주행하고, 닿으면 공용 `NewMoveCS`의 기존 죽음/리스폰 흐름을 호출한다.
- `RefundRun_Platformer` 카메라를 yongwoo 전용 `YongwooRoadCameraFollow`로 바꿔 비스듬한 2.5D 탑뷰에서 Z축 차로와 차량 이동이 보이게 했다. 공유 `CameraFollow`는 수정하지 않았다.
- 검증: `unity-cli safe_refresh --compile true`, `unity-cli console --type error`, `unity-scanner read`, Play Mode에서 Z축 차 이동/죽음 후 리스폰/`ScalePuzzle` additive 로드 및 플레이어 조작 잠금 확인.
- 플레이어가 떠오르던 문제는 플레이어 Animator의 Root Motion 체크 해제로 해결했다. 별도 물리 보정 스크립트 `YongwooPlayerGroundStabilizer`는 씬과 코드에서 제거했다.
- `YongwooRoadCameraFollow`는 원래 2.5D 고정 Z 카메라 흐름으로 되돌렸고, `YongwooDepthTrafficObstacle`는 초보자가 읽기 쉬운 단순 흐름과 한글 주석 중심으로 정리했다.
- Z축 차량이 오는 방향을 조금 더 읽기 쉽게 `YongwooRoadCameraFollow`에 X축 15도 카메라 각도를 적용했고, `offsetY=3.75`로 플레이어가 화면 중앙 근처에 오게 맞췄다.
- 도시 배경 이미지 `Assets/_WIP/yongwoo/Materials/kthan6v7.jpg`를 `Main Camera` 자식 `City Background` 쿼드로 배치했고, `Assets/_WIP/yongwoo/Materials/city background.mat`으로 연결했다.
- 공용 스크립트 변경에 덜 흔들리도록 `RefundRun_Platformer`의 플레이어를 `YongwooPlayerController`, X축 장애물을 `YongwooMovingTrap`로 교체했다. 씬 안에는 `NewMoveCS`, `MovingTrap`, `CameraFollow` 컴포넌트가 남지 않게 정리했다.
- Skybox 방식은 이미지 왜곡이 있어 사용하지 않기로 하고, 도시 배경은 다시 `Main Camera/City Background` 쿼드 방식으로 유지한다. `RenderSettings.skybox`는 비워두고 카메라는 `SolidColor` 배경 위에 쿼드를 렌더링한다.
- `RefundRun_Platformer`와 `ScalePuzzle`이 쓰는 외부 아트/폰트 에셋을 `Assets/_WIP/yongwoo/ExportAssets` 아래로 복사하고 씬/빌더 참조를 yongwoo 내부 경로로 바꿨다. 두 씬 기준 yongwoo 밖 일반 `Assets/` 의존성은 0개로 확인했다.
- export 범위를 단순하게 만들기 위해 참고용 `Assets/_WIP/yongwoo/Reference/Byeong_Stage1.unity` 폴더는 제거했다. 실제 작업 씬에는 참고 씬이 필요하지 않다.
- `ScalePuzzleTrigger`는 에디터 Play Mode에서 Build Settings에 씬이 없어도 `Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity` 경로로 additive 로드한다. Build Settings에서 yongwoo 씬을 임시 제거한 상태로 `ScalePuzzle` 로드 성공을 확인했다.

## 핵심 설계 결정

- `AGENTS.md`를 AI 작업 진입점으로 사용한다.
- Unity 에디터 제어는 `unity-cli`를 사용한다.
- Unity 자산 구조 확인은 가능하면 `unity-scanner`를 우선한다.
- 사람이 Inspector/Scene에서 조정한 값은 현재 source of truth로 본다.
- 다른 사람이 만든 씬은 직접 확장하지 않고, 참고만 한 뒤 새 씬을 만들어 작업한다.
- 현재 협업 방식은 `main`에서 직접 작업하되, 각자 소유 씬에서만 맵/함정/퀴즈를 편집한다.
- 개인 작업 파일은 `Assets/_WIP/<name>` 아래에 둔다. Yongwoo 개인 폴더는 `Assets/_WIP/yongwoo`다.
- `Stage1`은 기준 씬으로만 사용한다. 새 맵은 `Stage1`을 복사해 각자 씬을 만든 뒤 꾸미고 함정/퀴즈를 추가한다.
- 저울 퀴즈는 새 UI를 만들지 않고 `ScalePuzzle` 씬을 additive overlay로 재사용한다.
- 캐릭터 이동은 공용 `NewMoveCS`를 직접 참조하지 않고, yongwoo 전용 `YongwooPlayerController`로 유지한다.
- Z축 도로/카메라/전용 교차로 장애물은 공유 스크립트가 아니라 `Assets/_WIP/yongwoo` 전용 스크립트로 처리한다.
- 플레이어 부유 현상은 먼저 Animator Root Motion, Collider 위치, Rigidbody 상태를 확인한다. 공용 플레이어 스크립트를 수정하기보다 yongwoo 전용 보조 스크립트로 제한한다.

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
- 저사양 최적화는 ScalePuzzle 단독 씬 기준으로 검증했다. `RefundRun_Platformer`에서 additive overlay로 여는 전체 흐름은 다음 작업에서 다시 확인해야 한다.
- `Byeong_Stage1` 참고용 복사본은 export 준비 과정에서 제거했다. 이후 다시 참고가 필요하면 원본 브랜치나 별도 worktree에서만 확인한다.
- 도로 스테이지는 Z축 교차로 추가 후 smoke test를 통과했다. 실제 난이도, 차/나무/Z축 차량 타이밍은 에디터에서 직접 조작하며 감각 조정이 필요하다.
- 시작 지점 기준 Play Mode에서 콘솔 에러 없음은 확인했다. 플레이어 부유 현상은 Root Motion 해제로 처리했고, 별도 idle Y 보정 코드는 현재 제거된 상태다.
- 도시 배경은 `Assets/_WIP/yongwoo/Materials/kthan6v7.jpg`로 적용 완료했다. Skybox가 아니라 `Main Camera/City Background` 쿼드 방식이며, 밝기/크기/보이는 위치는 해당 오브젝트에서 Inspector로 조정하면 된다.
- yongwoo 폴더만 export할 수 있도록 `ExportAssets`에 필요한 아트/폰트 복사본을 넣었다. 단, Unity 패키지 의존성인 URP/InputSystem/UGUI는 프로젝트에 설치되어 있어야 한다.
- `ProjectSettings/EditorBuildSettings.asset`를 커밋하지 않아도 에디터 Play Mode의 퍼즐 로드는 yongwoo 경로 fallback으로 동작한다. 실제 빌드 파일을 만들 때는 팀 프로젝트 Build Settings에 두 씬을 추가해야 한다.
- CLI로 켠 Play Mode가 `frame=2` 이후 진행하지 않아 십자돌의 지속 이동은 수치로 끝까지 검증하지 못했다. 컴포넌트 부착과 첫 위치 변경은 확인했지만, 에디터에서 직접 플레이하며 타이밍 체감 검사가 필요하다.

## 세션 종료 시 갱신 규칙

1. **지금 어디까지** 갱신
2. **핵심 설계 결정** 추가
3. 완료한 **남은 작업** 체크
4. 다음 세션이 볼 **알려진 미해결** 갱신

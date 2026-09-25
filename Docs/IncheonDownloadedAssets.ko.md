# 다운로드 에셋 확인 및 실습 준비 상태

확인일: 2026-09-25. 원본 폴더: `C:/Users/JUNGLE/Downloads/Week4 프로토타입 에셋/에셋`.

후속 갱신: 사용자가 맵·배치 우선 순서를 요청한 시점의 저장된 씬에서 Main Camera의 CinemachineBrain, CM_Opening, IncheonSceneProfile 연결을 확인했다. 아래의 미연결 항목은 이전 검사 기록이다. 현재 진행 순서는 `Docs/IncheonMapLayoutStepByStep.ko.md`가 우선한다.

## 확인 결과

| 폴더/파일 | 직접 확인한 내용 | 다음 사용 범위 |
| --- | --- | --- |
| Universal Animation Library[Standard] | Unity/UAL1_Standard.fbx에 AnimationStack 43개. Swim_Fwd_Loop, Swim_Idle_Loop, Walk_Loop, Idle_Loop, Fixing_Kneeling, Interact, Death01, Hit_Chest 등 포함 | 수영·걷기·서 있는 대기 보충을 우선 확인. README에 따르면 `_RM` 없는 파일은 루트 이동 비활성 버전 |
| Universal Animation Library 2[Standard] | Unity/UAL2_Standard.fbx에 AnimationStack 43개. Hit_Knockback, LayToIdle, Walk_Carry_Loop 등 | 이번 필수 동작이 해결되지 않을 때 추가 검토. Female Mannequin은 README상 애니메이션 미포함 |
| kenney_watercraft-pack | FBX·OBJ·GLB, colormap.png, 각 배의 미리보기 포함. 전체 Preview.png 확인 | Unity용 FBX와 색상 텍스처만 먼저 선택 가능. 열린 갑판·탑승 높이·입수 동선을 실제 배치에서 확인. 역사적 상륙정으로 확정하지 않음 |
| kenney_particle-pack | 투명/검정 배경 PNG와 particlePack_samples.unitypackage 포함. 압축 목록에서 Fire·Smoke·Sparks·Magic·Hearts·Electricity 프리팹 6개 확인 | 이전 계획의 ‘완성 프리팹 없음’을 정정. 다만 URP 17 호환은 미확인. 물보라 전용 완성 효과는 이 목록에서 확인되지 않음 |
| sand_02_4k.blend | .blend와 별도 diff JPG, displacement PNG, normal/roughness EXR | 모래 표면용. 최초 배치에는 diff JPG만으로도 재질 후보. 해변 지형 자체가 완성되었다는 뜻은 아님 |
| coast_land_rocks_01_4k.blend | .blend와 별도 diff JPG, displacement PNG, normal/roughness EXR | 바위 섞인 해안 표면 후보. 산·해변 배치가 필요함 |
| BloodOverlay.png | 1280×800 RGBA, 알파 0~255 포함, 이미지 확인 | 정적 혈흔 원재료. 흐르는 빨간 물방울과 눈꺼풀 개폐는 별도 연결 필요 |
| 글꼴 | 제공 폴더 안에 .ttf/.otf 없음 | ‘40분 전.’을 표시할 한글 글꼴 또는 프로젝트의 사용 가능한 한글 폰트 확인 필요 |

Kenney 두 팩과 Quaternius 두 라이브러리는 다운로드 폴더의 License.txt에 CC0로 표기되어 있다. 위 확인은 로컬 파일·README·FBX/GLB 구조·패키지 목록·일부 미리보기 기준이며, Unity에 새로 임포트하거나 재생 테스트한 결과는 아니다. 외부 파일은 수정하지 않았다.

## 1~5번 진행 상황

사용자는 1~5번을 완료했다고 공유했고, 4번은 압축만 풀고 확인 전이라고 명시했다. 이에 따라 ‘사용자 완료 보고’와 ‘저장된 파일 확인’을 나누어 기록한다.

- `Assets/Scenes/Incheon_Practice.unity`와 `Assets/Settings/IncheonSceneProfile.asset`가 있다.
- `Assets/Samples/Shader Graph/17.0.4/Production Ready Shaders/` 아래 WaterLake, PostProcessUnderwater, PostProcessRainOnLens와 URP 예제 씬이 있다. 샘플 임포트는 파일로 확인된다.
- 저장된 Incheon_Practice의 Main Camera에는 FollowCamera가 활성화되어 있다. 캐릭터 Movement도 활성화 상태다.
- 저장된 Incheon_Practice의 Global Volume은 여전히 `SampleSceneProfile` GUID를 참조한다. 복제된 IncheonSceneProfile의 GUID와 다르다.
- 설치된 Cinemachine 3.1.7의 CinemachineBrain/CinemachineCamera 스크립트 GUID는 저장된 Incheon_Practice에서 발견되지 않는다.
- 따라서 1번의 Profile 재연결과 2번의 Cinemachine 연결을 저장된 씬 파일만으로 완료 처리할 수 없다. 에디터의 저장 전 상태는 확인하지 않았다. 기존 화면을 덮어쓰지 않도록 씬을 임의 수정하지 않았다.

Unity의 실제 작업 씬을 저장한 뒤 위 연결을 다시 확인하는 것이 다음 순서다. 연결이 이미 에디터에서 완료되어 있으면 저장으로 해결될 수 있다. 연결이 없다면 기존 단계별 안내의 1번 Profile 연결과 2번 Cinemachine 연결을 마무리한다.

## 다음 최소 작업

1. 실습 씬 저장 상태를 반영해 Cinemachine/Volume 연결 확인.
2. UAL1_Standard.fbx의 Swim_Fwd_Loop, Swim_Idle_Loop, Walk_Loop, Idle_Loop을 기존 캐릭터에 적용해 미리보기. 이름으로 적합 판정을 대신하지 않는다.
3. 기존 남은 동작과 합쳐 손/어깨 접촉·끄덕임에서 실제 부족한 부분만 확정.
4. 그 결과를 바탕으로 단계 6의 사건 재생·초기화 기반을 구성. 카메라 위치·Lens·Dutch·컷 길이는 사용자가 연습할 대상으로 남긴다.

이번 후속 작업에서 실제 수정한 Unity 에셋은 불필요 애니메이션 삭제와 Animator 두 개의 연결 보정이다. 사건 자동 재생, 눈꺼풀, 혈흔 흐름, 배 이동 기능을 완성한 것으로 간주하지 않는다.

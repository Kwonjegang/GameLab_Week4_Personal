# 인천 상륙 작전 — 단계별 실습 준비 안내

작성: 2026-09-25. Unity 6000.0.55f1 / URP 17.0.4 / Cinemachine 3.1.7 기준.

이 문서는 사용자가 앞으로 따라 할 안내다. 아래 씬 생성·에셋 다운로드·임포트·코드 연결을 이번 답변에서 실행한 것은 아니다.

## 현재 상태 갱신

이전 조사 이후 Cinemachine 3.1.7과 Post Processing 3.5.4가 추가되었다. manifest, lock, PackageCache에서 확인했다. Cinemachine 재설치 단계는 생략한다. 카메라가 씬에 연결되었는지는 설치 여부와 별개다.

현재 URP는 자체 Volume 후처리를 사용한다. 추가된 Post Processing 패키지의 Post-process Layer/Volume을 이번 카메라에 붙이지 않는다. 패키지 제거는 이 실습의 전제 조건이 아니다. [Unity 6 URP 공식 설명](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/integration-with-post-processing.html)

## 1. 실습용 씬과 Volume Profile을 복제한다

1. Play 모드를 종료한다.
2. 기존 Camera_Basics 씬을 열고 File → Save As로 다른 이름으로 저장한다.
3. 저장 위치 제안: C:/KSW/GameLab_Week4_Personal/Assets/Scenes/Incheon_CameraPractice.unity
4. Project 창에서 SampleSceneProfile을 검색한다. 해당 에셋을 선택하고 Ctrl+D로 복제하여 IncheonSceneProfile로 이름을 바꾼다.
5. 실습 씬의 Global Volume을 선택하고 Profile 칸에 복제한 IncheonSceneProfile을 연결한다.
6. 씬을 저장한다.

씬 복제만으로는 공유하는 Profile까지 복제되지 않는다. 복제한 프로필의 비네트 등을 바꿔야 기존 씬의 효과 설정을 함께 바꾸지 않을 수 있다.

완료 기준: 씬 이름이 Incheon_CameraPractice이며 Global Volume은 IncheonSceneProfile을 참조한다.

## 2. 시네마신 카메라 한 대를 연결한다

1. 실습 씬의 Main Camera를 선택한다.
2. FollowCamera 컴포넌트 왼쪽 체크를 꺼서 비활성화한다. Camera나 Main Camera 오브젝트 자체를 끄는 것은 아니다.
3. 위치를 고정해서 구도를 볼 동안에는 캐릭터의 Movement 컴포넌트도 비활성화한다. 변경은 실습 씬에서만 한다.
4. GameObject → Cinemachine → Cinemachine Camera를 선택한다.
5. 생성한 오브젝트를 CM_Opening으로 이름 짓는다. 첫 카메라는 대상을 자동 추적하는 동작을 추가하지 않은 기본 상태로 둔다.
6. Main Camera에 Cinemachine Brain이 있는지 확인한다. 첫 Cinemachine Camera 생성 시 보통 자동으로 추가된다. 없다면 Main Camera의 Add Component에서 Cinemachine Brain을 추가한다.
7. CM_Opening의 Transform 위치/회전과 Lens의 FOV를 바꾸며 Game 창을 확인한다. 더치 앵글은 Lens의 Dutch로 비교한다.
8. 이 기본 구성에서는 화면을 출력하는 일반 Camera는 Main Camera 한 대를 사용한다. CM_Opening은 샷을 정하는 Cinemachine 오브젝트다.

완료 기준: CM_Opening을 움직였을 때 Game 화면의 구도가 바뀐다. Play 전 편집 상태에서 시작하면 값이 사라지는 혼동을 줄일 수 있다.

근거: [기본 Cinemachine 연결](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/setup-cinemachine-environment.html), [Lens/Dutch 항목](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineCamera.html).

## 3. 보유 애니메이션을 미리 본다

Project 창에서 아래 이름을 검색하고 FBX를 선택한다. Inspector의 Animation 탭과 하단 Preview에서 재생한다. 필요한 경우 FBX를 펼쳐 내부 Animation Clip을 선택하고 Preview를 펼친다.

| 검색할 이름 | 사용할 후보 |
| --- | --- |
| Walk, Run | 접근과 달리기 |
| Crouch_Idle | 주인공의 움츠림 |
| Sitdown_Sankaku, Search_Ground | 동료가 몸을 낮추는 행동 |
| Use_Forward | 손 뻗기 |
| Talking_Yes | 고개 끄덕임 |
| Jump_All | 배에서 내려뛰기 |
| DamageM, Down-Ten-Long | 피격과 넘어짐 |

이름만 보고 채택하지 않는다. '쓸 수 있음 / 어색함 / 필요한 동작 없음' 정도로 기록한다. 어깨를 흔드는 정확한 동작과 수영이 부족한지 확인하는 것이 목적이다.

완료 기준: 접근·몸 낮추기·끄덕임·피격·넘어짐에 쓸 후보가 정해진다. Preview가 비어 있거나 Avatar가 맞지 않는 경우는 동작 부재가 아니라 임포트 확인 항목으로 남긴다.

## 4. 외부 에셋은 필요한 묶음부터 확보한다

첫 준비에서는 다운로드와 압축 해제까지 해도 된다. ZIP은 Assets 밖의 별도 다운로드 폴더에 모으고, 실제 Unity 임포트 때 모델·관련 텍스처·라이선스 파일을 함께 가져온다. 받은 폴더 전체를 무조건 프로젝트에 넣을 필요는 없다.

| 시점 | 링크 | 준비할 것 | 완료 기준 |
| --- | --- | --- | --- |
| 배 구간 준비 전 | [Kenney Watercraft Kit](https://kenney.nl/assets/watercraft-kit) | Download → 무료 다운로드 선택(기부 창이 나오면 Continue without donating). ZIP을 풀고 사람이 설 공간이 있는 배 후보를 찾는다. | 배 후보 하나와 텍스처가 준비됨 |
| 기존 수영 동작이 부족할 때 | [Quaternius Animation Library](https://quaternius.itch.io/universal-animation-library) | Download Now → No thanks, just take me to the downloads → Standard 파일. | 무료판 클립 목록을 확인함 |
| 4번 자막 준비 전 | [Noto Sans KR](https://fonts.google.com/noto/specimen/Noto+Sans+KR) | 글꼴 다운로드 후 TTF/OTF 파일 준비. | '40분 전.'에 쓸 글꼴 확보 |
| 해변 외형을 다듬을 때 | [Coast Sand 01](https://polyhaven.com/a/coast_sand_01) | 처음은 낮은 해상도(예: 1K) 컬러 텍스처부터. | 바닥에 쓸 표면 재료 확보 |
| 피 표현의 대체 재료가 필요할 때 | [Blood overlay](https://opengameart.org/content/blood-overlay) | Files의 BloodOverlay.png. | 정적 혈흔 이미지 확보 |
| 물보라·먼지 등 추가 시 | [Kenney Particle Pack](https://kenney.nl/assets/particle-pack) | 무료 이미지 묶음. | 입자 효과에 쓸 모양 확보 |

Quaternius 전체 소개에 수영이 있지만 무료 Standard의 정확한 클립 포함은 아직 미확인이다. 유료 Pro/Source를 먼저 구매하지 않고, Standard에서 부족한 경우 [Mixamo](https://www.mixamo.com/)를 다음 후보로 확인한다. Mixamo는 Adobe ID가 필요하다. [공식 FAQ](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html)

Kenney의 배가 실제 상륙정과 같다는 뜻은 아니다. 임시 외형을 허용한 현재 목표에서는 탑승 공간과 입수 구도가 맞는지가 기준이다. 혈흔 PNG도 움직이는 피가 완성된 에셋은 아니다.

완료 기준: 필요한 파일이 모이고, 아직 없는 것은 '수영 클립 / 손동작 보정'처럼 이름으로 특정된다.

## 5. 설치된 물·수중 샘플을 가져온다

1. Window → Package Management → Package Manager를 연다.
2. Unity Registry에서 Shader Graph를 검색하고 현재 설치된 17.0.4를 선택한다. In Project에서 보이면 그쪽에서 선택해도 된다.
3. Samples에서 Production Ready Shaders의 Import를 선택한다.
4. 임포트 완료 후 Project에서 WaterLake, PostProcessUnderwater, PostProcessRainOnLens를 검색한다.
5. 실습 씬을 저장한 다음 샘플을 보려면 URPProductionReadyShaders 씬을 연다. HDRP용 샘플 씬과 구분한다.
6. 샘플을 살핀 뒤 실습 씬으로 돌아온다. 원본 샘플 설정을 바꾸며 수정 저장하기보다 사용할 재료를 따로 구성한다.

샘플과 Common 의존성이 있으므로 Library 파일 하나만 직접 복사하는 방법보다 Samples 임포트를 우선한다. Import는 프로젝트로 재료를 가져오는 단계다. 실습 카메라에 수중·물방울 효과가 자동 연결되지는 않는다.

완료 기준: 세 가지 셰이더를 Assets에서 찾을 수 있다.

근거: [Unity의 샘플 임포트 안내](https://unity.com/blog/engine-platform/new-shader-graph-production-ready-shaders-in-unity-6), [수면](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Shader-Graph-Sample-Production-Ready-Water.html), [수중/렌즈 물방울](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Shader-Graph-Sample-Production-Ready-Post.html).

## 6. 사건을 반복 재생할 기반을 보조 구현으로 연결한다

이 단계는 코딩·리깅·렌더링 연결의 지원 작업이다. 앞으로 이 단계의 구현을 진행할 때 제가 맡을 수 있는 범위는 다음과 같다.

- 동료가 접근해서 몸을 낮추고 손을 뻗는 동작 연결.
- 어깨 접촉 위치와 기존 애니메이션의 어긋남 보정.
- 같은 시작 위치로 복원하고 1~3번만 반복 재생하는 기능.
- 눈꺼풀과 피의 흐름을 조절할 값 노출.
- 뒤의 구간을 위한 배 이동, 입수, 수중/수면 위 전환, 점등, 피격 동작 연결.
- 한글 자막, 재질 호환성, 애니메이션 리타기팅 등 에셋 연결.

사용자는 동작이 의도에 맞는지 확인하고 필요한 화면을 판단한다. 셰이더 작성이나 애니메이션 제어 코드 구현을 직접 익혀야만 카메라 연습을 시작하는 구조로 만들지 않는다.

완료 기준: 카메라를 아직 다듬지 않아도 동료와 사건이 같은 순서·위치에서 반복된다.

## 7. 먼저 1~3번을 카메라로 연습한다

1. CM_Opening을 낮게 놓고 지면 기준 약 30° 위를 보게 잡는다. Transform의 각도 부호는 실제 화면으로 확인한다.
2. Pitch는 그대로 두고 Dutch를 0°/10°/20°로 바꾸며 느낌을 비교한다.
3. 다가오는 동료를 따라 작은 Tilt를 준 뒤 2번 시작에서 멈춘다.
4. 얼굴과 손이 프레임 안에 들어오는지 확인한다.
5. 눈 깜박임 두 번의 속도와 간격을 바꾼다.
6. 붉은 색감·피·눈꺼풀이 닫히는 타이밍을 비교한다.
7. 눈이 완전히 닫혔을 때 피까지 가려지는지 확인한다.

Volume은 색감과 효과 강도, 눈꺼풀은 최종 가림으로 구분한다. 한번에 하나의 변수만 바꿔 같은 동작을 다시 본다.

완료 기준: '쓰러져 있다 → 동료가 깨운다 → 의식이 흐려진다'가 화면으로 전달된다.

## 8. 4~5번의 해안과 상륙을 붙인다

1. 단순 지형으로 바다·해변·산의 위치를 잡고 배를 놓는다.
2. 수면과 수중 샘플을 지원 작업으로 연결한다.
3. CM_Establish라는 ELS용 카메라를 만들어 배의 진행 방향과 해안 관계를 잡는다.
4. 배 위 POV의 도착 위치와 회전을 정한다.
5. ELS에서 배 위로 이동하는 경로와 속도를 비교한다. 두 카메라를 블렌드하는 것만으로 장애물을 피하는 경로가 보장되지는 않는다.
6. 동료 입수 → 주인공 입수 → 수중 이동 → 부상 → 물가 도착을 연결한다.
7. 둘러보기와 동료의 끄덕임을 조절한다.
8. '40분 전.' 자막을 연결한다.

완료 기준: 카메라가 배나 인물을 관통하지 않고, 물속/물 밖 구분과 이동 방향이 명확하다.

## 9. 6~7번과 전체 순서를 연결한다

1. 주인공 POV에서 앞선 동료의 보행을 따라간다.
2. 점등 → 첫 피격 → 주변 동료의 피격 → 주인공 움츠림 → 질주 순서를 반복 재생한다.
3. 첫 피격을 관객이 볼 시간과 움츠린 뒤 다시 전진하기까지의 시간을 조절한다.
4. 여러 카메라의 순서는 Timeline의 Cinemachine Track으로 연결해 비교한다.

Timeline 연결의 기본: 시퀀스용 빈 오브젝트 선택 → Window → Sequencing → Timeline에서 Create → Main Camera를 Timeline에 끌어 Create Cinemachine Track → 만든 Cinemachine Camera들을 그 트랙에 배치. 붙어 있는 Shot Clip은 컷, 겹치는 구간은 블렌드가 된다. 기존 Timeline이 있으면 새로 중복 생성하지 않고 그 안에 트랙을 추가한다. [공식 연결 안내](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/setup-timeline.html)

이 트랙은 어떤 카메라를 사용할지 정한다. 배우 동작과 이동, 카메라 자체의 Tilt/이동, 화면 효과의 시간 변화는 해당 트랙/키프레임/보조 연결이 따로 필요하다.

완료 기준: 별도 입력 없이 1~7번이 재생되고 원하는 구간을 반복 비교할 수 있다.

## 10. 마지막에 외형과 소리를 보완한다

카메라로 사건이 읽히면 모래 재질, 물보라, 먼지, 색감을 순서대로 다듬는다. 선택 음원 후보:

- [입수·기포·물소리](https://opengameart.org/content/40-cc0-water-splash-slime-sfx)
- [총성 후보](https://opengameart.org/content/residue-sfx)
- [파도](https://opengameart.org/content/beach-ocean-waves)

각 음원은 다운로드 전후 미리 들어 장면에 맞는지 판단한다. 군복 교체, 실제 상륙정 외형, 대규모 군중은 이 단계 이후 필요에 따라 검토한다.

첫 준비 구간은 1~5단계다. 이후 6단계에서 재생 기반을 연결하고 7단계의 첫 장면 실습을 시작하는 순서다.


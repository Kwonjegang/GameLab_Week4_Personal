# 인천 상륙 작전 — 에셋 확보 계획 v0.1

조사일: 2026-09-25 / 대상: 스토리보드 1~7번

최신 후속 결과: [애니메이션 평가·삭제·Animator 보정](IncheonAnimationReview.ko.md), [다운로드 에셋·실습 씬 확인](IncheonDownloadedAssets.ko.md). 사용자가 제외한 동작 65개는 프로젝트에서 삭제되었고 애니메이션 FBX는 현재 27개다. 아래 초기 조사의 92개 및 미다운로드/미임포트 표기는 당시 상태이며 최신 확보 상태는 후속 문서가 우선한다.

사용자 선택: **기존·무료 우선, 외형은 임시여도 괜찮음.**

후속 확인: 단계별 안내 작성 시 Cinemachine 3.1.7과 Post Processing 3.5.4가 manifest/lock/PackageCache에 추가된 것을 확인했다. 아래의 초기 조사 상태와 구분한다. 현재 단계별 진행은 Docs/IncheonLandingStepByStep.ko.md를 따른다. 후처리는 URP 내장 Volume 기준이다.

이 문서는 필요한 재료를 한 번에 추리는 목록이다. 구매·다운로드·임포트·제작을 실행한 기록이 아니다. 파일 존재와 배포 페이지 설명을 확인했으며, Unity 재생이나 외부 파일의 임포트 테스트는 하지 않았다.

## 1. 저장된 프로젝트에서 확인한 기반

| 항목 | 파일로 확인한 상태 | 이번 기획에서의 의미 |
| --- | --- | --- |
| Unity / 렌더링 | Unity 6000.0.55f1, URP 17.0.4. PC 품질 설정은 PC_RPAsset → PC_Renderer 참조. | URP 기준 후보 선정 |
| 카메라 | Camera_Basics의 Main Camera에 FollowCamera 활성. followTarget은 첫 UnitychanRFN_STD 루트, lookTarget은 그 자식 CameraTarget. | 기본 추적 연결 있음. Cinemachine과 동일 Transform을 동시에 제어하지 않도록 후속 구성 필요 |
| 시선 기준점 | CameraTarget 로컬 위치 (0, 1.4, 0) | 추적 기준점으로 재사용 후보 |
| 후처리 | Main Camera Post Processing 활성, Global Volume → SampleSceneProfile. Bloom·Vignette·Tonemapping 활성, MotionBlur 비활성. | 후처리 연결 기반 있음. 화면 결과 미확인 |
| 렌더링 입력 | PC_RPAsset의 Depth/Opaque Texture 활성 | 일부 물/화면 효과를 위한 기반 있음 |
| 캐릭터 | UnitychanRFN_STD 모델 및 같은 모델의 씬 인스턴스 2개 | 모델 재사용·복제로 인원 구성 |
| 애니메이션 | Character/Animations에 애니메이션용 FBX 92개. 모델과 점검한 주요 동작은 Humanoid 설정. | 보유 파일에서 동작을 먼저 확인. 92개 검증된 동작이라는 뜻은 아님 |
| Movement | 입력을 읽는 이동 스크립트 | 이번 자동 재생 기반과 역할 구분 필요 |
| Cinemachine | 최초 조사에는 없었으나 후속 확인에서 3.1.7 설치 확인 | 재설치는 생략하고 씬의 Brain/Camera 연결을 확인 |
| Timeline / DOTween | Timeline 1.8.7 선언 및 DOTween 파일 있음 | 보유 도구로 사건 재생을 구성할 후보. 씬 연출 연결은 미확인 |

디스크의 Assets/Scenes/Camera_Basics.unity 등을 검사했다. 현재 에디터에서 열려 있는 씬이나 저장 전 변경을 확인한 결과는 아니다.

## 2. 필요한 재료 전체 목록

| ID | 필요한 재료 / 최소 구성 | 장면 | 1차 확보 방향 | 현재 상태 |
| --- | --- | --- | --- | --- |
| A01 | 사람 모델 1종, 주인공+동료 4명 배치안 | 전체 | 기존 UnitychanRFN_STD | 파일 있음, 복제·배치 전 |
| A02 | 대기·걷기·달리기·웅크리기 | 1,5,6,7 | 기존 Idle/Walk/Run/Crouch | 파일 있음, 동작 선택 필요 |
| A03 | 몸 낮추기·손 뻗기·어깨 흔들기 | 2 | Sitdown/Seiza/Search_Ground/Use_Forward 확인 | 정확한 깨우기 동작 미확보; 조합·보정 후보 |
| A04 | 고개 끄덕임 | 5 | Talking_Yes | 파일 있음, 동작 확인 필요 |
| A05 | 배에서 내려뛰기 | 5 | Jump_All/Jump_InAir와 경로 | 파일 있음, 입수 적합성 미검증 |
| A06 | 수중 동료의 이동/수영 | 5 | 무료 애니메이션 후보 | 기존 수영 파일을 찾지 못함 |
| A07 | 피격·넘어짐·누운 유지 | 7 | Damage/Down 계열 | 파일 있음, 연결과 마지막 자세 확인 필요 |
| A08 | 사람이 서고 입수할 공간이 있는 배 1척 | 4,5 | Kenney Watercraft Kit 우선 | 외부 후보 있음, 미다운로드 |
| A09 | 해변·해안·산의 연결된 지형 | 4~7 | 기본 Terrain/단순 메시 배치안, 무료 모래 재질 | 해당 지형 없음, 배치 작업 필요 |
| A10 | 하늘·기본 조명 | 1,4~7 | 현재 씬 기본 Skybox/Directional Light 기반 | 기반 있음 |
| A11 | 수면 메시·재질 | 4,5 | 설치된 WaterLake 샘플 | PackageCache에 있음, Assets 미임포트 |
| A12 | 수중 화면 효과 | 5 | 설치된 PostProcessUnderwater | PackageCache에 있음, 미연결 |
| A13 | 위에서 흐르는 붉은 방울 | 3 | Rain On Lens 수정안 / 무료 혈흔 이미지 | 원재료 후보 있음, 원하는 피 흐름은 미구현 |
| A14 | 눈 깜박임과 최종 검은 눈꺼풀 | 1,3 | 화면 가림과 개폐 애니메이션 | 전용 연결 필요 |
| A15 | 붉은 부상 색감·어두워짐 | 1,3 | 기존 URP Volume 기반 | 구간별 프로필 설정 필요 |
| A16 | '40분 전.' 한글 표시 | 4 | Noto Sans KR + 텍스트 | 글꼴 후보 있음, 전용 자막 없음 |
| A17 | 물보라·기포·피격 먼지/섬광 | 5,7 | Kenney Particle Pack + Particle System | 완성 프리팹 없음, 조립 필요 |
| A18 | 사건을 알리는 점등/발광 | 7 | 기본 Light/발광 재질 | 광원 종류 미정, 전용 모델은 필수 아님 |
| A19 | 파도·입수·총성 | 선택 | 무료 효과음 후보 | 기존 음원 파일을 찾지 못함 |
| A20 | 구간 재생·초기화·배와 인물의 자동 동작 | 전체 | Timeline 등 보유 도구 활용 | 에셋 구매보다 지원 기능 연결이 필요한 항목 |

총·적 병사·탱크·대규모 군중·완전한 1인칭 팔 세트는 현재 화면 요구를 확정한 뒤 추가 여부를 판단한다. 화면 밖 발사, 점등, 반응 애니메이션으로 피격을 먼저 표현할 수 있다. 사용자의 7개 장면을 삭제하거나 축소한다는 뜻은 아니다.

## 3. 기존 동작 확인 목록 — 사용자 평가로 대체

파일명만으로 선정했던 이전 후보는 `Docs/IncheonAnimationReview.ko.md`의 직접 평가와 장면별 후보 표로 대체한다. 특히 기존 Idle/Walk/Walk2/Run/Seiza/Search_Ground/Use_Forward/Jump_All/Jump_InAir/DamageS 등은 사용자가 제외하고 삭제를 요청했으므로 다시 필수 후보로 제안하지 않는다. Run2, Crouch_Idle, DoubleJump, Down-Chi-Long, Down-Ten-Long 등을 유지한다. Talking_Yes와 DamageM은 확정품이 아니라 ‘애매하다’에 속한다.

## 4. 외부 후보와 우선순위

가격·배포 정보는 조사 시점 페이지 표시다. 라이선스도 배포 페이지의 표기를 기록한다. 무료판과 전체 팩의 내용, 다운로드 가능성과 임포트 성공은 구분한다.

| 순위 | 후보 / 출처 | 용도·비용·표시 라이선스 | 판단과 미확인 사항 |
| --- | --- | --- | --- |
| 1 | [Unity Water 샘플](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Shader-Graph-Sample-Production-Ready-Water.html) | 설치된 17.0.4의 수면 샘플. 별도 구매 없음. 로컬 LICENSE.md는 Unity Companion License. | WaterLake로 임시 바다 표현. 완성된 해양/수영 시스템이 아니며 물 아래 표시·경계·배 내부 확인 필요 |
| 1 | [Unity Post-Process 샘플](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Shader-Graph-Sample-Production-Ready-Post.html) | 설치된 Underwater/Rain On Lens | 수중과 흐르는 방울의 기반. 붉은 피·상단 시작·눈꺼풀 가림은 수정/연결 필요 |
| 1 | [Kenney Watercraft Kit](https://kenney.nl/assets/watercraft-kit) | 무료·CC0. 페이지상 45개 파일 | 임시 배 첫 후보. 갑판/탑승 공간 확인 필요. 역사적 상륙정 포함은 미확인 |
| 1 | [Quaternius Universal Animation Library](https://quaternius.itch.io/universal-animation-library) | Standard 무료·CC0 | 전체 소개에 수영·앉기·죽음 포함. 무료 Standard의 정확한 수영/깨우기 클립 포함 여부는 파일 확인 전. Pro/Source와 구분 |
| 1 | [Poly Haven Coast Sand 01](https://polyhaven.com/a/coast_sand_01) | 해변 재질. 무료·CC0 | 지형 모델이 아니라 표면 재료. 최초 배치는 단색으로도 가능 |
| 1 | [Noto Sans KR](https://github.com/google/fonts/tree/main/ofl/notosanskr) | 한글 글꼴. 무료, [OFL](https://github.com/google/fonts/blob/main/ofl/notosanskr/OFL.txt) | Unity에서 사용할 글꼴 파일과 한글 글리프 설정 확인 필요 |
| 2 | [Blood overlay — 1up Indie](https://opengameart.org/content/blood-overlay) | 무료·CC0, 1280×800 혈흔 이미지 | 정적인 원재료. 위에서 흐르는 애니메이션/Volume 효과가 완성된 에셋이 아님. 16:9 배치 조정 |
| 2 | [Kenney Particle Pack](https://kenney.nl/assets/particle-pack) | 무료·CC0, 512×512 이미지 80개 | 물보라·먼지·섬광을 만들 원재료 후보. 완성 URP 프리팹 포함으로 가정하지 않음 |
| 대체 | [Mixamo 공식 FAQ](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html) / [서비스](https://www.mixamo.com/) | Adobe ID로 무료 사용, 자체 이용 조건 | Standard에서 부족한 동작이 남을 때 탐색. 이번에는 로그인·업로드·개별 클립 검증 안 함 |
| 대체 | [LCVP Higgins Boat Opened — Thomas Visonà](https://sketchfab.com/3d-models/lcvp--higgins-boat-opened-d112cbc8f65140b7a9118bb39d4db8e5) | 검색 결과상 무료·CC Attribution·약 31.7k 삼각형 | 상세 페이지는 403으로 접근 실패. 실제 다운로드·포맷·라이선스/저작자 표기 재확인 필요. 확정 확보품 아님 |
| 후순위 | [Quaternius Ultimate Modular Men Pack](https://quaternius.com/packs/ultimatemodularcharacters.html) | 무료·CC0, 페이지상 11 캐릭터/24 애니메이션 | 현재 모델이 있어 외형 교체는 후순위. 시대에 맞는 병사 외형은 미확인 |

Poly Haven의 CC0 방침은 [공식 FAQ](https://docs.polyhaven.com/en/faq), Quaternius 무료/유료 구분은 [배포 목록](https://quaternius.itch.io/universal-animation-library/purchase)에서 확인했다.

[Boat Attack Water](https://github.com/Unity-Technologies/boat-attack-water)도 찾았지만 저장소는 지속 개발 중이며 공식 지원하지 않는다고 명시한다. 현재 설치 버전의 샘플이 있으므로 초기 도입 우선순위는 낮췄다.

## 5. 이미 설치된 물과 화면 효과의 위치

아래는 PackageCache의 파일이다. 이번에는 Assets로 복사하거나 수정하지 않았다. 후속 단계에서 Package Manager의 Samples를 통해 필요한 샘플과 의존성을 가져오는 방법부터 검토한다. Library 원본을 직접 수정하지 않는다.

- Library/PackageCache/com.unity.shadergraph@58c763b6444e/Samples~/ProductionReady/Environment/Water/WaterLake.shadergraph
- Library/PackageCache/com.unity.shadergraph@58c763b6444e/Samples~/ProductionReady/Environment/Water/WaterSimple_FoamMask.shadergraph
- Library/PackageCache/com.unity.shadergraph@58c763b6444e/Samples~/ProductionReady/PostProcess/PostProcessUnderwater.shadergraph
- Library/PackageCache/com.unity.shadergraph@58c763b6444e/Samples~/ProductionReady/PostProcess/PostProcessRainOnLens.shadergraph

package.json에는 Common 샘플 의존성도 선언되어 있다. 단일 shadergraph만 복사하면 관련 재료가 빠질 수 있다.

Assets/Floreswa/Materials/Sea.mat는 이름은 Sea지만 실제로는 일반 URP/Lit, 불투명 표면, 단색 재질이다. 이를 완성된 물 셰이더나 수중 시스템으로 세지 않았다. Floreswa의 물고기는 이번 필수 목록에 넣지 않았다.

## 6. 선택 효과음

화면과 카메라 재생 확인 후 붙일 선택 재료다. 직접 들어 음색을 검증하지 않았다.

| 용도 | 후보 | 확인 내용 |
| --- | --- | --- |
| 입수·기포·물소리 | [40 CC0 water / splash / slime SFX — rubberduck](https://opengameart.org/content/40-cc0-water-splash-slime-sfx) | 제작자 페이지에서 CC0와 splash/bubble/물 루프 포함 확인 |
| 총성 | [Residue-sfx — FacadeGaikan](https://opengameart.org/content/residue-sfx) | 제작자 페이지에서 CC0, gun/explosion 확인. 사실적인 전쟁 음색인지 미청취 |
| 파도 | [Beach Ocean Waves — jasinski, qubodup 게시](https://opengameart.org/content/beach-ocean-waves) | 게시 페이지에 CC0와 원 녹음 링크. FLAC은 필요 시 지원 형식으로 변환 |

배 엔진·발소리·숨소리·이명·음악·대사는 선택 확장 목록으로 남긴다. 이번 카메라 기능을 확인하기 위한 필수 전제는 아니다.

## 7. 별도 제작·보정 후보

현재 확정한 신규 모델 제작은 없다. 아래도 기존 재료의 적합성을 먼저 확인한 후 필요한 부분만 작업한다.

| 항목 | 먼저 확인 | 부족할 때만 할 일 |
| --- | --- | --- |
| 어깨를 흔들어 깨우기 | 몸 낮추기+손 뻗기 후보를 POV에서 재생 | 짧은 손/팔 포즈 보정 또는 IK |
| 위에서 흐르는 피 | Rain On Lens 드립, 무료 혈흔 이미지 | 붉은 색·마스크·흐름 위치 수정 |
| 눈꺼풀 개폐 | 기본 가림 도형/화면 마스크 | 열림 값으로 제어할 효과와 키프레임 |
| 해안과 산 윤곽 | Terrain/단순 메시를 ELS와 POV에서 비교 | 필요한 지형만 배치 |
| 배 내부 | 무료 배 후보의 탑승·입수 공간 | 적합한 무료 후보가 없을 때만 단순 배 블록아웃 |
| 반복 재생 기반 | 기존 동작과 Timeline 조합 | 초기화·구간 재생·효과 조절값 연결 |

이미지 재료를 구하는 것과 시간에 따라 움직이는 기능은 다르다. 혈흔 이미지를 구해도 흐름 타이밍은 연결해야 한다. Volume Weight를 이미지 알파나 레이어 순서로 취급하지 않는다.

## 8. 확보 순서 제안

1. 기존 캐릭터 동작을 재생해 재사용 가능 범위를 결정한다.
2. 설치된 수면·수중·물방울 샘플을 먼저 시험 대상으로 삼는다.
3. 무료 배 1척, 필요한 경우 모래 텍스처 1종, 한글 글꼴 1종을 고른다.
4. 실제 부족한 수영/깨우기 동작과 화면 효과만 보정 후보로 남긴다.
5. 재료가 정해지면 별도 연습 씬에서 사건의 반복 재생 기반을 준비하고, 카메라는 사용자의 실습 대상으로 둔다.

이번 산출물은 이 확보표와 Docs/IncheonLandingStoryboard.ko.md다. 다운로드, 구매, 샘플 임포트, 패키지 설치, 새 모델/효과 제작, 씬/코드 변경은 하지 않았다.


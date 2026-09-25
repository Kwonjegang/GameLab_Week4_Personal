# 카메라 학습 기록과 지원 기준

기록일: 2026-09-25 (한국 시간)

이 문서는 사용자가 대화에 직접 제공한 학습 노트와 프로젝트 목표를 정리한 기록이다. 원문 전체의 전사본은 아니다. 학습한 내용, 자료를 바탕으로 한 수준 판단, 보완 설명을 구분한다. 정리된 용어를 실제로 모두 구현하거나 숙달했다고 가정하지 않는다.

## 1. 학습자와 목표

- 크래프톤 정글 게임랩에서 게임 개발을 공부하는 기획자다.
- 사용자는 자신의 코딩 구현 능력이 낮다고 설명했다. 카메라 추적, 회전 오프셋, LookAt 예제를 설명 난이도의 기준으로 제시했다.
- 이번 주의 중심 과제는 카메라다. 특히 Cinemachine과 포스트 프로세싱을 직접 실습하고 싶어 한다.
- 프로토타입은 '카메라 연출만을 이용한 인천 상륙 작전'이다.
- 별도 사용자 조작 없이 주인공과 동료가 바다에서 앞으로 달려간다. 포탄이 날아오고, 피격으로 넘어지고, 동료가 주인공을 지키려다 쓰러지고, 주인공의 과거 회상으로 이어진다.
- 풀숏, POV, 익스트림 롱숏 등을 선택하고 연결하면서 사건의 규모, 긴박함, 인물의 감정을 표현하려 한다.
- 지원 목표는 이동·사건 진행·반복 재생 등의 기반을 마련하여 사용자가 카메라 연출 학습에 집중하도록 돕는 것이다. 구체적인 구현은 해당 작업 요청과 당시 프로젝트 상태를 기준으로 진행한다.

## 2. 현재 수준 판단

현재 단계는 **카메라 연출의 기초 개념을 폭넓게 학습했고, 이를 Unity에서 재현하는 구현은 입문 단계**로 판단한다. 이는 제출한 자료에 대한 평가이며 실기 검증 결과는 아니다.

| 영역 | 제공 자료에서 확인되는 내용 | 지원이 필요한 부분 또는 미확인 사항 |
| --- | --- | --- |
| 숏과 연출 언어 | 숏의 크기·앵글·움직임·시점을 구분하고 감정 및 공간 전달과 연결해서 설명한다. | 실제 장면에 적용한 구도, 컷의 길이, 숏 연결의 완성도는 아직 확인하지 않았다. |
| 렌즈와 공간감 | FOV와 거리의 조합을 비교하고, Perspective·Orthographic 및 심도를 학습했다. | FOV, 촬영 위치, 초점, 심도를 각각 다른 변수로 구분하는 보완이 필요하다. |
| 카메라 제어 | Follow와 LookAt의 역할, yaw 기반 오프셋, 축별 Damping 예제를 학습했다. | 독립적인 C# 작성·디버깅·씬 연결 능력은 확인되지 않았다. 복잡한 수학이나 구조 설계를 전제하지 않는다. |
| 추적과 구도 유지 | Damping·Dead Zone·Look Ahead의 목적을 자신의 말로 정리했다. | Dead Zone 구현은 미학습으로 명시했다. Look Ahead도 예제를 검토하며 배우는 단계다. |
| Cinemachine·후처리 | 집중해서 배우고 싶은 분야가 명확하다. | 카메라 연결, 전환, Volume 설정을 직접 완료한 증거는 아직 없다. |

단일한 초급/중급 등급보다 영역별 수준을 기준으로 설명한다. 용어를 처음부터 전부 반복하기보다 사용자가 이미 아는 연출 의도를 Unity의 설정 항목과 연결한다.

## 3. 사용자가 학습했다고 제시한 범위

### 숏 크기

- ECU: 눈·입 등 일부를 극단적으로 가까이 담아 긴장과 감정을 강조.
- CU / 헤드숏: 얼굴이나 특정 동작을 가까이 담아 미세한 감정을 전달.
- BS / MCU: 머리부터 가슴 부근을 담는 구도와 대사 전달.
- WS(사용자 노트에서는 waist shot): 머리부터 허리 부근, 두 인물과 배경 활용.
- KS: 머리부터 무릎 부근, 그룹 및 배경 활용.
- FS: 전신과 주변 여백의 의미.
- MS / MLS: 인물과 공간의 비중 조절.
- LS / ELS: 장소, 인물 관계, 공간의 규모와 설정 전달.

### 앵글·시점·촬영 방식

- 픽스숏, 버즈 아이 뷰, 하이앵글, 아이레벨, 로우앵글, 익스트림 로우앵글.
- 버티컬 앵글 / 오버헤드, 더치 앵글, 리버스 앵글, 오버 숄더.
- 스테디캠의 안정된 움직임, 핸드헬드의 흔들림, POV의 인물 시점.
- 각 방식이 외소함·나약함·위압감·불안·긴박함 등의 인상에 어떻게 쓰이는지 학습했다.

### 카메라 움직임

- Pan: 고정 위치에서 좌우 회전.
- Tilt: 고정 위치에서 위아래 회전.
- Pedestal: 카메라 자체의 상하 이동.
- Travelling, Crane, Aerial: 공간을 이동하거나 공중에서 촬영하는 방식.
- Zoom: 화각 변화.
- Dolly: 카메라 자체의 전후 이동과 그에 따른 몰입·긴장 표현.
- Tracking: 움직이는 인물을 따라가는 이동.
- Arc: 대상을 중심으로 돌아가는 이동. 배틀그라운드에서 Alt를 누른 채 시점을 돌리는 경험과 연결했다.

### 장면 구성과 구도

- 설정숏, 마스터숏, 커버리지숏의 역할.
- 원숏·투숏·쓰리숏·그룹숏·몹 씬의 인물 구성.
- 헤드룸, 노즈룸 / 루킹룸, 리드룸.
- 180도 법칙과 30도 법칙을 통한 공간 방향 및 컷 연결의 이해.
- 딥 포커스와 셸로우 포커스, 조리개 값과 심도의 기본 관계.

### Unity 렌즈 실험과 관심사

- 같은 화면상 캐릭터 크기를 유지하면서 FOV 30 + 먼 거리, FOV 60 + 중간 거리, FOV 90 + 가까운 거리를 비교했다는 노트를 제공했다.
- 사용자가 기록한 인상: 좁은 FOV와 먼 거리는 배경이 압축되고 얼굴이 완만하며 안정적이다. 넓은 FOV와 가까운 거리는 공간의 원근이 강조된다. 이는 사용자 관찰 기록이며 현재 씬에서 재검증한 결과는 아니다.
- Perspective, Orthographic, 낮은 FOV와 먼 거리로 만든 약한 원근 표현에 관심이 있다.
- Orthographic에서도 그림자, 조명, 높이, 겹침, 크기 연출, 색, 안개, 시차 같은 깊이 단서를 활용할 수 있다고 정리했다.
- 어안 표현에 관심이 있어 Cubemap과 셰이더 방식, 디스토션 플러그인 자료를 수집했다. 호환성·현재 가격·적합성은 이번 기록에서 검증하지 않았다.

## 4. 코딩 이해의 기준점

사용자 코드 블록의 언어 표시는 Java, Arduino 등으로 섞여 있으나 내용은 Unity C# 예제다. 다음 항목을 접하고 학습한 것으로 기록한다. 직접 처음부터 작성할 수 있다고 단정하지 않는다.

### 기본 Follow / LookAt

```csharp
public Transform followTarget;
public Transform lookTarget;
public Vector3 offset = new Vector3(0f, 2.5f, -5f);

void LateUpdate()
{
    Quaternion yawRotation =
        Quaternion.Euler(0f, followTarget.eulerAngles.y, 0f);

    Vector3 desiredPosition =
        followTarget.position + yawRotation * offset;

    transform.position = desiredPosition;
    transform.LookAt(lookTarget);
}
```

이는 클래스 내부에 들어가는 학습용 코드 조각이며 독립된 완성 스크립트가 아니다. followTarget과 lookTarget의 연결을 전제로 한다.

### Damping

- 사용자의 표현: '얼마나 빨리 반응할 것인가?', '플레이어의 움직임을 카메라에 얼마나 직접 전달할 것인가'.
- Mathf.SmoothDamp로 위치의 x/y/z를 계산하고, x/z에는 horizontalSmoothTime = 0.15f, y에는 verticalSmoothTime = 0.4f를 적용한 예제를 학습했다.
- 축마다 velocityX/Y/Z를 별도로 보관하는 예제를 제공했다.
- 점프에서는 수직 반응을 느리게, 방향 전환에서는 수평 반응을 느리게 하는 연출을 생각하고 있다.

### Dead Zone

- 사용자의 표현: '언제부터 반응할 것인가?', '특정 범위를 넘어가면 팔로우 시작'.
- 개념은 학습했으나 코드 구현은 앞으로 알아볼 항목이라고 명시했다.

### Look Ahead

- 사용자의 표현: '어디를 미리 보여줄 것인가?', '리드룸 자동화'.
- 이전 프레임 위치와 현재 위치의 차이를 Time.deltaTime으로 나눠 속도를 추정하는 예제를 제공했다.
- targetVelocity.normalized * 3f를 lookTarget.position에 더하여 미리 볼 지점을 만들려 한다.
- 정지·급격한 방향 전환·예측의 부드러움은 아직 검증되지 않았다.

## 5. 앞으로 설명할 때 함께 보완할 사항

아래는 사용자가 이미 숙달한 내용이 아니라 이번 기록에서 분리해 남긴 보완 메모다. 한 번에 모두 설명하거나 구현 과제로 부과하지 않는다.

1. **FOV와 촬영 위치를 구분한다.** 일반적인 Perspective 카메라에서 FOV는 화면에 담는 각도의 범위를 정한다. 같은 위치와 방향에서 FOV만 바꾸면 확대·축소되어 보이고, 피사체의 앞뒤 상대 크기를 결정하는 시점은 그대로다. 같은 피사체 크기를 유지하면서 거리를 함께 바꿀 때 원근 표현이 달라진다. 이 구분은 Unity의 [뷰 프러스텀 설명](https://docs.unity3d.com/cn/2018.3/Manual/UnderstandingFrustum.html)과 [돌리 줌 설명](https://docs.unity3d.com/cn/2018.3/Manual/DollyZoom.html)을 바탕으로 정리했다. 오래된 페이지는 투영 원리 참고용이며 코드/API 안내는 현재 설치 버전을 기준으로 한다.
2. **FOV와 초점 흐림을 구분한다.** FOV 변경 자체가 자동 초점이나 배경 흐림을 켜주지는 않는다. URP의 Depth of Field는 별도 Volume 효과이며 모드에 따라 조절 항목이 다르다. [Unity 6 URP Depth of Field](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/depth-of-field-volume-override-reference.html)
3. **LateUpdate와 부드러움을 구분한다.** LateUpdate는 Update 이후에 실행할 순서를 제공한다. 제공된 기본 코드의 위치 직접 대입과 LookAt에는 별도 감쇠가 없으며, SmoothDamp 예제에서도 위치만 부드럽게 바뀌고 회전은 LookAt으로 즉시 갱신된다. [Unity LateUpdate](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.LateUpdate.html)
4. **회전 중심과 바라보는 대상을 구분한다.** 제공 코드의 위치는 followTarget.position을 기준으로 정한다. lookTarget은 바라보는 방향을 정한다. 따라서 오프셋 회전의 기준을 항상 lookTarget이라고 설명하지 않는다. yaw만 사용하는 것은 오프셋 계산이며, LookAt에 의해 실제 카메라의 상하 각도는 바뀔 수 있다. 이 설명은 제공 코드 분석에 근거한다.
5. **Look Ahead 코드 조각은 아직 완성본이 아니다.** 제공문 그대로라면 targetVelocity는 LateUpdate 내부 지역 변수인데 이후 계산은 함수 밖에 있어 범위 및 실행 위치 수정이 필요하다. normalized * 3f는 속력 정보를 제거한 고정 거리 예측이다. 속력에 따라 얼마나 앞을 볼지 바꾸려면 속도와 예측 시간을 연결하는 방식 등을 별도로 배운다. Cinemachine의 Lookahead Time도 미래 위치를 예측하는 시간 개념으로 설명된다. [Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html)
6. **화면 기준과 월드 축 기준을 구분한다.** 사용자의 SmoothDamp 예제는 월드 x/y/z 위치를 다룬다. Cinemachine Position Composer의 감쇠는 카메라 기준 축을 다루며 Dead Zone은 화면상 구도 영역이다. '좌우'라는 설명이 어떤 좌표계를 뜻하는지 실제 컴포넌트를 보며 연결한다. [Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html)
7. **게임 사례와 구현 단정을 분리한다.** '하데스가 강한 Perspective·약한 Perspective·Orthographic을 유동적으로 섞는다'는 사용자 노트의 문장은 출처 미확인으로 남긴다. 해당 게임의 실제 구현 사실로 인용하지 않는다. 어안 지원 여부도 사용 파이프라인·도구·원하는 투영에 따라 확인하며 'Unity에서 절대 구현되지 않는다'고 일반화하지 않는다.
8. **촬영 용어의 세부 정의는 필요할 때 검증한다.** 사용자 노트는 학습 범위의 기록이며 용어 사전으로 검증한 자료가 아니다. 숏의 경계, 약어, 장비와 움직임의 구분, 특정 표현의 역사 등은 실제 설명에 사용하기 전에 확인한다. 이번 기록은 모든 문장을 사실로 승인한 것이 아니다.

## 6. 설명과 작업 지원 방식

- 한국어로 설명하고, '원하는 화면과 감정 → 선택할 오브젝트/컴포넌트 → 바꿀 값 → 관찰할 결과' 순서를 기본으로 한다.
- 한 번에 변수 하나 또는 작은 묶음만 바꾸어 전후를 비교하도록 돕는다.
- 코드가 필요하면 짧은 C# 예제, 핵심 줄의 역할, 붙일 오브젝트, Inspector에서 연결할 대상을 함께 설명한다.
- 프로젝트의 실제 Unity·Cinemachine 버전을 확인한 뒤 그 버전에 있는 메뉴와 필드 이름을 안내한다.
- 사용자가 카메라를 직접 실습할 수 있도록 기본 이동·사건 진행·재생 기반을 지원한다. 연출의 자동 완성과 학습 환경 마련은 당시 요청에 맞게 범위를 정한다.
- 포격, 피격, 동료의 희생, 회상을 반복 비교할 수 있는 환경을 지향한다. 아직 구현된 기능으로 간주하지 않는다.
- 다음 학습 후보는 기본 연결 → 같은 사건의 FS/POV/ELS 비교 → Damping/Dead Zone/Look Ahead 조절 → 카메라 전환 → 후처리 비교다. 이는 지원 방향이며 확정된 일정이나 완료한 실습이 아니다.

## 7. 이번에 파일로 확인한 환경과 확인 범위

2026-09-25 파일 기준:

- ProjectSettings/ProjectVersion.txt: Unity 6000.0.55f1.
- Packages/manifest.json: URP 17.0.4, Timeline 1.8.7, Input System 1.14.1 선언.
- Packages/manifest.json과 Packages/packages-lock.json에서 com.unity.cinemachine 항목이 확인되지 않았다. 향후 설치/연결 작업 시 다시 확인한다.
- URP 패키지 선언만 확인했으며 실제 활성 Render Pipeline Asset, Volume, 카메라 후처리 연결은 검사하지 않았다.
- 현재 에디터에서 열려 있는 씬, 저장 전 변경, 씬 오브젝트 및 카메라 연결 상태는 이번 작업에서 확인하지 않았다.
- 이번 작업에서는 학습 기록과 이를 찾아볼 프로젝트 안내 파일만 추가한다. 기존 씬, 스크립트, 애니메이터, 프로젝트 설정은 수정하지 않는다.

## 8. 사용자가 수집한 참고 링크

아래 링크는 사용자 노트에서 보존한 자료다. 원문이나 동작을 이번 작업에서 검증하지 않았다. 유료 에셋의 가격, 최신 호환성, 구매 적합성도 미확인이다.

- [처음 공유한 Notion 페이지](https://app.notion.com/p/3e49b1df54a980fa85f0f524a0fa6cfa?source=copy_link): 접근에 실패했다. 이후 사용자가 대화에 붙여넣은 내용을 기록의 근거로 사용했다.
- [페디스털 관련 영상](https://www.youtube.com/watch?v=ucaap9mP51w&t=6s)
- [Unity-Fisheye](https://github.com/psiorx/Unity-Fisheye)
- [unity-fulldome-camera](https://github.com/i-DAT/unity-fulldome-camera)
- [URP Distortion 에셋](https://assetstore.unity.com/packages/tools/camera/urp-distortion-346300)

## 9. 기록 갱신 원칙

사용자의 추가 학습, 직접 실습 결과, 새로운 목표가 확인되면 이 파일을 갱신한다. '노트로 학습함', '직접 구현함', '실행 결과를 확인함'을 구별한다. 이 기록은 이 프로젝트에서 참조할 로컬 파일이며 계정 전체의 영구 기억을 뜻하지 않는다.

## 10. 최신 스토리보드와 작업 범위 (2026-09-25 추가)

- 사용자가 구체적인 1~7번 장면을 제시했다. 부상한 POV로 깨어남과 동료의 깨우기 → 피와 눈 감기 → '40분 전.' → 측면 ELS → 배 위 POV·입수·수중 이동·상륙·끄덕임 → 앞선 동료를 바라봄 → 불빛과 동료들의 피격·주인공의 움츠림·전방 질주까지다.
- 이번 분량은 여기까지다. 처음의 더 큰 구상은 이후 확장 후보이며 임의로 추가하지 않는다.
- 사용자는 AI를 카메라 연습의 보조 수단으로 쓰는 목적을 재강조했다. 카메라 연출을 대신 완성하는 범위로 확대하지 않는다.
- 현재 요청은 필요한 에셋을 기존 보유품과 외부 검색으로 한 번에 추리는 일이다. 재료가 없다고 바로 제작하지 않는다.
- 에셋 선호 답변: '기존·무료 우선, 외형은 임시여도 괜찮음'.
- 기획 문서: Docs/IncheonLandingStoryboard.ko.md. 확보 계획: Docs/IncheonLandingAssetPlan.ko.md.
- 후속 파일 검사에서 저장된 Camera_Basics의 FollowCamera 대상과 Global Volume/Profile 연결을 확인했다. 설치된 Shader Graph 17.0.4 패키지 캐시에서 WaterLake, PostProcessUnderwater, PostProcessRainOnLens를 찾았다. 씬 실행이나 에디터의 저장 전 상태를 확인한 것은 아니다.
- 이번 추가 작업도 문서 작성과 조사만 수행했다. 패키지 설치, 에셋 임포트, 새 에셋 제작, 씬 및 실행 코드 수정은 하지 않았다.

### 단계별 안내 작성 시 추가 확인

- 사용자가 링크를 포함한 현실적인 단계별 진행 안내를 요청했다. Docs/IncheonLandingStepByStep.ko.md에 준비·연결·카메라 실습 순서를 저장했다.
- manifest, packages-lock, PackageCache를 다시 확인하니 Cinemachine 3.1.7과 Post Processing 3.5.4가 추가되어 있었다. 에이전트가 이번에 설치한 것은 아니다.
- 현재는 Cinemachine 설치 단계가 필요하지 않다. URP 내장 Volume 후처리를 사용하며 별도 Post Processing 패키지의 Post-process Layer/Volume 연결과 혼동하지 않는다.
- 단계별 안내는 앞으로 할 일이며 이미 실행한 작업으로 간주하지 않는다.


### 사용자 애니메이션 평가 및 후속 삭제 (2026-09-25)

- 사용자가 단계별 안내 1~5번 완료를 보고했다. 4번 외부 에셋은 Downloads에서 압축만 푼 상태였다. 저장된 파일과 보고가 다른 부분은 Docs/IncheonDownloadedAssets.ko.md에 구분하여 기록했다. 실습 씬 파일에는 Cinemachine 연결·Profile 재연결이 아직 확인되지 않으므로 저장 전 에디터 상태를 다시 확인해야 한다.
- 사용자 평가: 쓸만하다 10개, 애매하다 11개, 필요없다 65개. 최신 선택의 원본 기준은 Docs/IncheonAnimationReview.ko.md다. 종전 계획의 파일명 추정보다 사용자 평가를 우선한다.
- 사용자가 ‘파일도 제거’와 ‘애니메이터 설정 후보정’을 명시적으로 요청했다. 65개 FBX 및 같은 이름의 추출 클립 1개·압축본 2개와 대응 .meta를 제거하고 Controller 두 개를 보정했다. 원본은 UserSettings/AnimationCleanupBackups에 압축 보관했다.
- 평가하지 않은 변형 6개는 유지했다. Sotay_Dash는 상체만, DoubleJumpRolling은 앞부분만 활용할 후보라는 사용자 설명을 보존했다. 실제 편집은 미실행이다.
- 현재 기본 Idle은 Crouch_Idle, WalkRun은 Run2 단독 재생이다. 이는 삭제 후 빈 Motion 방지를 위한 임시 구성이고 완성된 자동 사건 진행이 아니다. 연결 검사는 통과했으나 Unity 재생 화면은 확인하지 않았다.
- 내려받은 UAL1 Unity용 FBX에 수영/걷기/대기 동작이 포함된 것을 확인했다. 프로젝트에 임포트하거나 기존 캐릭터에 적용한 것은 아니다.

### 맵·배치 우선 실습 안내 (2026-09-25)

- 사용자는 카메라 세팅 전에 맵과 배경 배치를 먼저 정하고 싶어 하며 본인이 할 일을 단계별로 요청했다.
- 최신 안내는 Docs/IncheonMapLayoutStepByStep.ko.md다. 큰 지형 → 사건 위치 → 배·배우 → 임시 화면 검사 → 공간 보정 → 반복 재생 기반 → 카메라 연출 → 배경·효과 마무리 순서를 제안했다. 사용자가 실제로 배치를 완료한 기록은 아니다.
- 같은 시점의 저장된 Incheon_Practice에는 활성 CinemachineBrain, CM_Opening, IncheonSceneProfile 연결이 확인됐다. 이전의 미연결 상태는 해소됐다. Movement는 활성이다. Unity Game 뷰 재생은 미확인이다.

### 사용자 오프닝 제작 이후 재확인 (2026-09-25 저녁)

- 사용자가 배경 배치 후 직접 약 15초 Opening 타임라인, CM_Opening 애니메이션, Blink_Volume, 피·암전 UI, 40 MINUTES AGO 텍스트·폰트와 TMPOutlineAnimator를 추가했다. 흐린 하늘 MAT_Sky_War_Overcast와 조명·안개·Volume 설정도 사용자 연출이다.
- 사용자는 직접 수정한 카메라·타임라인·새 파일을 건드리지 말고, 새 스카이박스 등 연출도 보존하라고 명시했다. 이 제약은 후속 작업에도 적용한다. 최초 배경 생성 설정으로 되돌리거나 지형을 재생성하지 않는다.
- Unity 편집기에서 6개 타임라인 트랙의 바인딩과 Main Camera의 Brain/후처리, 두 Volume 연결을 검사했다. 누락 스크립트·직렬화 참조가 보고되지 않았다. Play 모드에서 어두워짐과 검은 배경의 40 MINUTES AGO 표시를 확인했고 콘솔 오류·경고는 없었다.
- 배경은 기존 다운로드 모래·해안 텍스처와 프로젝트 Shader Graph의 물·바위·지형 재질로 생성되어 있다. 상세 상태와 점검 메뉴는 Docs/IncheonBackgroundBuild.ko.md에 기록한다. 수중 효과나 배·동료의 자동 연기는 아직 구현된 것으로 간주하지 않는다.

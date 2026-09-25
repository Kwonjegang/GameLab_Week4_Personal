# 인천 카메라 실습 배경과 현재 연결 상태

2026-09-25 저녁 재확인. 사용자가 작업한 ‘오프닝 연출 v1’(4b8e348)을 기준으로 확인했다.

## 현재 확인된 연결

`Opening_Timeline`의 Playable Director는 `Assets/Timeline/Opening.playable`에 연결되어 있다. 길이는 약 15초이며 Play On Awake가 켜져 있다.

| 타임라인 트랙 | 연결된 씬 대상 |
|---|---|
| Animation Track | Blink_Volume |
| Animation Track (1) | BloodOverlay |
| Animation Track (2) | BloodColor |
| Animation Track (3) | CM_Opening |
| Animation Track (4) | BlackCover |
| Animation Track (5) | Text (TMP) |

6개 트랙 모두 Animator 대상과 연결되어 있다. 타임라인에서 직접 재생하는 이 대상들은 Animator Controller가 비어 있어도 그 자체로 연결 오류는 아니다. 캐릭터 Animator에는 `UnitychanRFN_STD_FN_Ver2`가 연결되어 있다.

Main Camera에는 Cinemachine Brain이 있고 후처리가 활성화되어 있다. CM_Opening과 Main Camera의 현재 위치는 `(0, 8.06, -41.4)`다. Global Volume은 IncheonSceneProfile, Blink_Volume은 Blink_Volume Profile에 연결되어 있다. Blink_Volume의 기본 weight는 0, priority는 10이다.

실제 Play 모드에서 화면 어두워짐, 피 오버레이 변화와 검은 배경의 `40 MINUTES AGO` 표시를 확인했다. 해당 재생 확인 중 Console 오류와 경고는 0개였다. 모든 연출 타이밍이나 컷의 적절성을 평가한 것은 아니다.

## 기존 에셋으로 구성한 배경

Hierarchy의 `Enviroment > Coast_Background` 아래에 있다.

| 오브젝트 | 역할 / 수정 위치 |
|---|---|
| Coastal_Terrain | 해저·해변·뒤쪽 산. Terrain 도구에서 높이와 페인트를 수정할 수 있다. |
| Coastal_Water_Surface | 수면 Y=0의 물 평면. Coastal_Water 재질로 색과 잔물결을 조절한다. |
| Coastal_Rocks | 양옆에 배치된 바위 36개. 각각 이동·크기 조절 가능. |

지형은 768×640m, 높이맵 1025, 표면 재질 4개다. 중앙의 상륙·피격·오프닝 지점을 넓게 비워 두었다. 배 출발 지점의 수심은 약 6m, 정지 지점은 약 4.47m다. 눈맞춤 지점의 지면은 약 Y=4.45, 피격·오프닝 지면은 약 Y=7.38이다. 기존 마커 좌표는 유지되어 있다.

사용한 원본은 다음과 같다.

- Downloads의 `sand_02_4k.blend/textures`: 모래 색상·노멀 텍스처.
- Downloads의 `coast_land_rocks_01_4k.blend/textures`: 젖은 해안 색상·노멀 텍스처.
- 프로젝트의 Shader Graph 샘플 `WaterLake`: 수면 재질.
- Shader Graph Common의 `Rock_A_01`, `Rock_A_02`: 바위 프리팹.
- Shader Graph Common의 `ground_grass_fells_mossy`, `stone_ground`: 산 표면 Terrain Layer.

다운로드 원본은 유지하고 필요한 텍스처만 `Assets/IncheonEnvironment/Textures`에 복사했다. 최초 배경 생성 전 씬은 `UserSettings/EnvironmentBackups/Incheon_Practice-20260925-151439.unity`에 보관했다. 이 백업은 사용자가 이후 만든 오프닝보다 오래된 파일이다.

## 저녁 점검에서 이어서 보완한 부분

- 비활성 마커도 검사하도록 기존 배경 제작 도구를 보완했다. 사용자가 숨긴 마커를 다시 활성화하지 않는다.
- 나중의 수동 지형 편집을 덮어쓸 수 있는 일회성 지형 재생성 메뉴와 단축키를 제거했다.
- Unity 기본 Step과 겹치던 미리보기 단축키를 제거했다. 메뉴로 실행할 수 있다.
- 현재 씬의 누락 스크립트·직렬화 참조, 타임라인 대상, 카메라·Volume 연결을 읽기 전용으로 검사하는 메뉴를 추가했다.
- 넓은 숏에서 물 평면의 끝이 보이는 것을 줄이도록 기존 물 오브젝트의 X/Z Scale을 800으로 확장했다. 평면 크기는 8km×8km이며 메시 수와 재질은 그대로다.

사용자가 직접 설정한 카메라, 타임라인 키, 피·암전 UI, 폰트, TMPOutlineAnimator, 새 스카이박스 `MAT_Sky_War_Overcast`, 조명·안개·Volume·베이크 자료는 보존한다. 초기 배경의 밝은 낮 설정으로 되돌리지 않는다.

최종 Git 비교에서 씬 파일의 변경은 기존 물 평면의 Scale 한 줄뿐임을 확인했다. 사용자 Timeline·스카이박스·Profile·폰트·스크립트 파일에는 변경이 남아 있지 않다. 재생·저장 중 Unity가 자동으로 채운 동적 폰트 아틀라스도 작업 시작 시 파일로 되돌렸다. 글꼴 설정은 변경하지 않았다.

## 화면 확인 방법

1. `Incheon_Practice` 씬을 연다.
2. 배경 전체는 `Tools > Incheon Background > 2 View Coast In Scene`으로 확인한다. Scene 뷰만 이동한다.
3. 오프닝 확인은 Play로 한다. 약 15초의 현재 사용자 연출이 재생된다.
4. 필요하면 `6 Audit Current Connections`로 연결을 재검사한다. 결과는 `Temp/incheon-current-connections.txt`와 `Temp/incheon-background-validation.json`에 기록된다.
5. `3 Capture Background Previews`는 현재 조명·스카이박스·Volume을 반영한 배경 이미지를 `Docs/Previews`에 만든다. 본편 카메라를 이동시키지 않는다.

지금의 물은 수면과 해저 배경이다. 수중 색보정·물방울·입수 효과, 배의 이동, 동료의 접근·피격 동작은 별도의 다음 작업이다. 추가 구매 없이 현재 배경과 카메라 실습은 진행할 수 있다. 다음 제작 단위는 기존 오프닝을 보존한 상태에서 배·동료의 위치와 이동을 준비하는 것이다.

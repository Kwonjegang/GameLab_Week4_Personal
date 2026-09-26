# Sequence01 배 위 동료 — 설정과 사용법

확인일: 2026-09-26. 대상: `Assets/Scenes/Incheon_Practice.unity`.

## 고친 문제

배 위 7명의 `Animator > Apply Root Motion`이 켜져 있었다. 실제 Play 검사에서 배가 움직여도 배우들은 출발 지점의 월드 위치에 남고, 배 기준 Local Position이 크게 바뀌었다. 부모로 묶는 것과 Movement의 중력만 끄는 것으로는 해결되지 않았다.

배 위 인물에 한해 Apply Root Motion을 끄고, Movement와 Character Controller도 비활성화했다. 이제 인물의 전체 위치는 부모인 `ship-cargo-c`를 따라가고, 애니메이션은 몸의 자세를 바꾼다. 기존 좌석 위치·회전·크기는 바꾸지 않았다. 원본 FBX와 공유 Animator Controller도 수정하지 않았다.

기존 Controller에는 웅크린 대기 동작이 있었다. 일어서기 트랙이 없었으므로 Sequence01에 다음 두 Animation Track을 추가했다.

| 트랙 | 연결 대상 | Sequence01 시작 기준 | 전체 재생 기준 |
|---|---|---|---|
| Companion Right - Stand | UnitychanRFN_STD (1) | 20.00~21.60초 | 35.00~36.60초 |
| Companion Left - Stand | UnitychanRFN_STD (2) | 21.85~23.45초 | 36.85~38.45초 |

처음에는 기존 Crouch_Idle로 기다리다가, 기존 `UnitychanRFN_STD_FN_Sitdown_Chair_End`로 일어난다. 두 자세는 0.35초 동안 섞이며, 완료 후에는 Sequence01이 재생되는 동안 마지막 선 자세를 유지한다. 추가 다운로드는 하지 않았다. 웅크린 자세에서 의자 일어서기 동작으로 연결한 실습용 구성이다.

EyePoint의 오른쪽 보기(20.85초), 왼쪽 보기(22.67초), 주인공 일어서기(25.52~26.97초)에 앞서 양옆 동료가 차례로 일어나도록 배치했다.

## 지금 확인할 순서

1. `Incheon_Practice` 씬을 열고 Unity의 ▶ Play를 누른다.
2. 오프닝 이후 배 위 POV를 본다. 전체 재생 약 35초부터 오른쪽 동료가, 약 36.85초부터 왼쪽 동료가 일어난다.
3. 직접 조정하려면 Hierarchy에서 `Sequence01_Ship`을 선택하고 Timeline을 연다. 맨 아래 `Companion Right - Stand`, `Companion Left - Stand`가 이번에 추가한 트랙이다.
4. `Stand up` 클립을 선택하면 Inspector의 Start에서 시작 시점을 바꿀 수 있다. 이 창의 시간은 전체 재생 시간이 아니라 Sequence01 내부 시간이다. Start를 바꿀 때는 앞의 `Crouch - wait` 끝도 옮겨 약 0.35초 겹치게 유지한다.
5. 빠르기를 바꾸려면 `Stand up` 클립의 Speed Multiplier를 조절한다. 현재 원본 약 1.217초를 1.6초에 걸쳐 재생한다. Post-Extrapolate는 Hold로 둔다.

배에 타 있는 동안은 7명의 Apply Root Motion, Movement, Character Controller를 다시 켜지 않는다. 나중에 하선시키려면 배에서 내리는 시점의 이동 방식과 함께 연결한다.

## 보존과 검증

- 카메라, EyePoint, 배의 기존 Timeline 트랙과 AnimationClip 데이터는 수정 전과 일치한다. Sequence01에는 배우 트랙 2개와 그 클립만 추가했다.
- 기존 카메라 연결, Opening, SequenceManager, 스카이박스, 지형, 조명, Volume은 이 작업의 수정 대상이 아니다.
- Unity 6000.0.55f1에서 화면 없이 실제 Play 모드로 Opening → Sequence01을 44.3초까지 연속 재생해 검사했다. 13개 시점에서 7명 모두 배 기준 위치를 유지하고, 좌우 동료 머리 높이가 약 0.78에서 1.30으로 올라가 유지되는 것을 확인했다. 수치는 모델 루트 기준이다.
- 이 검사는 위치·관절·자동 재생의 동작 검증이다. 최종 카메라 구도와 동작의 인상은 사용자가 Game 뷰에서 다듬는다.
- 상세 결과: `Docs/IncheonShipPassengerVerification.json`. 검사 도구: `Assets/IncheonEnvironment/Editor/ShipPassengerSetup.cs`. 일반 편집 때 자동으로 설정을 덮어쓰지 않는다.
- 수정 전 씬·Timeline 백업: `UserSettings/ShipPassengerBackups/20260926-161531/`. 이전 씬 전체로 복원하면 이후 사용자 작업도 되돌릴 수 있으므로 비교용으로 보관한다.

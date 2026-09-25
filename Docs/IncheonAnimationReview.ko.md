# 인천 상륙 작전 — 애니메이션 평가와 정리 결과

기록일: 2026-09-25. 사용자 직접 평가를 기준으로 한다. 앞선 에셋 계획의 파일명 기반 추정보다 이 기록이 우선한다.

## 평가 및 처리 결과

| 사용자 평가 | 동작 수 | 처리 |
| --- | ---: | --- |
| 쓸만하다 | 10 | 원본 유지 |
| 애매하다 | 11 | 원본 유지, 보정 또는 재검토 후보 |
| 필요없다 | 65 | 사용자의 후속 삭제 요청에 따라 해당 FBX 및 .meta 제거 |
| 목록에 없던 변형 | 6 | 미평가로 유지 |

같은 이름의 `UnitychanRFN_STD_FN_Sword_Ready.anim`, `UnitychanRFN_STD_FN_Ladder_All.zip`, `UnitychanRFN_STD_FN_Sword_RunToIdle(Root).zip`도 제거했다. 총 에셋 파일 68개와 대응 .meta 68개를 제거했으며, 애니메이션 폴더에는 FBX 27개가 남는다. 사용자가 추출한 `UnitychanRFN_STD_DoubleJump.anim`은 유지했다. Downloads의 원본 패키지는 수정하지 않았다.

## Animator 보정

수정 대상은 `Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN.controller`와 `UnitychanRFN_STD_FN_Ver2.controller` 두 파일이다.

- 두 Controller의 GUID와 씬에서의 Controller 연결은 유지했다. 기존 이름 `Idle`, `WalkRun` 및 파라미터 이름도 유지했다.
- `Idle`의 Motion을 삭제되는 Idle 대신 사용자 승인 동작 `UnitychanRFN_STD_FN_Crouch_Idle`로 연결했다. 시작 자세가 웅크림으로 바뀌는 임시 대기 구성이다. 서 있는 대기는 내려받은 UAL1의 `Idle_Loop`을 캐릭터에 적용해 확인한 뒤 교체할 수 있다.
- `WalkRun`의 걷기/달리기 Blend Tree를 제거하고 남는 `UnitychanRFN_STD_FN_Run2`에 직접 연결했다. 현재는 걷기 동작이 섞이지 않는다. `Speed > 0.5`이면 Run2, `Speed < 0.5`이면 Idle이라는 기존 전환 조건과 0.25초 전환 시간은 유지했다. 정확히 0.5에서는 이전 상태를 유지한다.
- Ver2에서 삭제 클립을 쓰는 상태 11개와 불필요해진 전환 22개를 제거했다. 남은 상태는 `Idle`, `WalkRun`, `DownLong-Ten`, `SwordIdle`, `DamageM`이다. 두 Controller에 각각 있던 Blend Tree 1개씩을 제거했다.
- `DamageM → SwordIdle`은 두 Motion 모두 유지되므로 기존 전환을 보존했다. `DownLong-Ten`에서 삭제되는 상태로 가던 전환은 제거했다. 넘어지고 다시 일어나는 동작을 자동으로 추가하지 않았다.
- `CameraDrill_Animator.controller`는 원래 빈 Controller이며 삭제 파일을 참조하지 않아 수정하지 않았다.
- 캐릭터를 자동으로 움직이거나 Speed를 갱신하는 기능을 추가한 것은 아니다. 기존 Movement 코드에도 Animator Speed 갱신이 없었다. 사건별 재생 기반은 후속 작업이다.

검증: Assets 내 파일 1038개를 검사하여 삭제 GUID 참조 0건, 수정 Controller의 끊긴 내부 참조 0건, 빈 Motion 0건을 확인했다. 각 Motion의 GUID와 FBX 내부 클립 ID가 남은 파일의 .meta와 일치한다. 삭제 대상 외 Assets 파일 1,034개의 내용이 작업 전과 동일함도 확인했다. `git diff --check`는 통과했다. Unity 재임포트 후 실제 캐릭터의 재생, 발 접지, 자세 전환 화면은 아직 확인하지 않았다.

## 복구본

`UserSettings/AnimationCleanupBackups/before-cleanup-20260925-023950.zip`

삭제 파일, 대응 .meta, 수정 직전 Controller와 .meta, SHA-256 목록이 들어 있다. 압축 무결성과 파일 내용을 확인했다. Assets 밖의 UserSettings에 있으므로 Unity가 중복 에셋으로 읽지 않는다. UserSettings는 이 프로젝트에서 Git 제외 대상이므로 다른 PC로 자동 공유되지 않는다.

복구할 때는 압축 안의 프로젝트 상대 경로대로 필요한 에셋과 .meta를 함께 돌려놓는다. Controller 원본까지 복구하면 삭제 이전 상태 구성이 돌아오므로 전체 연결을 되돌릴 때 함께 복구한다. 이후 수정한 Controller가 있다면 덮어쓰기 전에 비교해야 한다.

## 쓸만하다 — 유지

- `UnitychanRFN_STD_DoubleJump`
- `UnitychanRFN_STD_FN_Creeping_All`
- `UnitychanRFN_STD_FN_Crouch_Idle`
- `UnitychanRFN_STD_FN_Down-Chi-Long`
- `UnitychanRFN_STD_FN_Down-Ten-Long`
- `UnitychanRFN_STD_FN_Land-Run2`
- `UnitychanRFN_STD_FN_Run2`
- `UnitychanRFN_STD_FN_Sitdown_Chair_All`
- `UnitychanRFN_STD_FN_Wall_Pull`
- `UnitychanRFN_STD_FN_Wall_Push`

## 애매하다 — 유지

- `Sotay_Dash`
- `UnitychanRFN_STD_DoubleJumpRolling`
- `UnitychanRFN_STD_FN_Crouch_Walk`
- `UnitychanRFN_STD_FN_DamageM`
- `UnitychanRFN_STD_FN_Guard-HitM`
- `UnitychanRFN_STD_FN_Guard-Idle`
- `UnitychanRFN_STD_FN_Sword_Idle`
- `UnitychanRFN_STD_FN_Sword_Idle3`
- `UnitychanRFN_STD_FN_Sword_Walk`
- `UnitychanRFN_STD_FN_Talking_Normal`
- `UnitychanRFN_STD_FN_Talking_Yes`

사용자 메모:

- `Sotay_Dash`: 고개를 내리고 눈을 가리는 상체만 유용하다. 하체를 뒤로 빼는 대시 동작은 필요 없다. 상체만 쓰는 Avatar Mask/동작 혼합 후보이며 아직 마스크를 제작하거나 적용하지 않았다.
- `UnitychanRFN_STD_DoubleJumpRolling`: 중간까지 쓸만하다. 포탄을 맞고 날아가 쓰러지는 장면에 앞부분만 쓰는 후보다. 자를 실제 프레임은 미정이다. 포격 장면은 현재 1~7번의 필수 범위로 추가하지 않는다.

## 필요없다 — 삭제

- `UnitychanRFN_STD_BackStep`
- `UnitychanRFN_STD_FN_ArielKick`
- `UnitychanRFN_STD_FN_DamageKB`
- `UnitychanRFN_STD_FN_DamageS`
- `UnitychanRFN_STD_FN_Down-Chi-Impact`
- `UnitychanRFN_STD_FN_Down-Ten-Rolling`
- `UnitychanRFN_STD_FN_Down-Ten-Short`
- `UnitychanRFN_STD_FN_Elude_All`
- `UnitychanRFN_STD_FN_Embarrassed`
- `UnitychanRFN_STD_FN_Guard-HitS`
- `UnitychanRFN_STD_FN_Guard-Release`
- `UnitychanRFN_STD_FN_Idle`
- `UnitychanRFN_STD_FN_Jump_All`
- `UnitychanRFN_STD_FN_Jump_InAir`
- `UnitychanRFN_STD_FN_Ladder_All`
- `UnitychanRFN_STD_FN_Ladder_Up`
- `UnitychanRFN_STD_FN_Land-Walk2`
- `UnitychanRFN_STD_FN_Lose`
- `UnitychanRFN_STD_FN_Rolling`
- `UnitychanRFN_STD_FN_Run`
- `UnitychanRFN_STD_FN_Search_Ground`
- `UnitychanRFN_STD_FN_Seiza_All`
- `UnitychanRFN_STD_FN_Sitdown_Sankaku`
- `UnitychanRFN_STD_FN_Sliding`
- `UnitychanRFN_STD_FN_Sliding_All`
- `UnitychanRFN_STD_FN_Stand_RifleAim`
- `UnitychanRFN_STD_FN_Standup_Sankaku`
- `UnitychanRFN_STD_FN_Stomp`
- `UnitychanRFN_STD_FN_Sword_Idle2`
- `UnitychanRFN_STD_FN_Sword_Ready`
- `UnitychanRFN_STD_FN_Sword_Run`
- `UnitychanRFN_STD_FN_Sword_Run2`
- `UnitychanRFN_STD_FN_Sword_RunToIdle(Root)`
- `UnitychanRFN_STD_FN_Sword_RunToIdle`
- `UnitychanRFN_STD_FN_Sword_Sheath`
- `UnitychanRFN_STD_FN_Sword_Slash1`
- `UnitychanRFN_STD_FN_Sword_Slash2`
- `UnitychanRFN_STD_FN_Sword_Slash3`
- `UnitychanRFN_STD_FN_Sword_Slash_InAir1`
- `UnitychanRFN_STD_FN_Sword_Slash_Upper`
- `UnitychanRFN_STD_FN_Sword_Walk-Start`
- `UnitychanRFN_STD_FN_Sword_WalkingSheath(Root)`
- `UnitychanRFN_STD_FN_Talking_Hi`
- `UnitychanRFN_STD_FN_Talking_No`
- `UnitychanRFN_STD_FN_Turn`
- `UnitychanRFN_STD_FN_Use_Forward`
- `UnitychanRFN_STD_FN_Use_Up`
- `UnitychanRFN_STD_FN_Victory`
- `UnitychanRFN_STD_FN_Wakeup-Chi`
- `UnitychanRFN_STD_FN_Wakeup-Ten`
- `UnitychanRFN_STD_FN_Wakeup-Ten_BackRolling`
- `UnitychanRFN_STD_FN_Walk`
- `UnitychanRFN_STD_FN_Walk2`
- `UnitychanRFN_STD_FN_Walk2_ForwardLeft`
- `UnitychanRFN_STD_FN_Walk2_ForwardRight`
- `UnitychanRFN_STD_FN_Walk2_Left`
- `UnitychanRFN_STD_FN_Walk2_Right`
- `UnitychanRFN_STD_FN_Wall_Hang`
- `UnitychanRFN_STD_FN_Wall_Jump`
- `UnitychanRFN_STD_FN_Wall_Touch`
- `UnitychanRFN_STD_FN_WallCheck`
- `UnitychanRFN_STD_FN_Wall_Uplift_M`
- `UnitychanRFN_STD_GhostDash-Dash`
- `UnitychanRFN_STD_GhostDash-End`
- `UnitychanRFN_STD_GhostDash-Start`

## 미평가 변형 — 유지

- `UnitychanRFN_STD_BackStep(Origin)`
- `UnitychanRFN_STD_FN_Down-Ten-Long(Origin)`
- `UnitychanRFN_STD_FN_Jump_All_Origin`
- `UnitychanRFN_STD_FN_Sword_WalkingSheath`
- `UnitychanRFN_STD_FN_Wakeup-Ten(Origin)`
- `UnitychanRFN_STD_GhostDash-Dash_Mesh`

이름이 비슷해도 별도 파일이므로 사용자 목록에 없는 변형까지 삭제 범위를 확대하지 않았다.

## 장면에 연결할 후보

| 필요한 행동 | 다음 확인 후보 | 남은 확인 |
| --- | --- | --- |
| 접근/보행 | 다운로드 UAL1 `Walk_Loop` | 기존 Walk/Walk2는 제외됨. 리타기팅·발 미끄러짐 확인 |
| 전방 질주 | 기존 Run2 | 경로 속도와 동작 맞추기 |
| 낮추고 깨우기 | 기존 Crouch_Idle, Sitdown_Chair_All, Wall_Pull/Push / UAL1 Fixing_Kneeling, Interact | 의자 앉기와 지면 자세는 다름. 어깨 접촉과 손 왕복은 아직 미확보 |
| 입수 | 기존 DoubleJump, Land-Run2 | 입수에 적합한 부분과 이동 경로 확인 |
| 수영 | 다운로드 UAL1 Swim_Fwd_Loop, Swim_Idle_Loop | Unity용 FBX에 포함 확인. 캐릭터 적용·수중 이동은 미실행 |
| 끄덕임 | 기존 Talking_Yes | 사용자 평가 애매함. 필요 부분만 쓰거나 고개만 보정 |
| 피격·넘어짐 | 기존 Down-Chi-Long, Down-Ten-Long / DamageM은 보류 | 넘어짐 시점·마지막 자세·지면 높이 확인 |

다음 재료 확인 및 저장된 씬 상태는 `Docs/IncheonDownloadedAssets.ko.md`를 참고한다.

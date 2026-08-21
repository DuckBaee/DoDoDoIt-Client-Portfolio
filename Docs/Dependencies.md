# Dependencies

이 Repository는 단독 컴파일을 지원하지 않습니다. 다른 팀원의 코드를 가져오거나 Stub을 새로 작성하는 대신 원본 의존성을 문서로 남깁니다.

| 전시 코드 | 의존 시스템 | 소유권 | 처리 |
|---|---|---|---|
| Rope States | `PlayerController` | 팀 중심 공동 코드 | 복사하지 않음 |
| Rope States | `IPlayerState`, `PlayerStateMachine` | 팀원 중심 | 복사하지 않음 |
| Rope 이동 | Rigidbody, Transform | Unity | 코드 API 사용만 유지 |
| Rope 표현 | Animator, LineRenderer | Unity | Scene/Animator/Material 미포함 |
| Rope 수치 | `PlayerStats`, Attributes | 팀 시스템 | 복사하지 않음 |
| Target Detector | Running/Jump/Falling States | 팀 중심 공동 코드 | 호출 관계만 문서화 |
| Target Effect | `Managers.Resource`, `Util` | 팀 시스템 | 복사하지 않음 |
| Camera | `CameraManager` | 타인 작성 | 복사하지 않음 |
| Camera | Cinemachine Virtual Camera | Unity Package | Package와 Prefab 미포함 |
| Rope Sound | `SoundManager` | 본인 중심 공동 코드 | 이번 Source에서 제외 |
| Sound | FMOD | 외부 middleware | Package와 Bank 미포함 |
| Curve Effect | `Managers.Resource` | 팀 시스템 | 복사하지 않음 |

## 단독 실행을 지원하지 않는 이유

본 저장소의 목적은 게임을 재현하는 것이 아니라 다음을 코드로 검증하는 것입니다.

- 실제 Commit 당시 구현
- Prototype과 Release의 구조 차이
- 본인 코드와 팀 시스템의 경계
- 실제 문제를 수정한 Git 이력

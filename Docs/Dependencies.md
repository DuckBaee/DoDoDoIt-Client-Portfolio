# Team & External Dependencies

DoDoDoIt!은 팀 프로젝트이기 때문에 제가 구현한 Rope 코드도 팀 공용 Player 시스템과 외부 Unity 기능을 사용합니다.

| 제 코드 | 연결된 시스템 | 구분 |
|---|---|---|
| Rope States | `PlayerController` | 팀 공용 코드 |
| Rope States | `IPlayerState`, `PlayerStateMachine` | 팀 공용 코드 |
| Rope 이동 | Rigidbody, Transform | Unity |
| Rope 표현 | Animator, LineRenderer | Unity |
| Rope 수치 | `PlayerStats`, Attributes | 팀 공용 코드 |
| Target 입력 | Running/Jump/Falling States | 팀 공용 코드 |
| Target Effect | `Managers.Resource`, `Util` | 팀 공용 코드 |
| Camera | `CameraManager` | 팀원 코드 |
| Camera | Cinemachine Virtual Camera | Unity Package |
| Sound | `SoundManager` | 본인 중심 공동 코드 |
| Sound | FMOD | 외부 Middleware |

이 저장소에는 제가 보여주려는 Rope·Camera 코드만 담았습니다. Player 공용 시스템, Scene, Prefab과 외부 Asset은 복사하지 않았기 때문에 단독 실행용 Unity 프로젝트는 아닙니다.

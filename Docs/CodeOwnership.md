# Code Ownership

## 판정 기준

- 로컬 Git 사용자: `duckbaee23 / duckbaee23@gmail.com`
- 본인 Commit 작성자명으로 판단한 값: `DuckBaee`
- 파일 최초 추가 Commit, file history와 `git blame`을 함께 확인
- 한 줄이라도 다른 작성자가 남아 있는 파일은 공동 수정으로 표시

blame은 최종 파일에 남은 줄을 기준으로 하므로 삭제되거나 이후 대체된 과거 작업량까지 나타내지는 않습니다.

## 복사 파일

| 단계 | 파일 | Commit | blame 요약 | 판정 |
|---|---|---|---|---|
| Prototype 01 | `RopeActionWithRunning.cs` | `1ff69ecb` | DuckBaee 113 / StarPilot 2 | 공동 수정, 본인 중심 |
| Prototype 02 | `PlayerGrapplingState.cs` | `51c040b8` | DuckBaee 171 / StarPilot 22 | 공동 수정, 본인 중심 |
| Lifecycle Split | `PlayerEnterGrapllingState.cs` | `a59070ce` | DuckBaee 103 | 본인 작성 확인 |
| Lifecycle Split | `PlayerGrapplingState.cs` | `a59070ce` | DuckBaee 46 / StarPilot 계열 68 | 공동 수정 |
| Lifecycle Split | `PlayerExitGrapllingState.cs` | `a59070ce` | DuckBaee 56 | 본인 작성 확인 |
| Lifecycle Split | `RopeSystem.cs` | `a59070ce` | DuckBaee 32 | 본인 작성 확인 |
| Release | `PlayerEnterGrapllingState.cs` | `c0247122` | DuckBaee 197 | 본인 작성 확인 |
| Release | `PlayerGrapplingState.cs` | `c0247122` | DuckBaee 68 / StarPilot 계열 38 | 공동 수정 |
| Release | `PlayerExitGrapllingState.cs` | `c0247122` | DuckBaee 73 / StarPilot01 1 | 공동 수정, 본인 중심 |
| Release | `GrapplePointDetector.cs` | `c0247122` | DuckBaee 112 / StarPilot01 91 | 공동 수정 |
| Release | `PlayerEnterRotateGrapllingState.cs` | `c0247122` | DuckBaee 122 / StarPilot01 2 | 공동 수정, 본인 중심 |
| Release | `PlayerRotateGrapplingState.cs` | `c0247122` | DuckBaee 87 / StarPilot01 2 | 공동 수정, 본인 중심 |
| Release | `PlayerExitRotateGrapplingState.cs` | `c0247122` | DuckBaee 72 / StarPilot01 1 | 공동 수정, 본인 중심 |
| Release | `LevelCurve.cs` | `c0247122` | DuckBaee 40 | 본인 작성 확인 |
| Release | `CurveTrigger.cs` | `c0247122` | DuckBaee 중심 공동 수정 | 공동 수정 |
| Camera | `SwitchCam.cs` | `c0247122` | DuckBaee 53 / StarPilot01 9 | 공동 수정, 본인 중심 |
| Camera | `CameraRegister.cs` | `c0247122` | DuckBaee 21 | 본인 작성 확인 |

## 포함하지 않은 핵심 의존 파일

| 파일 | 이유 |
|---|---|
| `PlayerController.cs` | Release 635줄 중 DuckBaee 100줄, 팀 중심 코드 |
| `PlayerStateMachine.cs` | StarPilot 계열 58줄, DuckBaee 7줄 |
| Running/Jump/Falling | 본인 수정은 있으나 Release의 핵심 구현은 팀 코드 |
| `CameraManager.cs` | Release 최종 blame이 타인 작성 |

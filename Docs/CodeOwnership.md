# Code Ownership

팀 프로젝트에서 제가 작성한 부분과 공동 수정한 부분을 구분했습니다. Git Commit 작성자 `DuckBaee`를 제 작업으로 보고 file history와 `git blame`을 확인했습니다.

| 단계 | 파일 | Commit | 작성 범위 |
|---|---|---|---|
| Prototype 01 | `RopeActionWithRunning.cs` | `1ff69ecb` | 공동 수정, 본인 113/115줄 |
| Prototype 02 | `PlayerGrapplingState.cs` | `51c040b8` | 공동 수정, 본인 171/193줄 |
| Lifecycle Split | `PlayerEnterGrapllingState.cs` | `a59070ce` | 본인 작성 103/103줄 |
| Lifecycle Split | `PlayerGrapplingState.cs` | `a59070ce` | 공동 수정 |
| Lifecycle Split | `PlayerExitGrapllingState.cs` | `a59070ce` | 본인 작성 56/56줄 |
| Lifecycle Split | `RopeSystem.cs` | `a59070ce` | 본인 작성 32/32줄 |
| Release | `PlayerEnterGrapllingState.cs` | `c0247122` | 본인 작성 197/197줄 |
| Release | `PlayerGrapplingState.cs` | `c0247122` | 공동 수정, 본인 68/106줄 |
| Release | `PlayerExitGrapllingState.cs` | `c0247122` | 공동 수정, 본인 73/74줄 |
| Release | `GrapplePointDetector.cs` | `c0247122` | 공동 수정, 본인 112/203줄 |
| Release | `PlayerEnterRotateGrapllingState.cs` | `c0247122` | 공동 수정, 본인 122/124줄 |
| Release | `PlayerRotateGrapplingState.cs` | `c0247122` | 공동 수정, 본인 87/89줄 |
| Release | `PlayerExitRotateGrapplingState.cs` | `c0247122` | 공동 수정, 본인 72/73줄 |
| Release | `LevelCurve.cs` | `c0247122` | 본인 작성 40/40줄 |
| Release | `CurveTrigger.cs` | `c0247122` | 공동 수정, 본인 중심 |
| Camera | `SwitchCam.cs` | `c0247122` | 공동 수정, 본인 53/62줄 |
| Camera | `CameraRegister.cs` | `c0247122` | 본인 작성 21/21줄 |

PlayerController와 PlayerStateMachine은 팀 공용 코드라서 저장소에 포함하지 않았습니다. Rope State가 해당 시스템에 의존하는 관계는 Architecture와 Dependencies 문서에 표시했습니다.

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace DoDoDoIt
{
    public class SwitchCam : MonoBehaviour
    {
        private PlayerController _playerController;
        public CinemachineVirtualCamera MainCam;
        public CinemachineVirtualCamera RopeSwingCam;
        public CinemachineVirtualCamera RopeRotateCam;
        public CinemachineVirtualCamera RopeStartCam;
        public CinemachineVirtualCamera CurveLeftCam;
        public CinemachineVirtualCamera CurveRightCam;
        public CinemachineVirtualCamera EndingCam;
        private void Awake()
        {
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        public void SwitchToEndingCam()
        {
            CameraManager.SwitchCamera(EndingCam);
        }
        private void Update()
        {
            if (_playerController.StateMachine.GetCurrentState() is PlayerRotateGrapplingState)
            {
                RopeRotateCam.LookAt = _playerController.GrapPoint.gameObject.transform;
                CameraManager.SwitchCamera(RopeRotateCam);
            }
            else if (_playerController.StateMachine.GetCurrentState() is PlayerEnterGrapplingState)
            {
                RopeStartCam.LookAt = _playerController.GrapPoint.gameObject.transform;
                CameraManager.SwitchCamera(RopeStartCam);
            }
            else if (_playerController.StateMachine.GetCurrentState() is PlayerGrapplingState)
            {
                CameraManager.SwitchCamera(RopeSwingCam);
            }
            else if (_playerController.StateMachine.GetCurrentState() is PlayerEnterLeftCurveState)
            {
                CameraManager.SwitchCamera(CurveLeftCam);
            }
            else if (_playerController.StateMachine.GetCurrentState() is PlayerEnterRightCurveState)
            {
                CameraManager.SwitchCamera(CurveRightCam);
            }
            else if (_playerController.StateMachine.GetCurrentState() is PlayerEndingSceneRunningState)
            {
                CameraManager.SwitchCamera(EndingCam);
            }
            else
            {
                CameraManager.SwitchCamera(MainCam);
            }
        }
    }
}

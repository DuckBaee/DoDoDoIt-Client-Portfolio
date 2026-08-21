using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace DoDoDoIt
{
    public class CameraRegister : MonoBehaviour
    {
        private void OnEnable()
        {
            CameraManager.Register(GetComponent<CinemachineVirtualCamera>());
        }

        private void OnDisable()
        {
            CameraManager.UnRegister(GetComponent<CinemachineVirtualCamera>());
        }
    }
}

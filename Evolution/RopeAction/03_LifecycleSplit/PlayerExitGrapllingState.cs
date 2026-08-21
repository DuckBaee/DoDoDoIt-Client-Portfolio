using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerExitGrapplingState : IPlayerState
    {
        private PlayerController _player;
        private float _rotationDuration = 0.5f;

        public void EnterState(PlayerController player)
        {
            _player = player;
            _player.StartCoroutine(SmoothRotate(_player.gameObject.transform));
        }

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            
        }

        public void ExitState()
        {
            _player.GrapPoint = null;
        }
        

        private IEnumerator SmoothRotate(Transform child)
        {
            Quaternion initialRotation = child.rotation;
            Quaternion targetRotation = Quaternion.Euler(Vector3.zero);
        
            float elapsedTime = 0f;
        
            while (elapsedTime < _rotationDuration)
            {
                // 시간에 따라 회전을 부드럽게 변경
                child.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / _rotationDuration);
                elapsedTime += Time.deltaTime;
        
                yield return null;
            }
        
            // 최종적으로 목표 회전에 정확히 도달
            child.rotation = targetRotation;
            _player.TransitionTo(Define.EPlayerState.Running);
        }
    }
}
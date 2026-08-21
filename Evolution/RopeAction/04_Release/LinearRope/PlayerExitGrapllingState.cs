using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace DoDoDoIt
{
    public class PlayerExitGrapplingState : IPlayerState
    {
        private PlayerController _player;
        private float _rotationDuration = 0.5f;
        private float rotationDir = 0f;
        public void EnterState(PlayerController player)
        {
            SoundManager.Instance.PlaySFX("event:/SFX/Dodo/Lantern", "Type", 3f);
            _player = player;
            _player.StartCoroutine(SmoothRotate(_player.gameObject.transform));
            _player.Rigidbody.velocity = _player.gameObject.transform.forward.normalized * 100f;
        }

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            _player.Rigidbody.AddForce ( 
                Vector3.up * _player.Stats.Attributes.GravityScale.GetValue(),
                ForceMode.Acceleration
            );
        }

        public void ExitState()
        {
            
        }


        private IEnumerator SmoothRotate(Transform child)
        {
            Quaternion initialRotationPlayer = child.rotation;
            Quaternion initialRotationGFX = _player.PlayerGFX.gameObject.transform.rotation;
            if (_player.RotationDirection != 0)
            {
                rotationDir = _player.RotationDirection <= -1 ? 90f * Mathf.Abs(_player.RotationDirection)  : -90f * Mathf.Abs(_player.RotationDirection);
            }
            else
            {
                rotationDir = 0;
            }
            Quaternion targetRotation = Quaternion.Euler(0f, rotationDir, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < _rotationDuration)
            {
                // 시간에 따라 회전을 부드럽게 변경
                child.rotation = Quaternion.Slerp(initialRotationPlayer, targetRotation, elapsedTime / _rotationDuration);
                _player.PlayerGFX.gameObject.transform.rotation = Quaternion.Slerp(initialRotationGFX, targetRotation,
                    elapsedTime / _rotationDuration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 최종적으로 목표 회전에 정확히 도달
            child.rotation = targetRotation;
            _player.PlayerGFX.gameObject.transform.rotation = targetRotation;
            _player.TransitionTo(Define.EPlayerState.Running);
        }
    }
}
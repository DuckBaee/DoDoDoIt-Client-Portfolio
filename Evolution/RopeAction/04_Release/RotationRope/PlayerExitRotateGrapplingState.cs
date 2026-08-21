using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerExitRotateGrapplingState : IPlayerState
    {
        private PlayerController _player;
        private float _rotationDuration = 0.5f;

        public void EnterState(PlayerController player)
        {
            _player = player;
            _player.StartCoroutine(SmoothRotate(_player.gameObject.transform));
            Vector3 forwardForce = _player.transform.forward * 100f;
            _player.Rigidbody.velocity = forwardForce + new Vector3(0f, 80f, 0f);
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
            Quaternion targetRotation;

            if (_player.RotationDirection == -1)
            {
                targetRotation = Quaternion.LookRotation(Vector3.left);
            }
            else
            {
                targetRotation = Quaternion.LookRotation(Vector3.right);
            }

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

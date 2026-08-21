using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerGrapplingState : IPlayerState , IDamageableState
    {
        private PlayerController _player;
        private bool _bShouldGrappling;
        private bool _bExistGrapplingPoint;

        private float _targetHeight = 3f;
        private float _fixedRadius = 10f;
        private float _cameraMoveSpeed = 1f; // 카메라 이동 속도

        private CinemachineVirtualCamera _virtualCamera;

        public void EnterState(PlayerController player)
        {
            _player = player;
            _virtualCamera = _player.Cam;
        }

        public void Update()
        {
            DrawRope();
            RopeTransformMatch();
        }

        public void FixedUpdate()
        {
            Grappling();
        }

        public void ExitState()
        {
            
        }

        private void Grappling()
        {
            _player.Rigidbody.velocity = new Vector3(0f, 0f, _player.Stats.Attributes.ForwardSpeed);
            float currentPlayerHeight = _player.transform.position.y;
            if (currentPlayerHeight < _player.GrapPoint.transform.position.y + _targetHeight)
            {
                Vector3 directionToGrapPoint = _player.transform.position - _player.GrapPoint.transform.position;
                float currentDistance = directionToGrapPoint.magnitude;

                if (Mathf.Abs(currentDistance - _fixedRadius) > 0.01f)
                {
                    Vector3 targetPosition = _player.GrapPoint.transform.position + directionToGrapPoint.normalized * _fixedRadius;
                    _player.gameObject.transform.position = Vector3.Lerp(_player.transform.position, targetPosition, 0.1f);
                }

                _player.transform.RotateAround(_player.GrapPoint.transform.position, Vector3.left, 90f * Time.deltaTime);
            }
            else
            {
                _player.Animator.SetTrigger("EndSwing");
                _player.TransitionTo(Define.EPlayerState.ExitGrappling);
            }
        }

        private void DrawRope()
        {
            if (_player.LineRenderer != null && _player.GrapPoint != null)
            {
                _player.LineRenderer.SetPosition(0, _player.gameObject.transform.position);
                _player.LineRenderer.SetPosition(1, _player.GrapPoint.gameObject.transform.position);
            }
        }


        private void RopeTransformMatch()
        {
            Vector3 parentPosition = _player.GrapPoint.gameObject.transform.position;

            Vector3 childPosition = _player.transform.position;

            float newX = Mathf.Lerp(childPosition.x, parentPosition.x, 1f * Time.deltaTime);

            _player.transform.position = new Vector3(newX, childPosition.y, childPosition.z);
        }

        

        private IEnumerator CameraMove(Vector3 startOffset, Vector3 endOffset, float duration)
        {
            if (_virtualCamera != null)
            {
                CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    transposer.m_FollowOffset = Vector3.Lerp(startOffset, endOffset, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transposer.m_FollowOffset = endOffset;
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            throw new System.NotImplementedException();
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DoDoDoIt.Define;

namespace DoDoDoIt
{
    public class LevelCurve : MonoBehaviour , IGrapplable
    {
        private PlayerController _playerController;
        private GameObject _gameObject;

        public int rotationDirection = 1; // -1 : 반시계     |||    1 : 시계
        private void Awake()
        {
            _playerController = FindAnyObjectByType<PlayerController>();
            _gameObject = Util.FindChild(this.gameObject, "Object029", true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _playerController.GrapPoint = _gameObject;
                IsGrapplable(_gameObject);

                _playerController.RotationDirection = rotationDirection;
                _playerController.TransitionTo(EPlayerState.EnterRotateGrappling);
            }
        }

        public void IsGrapplable(GameObject GrapplePoint)
        {
            GameObject GrapplePointEffect = Managers.Resource.Instantiate("GrapplePointEffect");
            GrapplePointEffect.transform.position = GrapplePoint.transform.position + Vector3.up * 8f;
            Destroy(GrapplePointEffect, 2f);
        }
    }
}

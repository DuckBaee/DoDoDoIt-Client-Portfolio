using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DoDoDoIt.Define;

namespace DoDoDoIt
{
    public class CurveTrigger : MonoBehaviour
    {
        private PlayerController _player;
        
        [SerializeField]
        private int _curveDir;

        [SerializeField] 
        private bool _isApplyRocket = false;
        
        private void Awake()
        {
            _player = FindAnyObjectByType<PlayerController>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _isApplyRocket == _player.IsAppliedRocketBuff)
            {
                
                _player.RotationDirection += _curveDir;
                
                if (_curveDir == 1)
                {
                    _player.StateMachine.TransitionTo(EPlayerState.EnterLeftCurve);
                }
                else if (_curveDir == -1)
                {
                    _player.StateMachine.TransitionTo(EPlayerState.EnterRightCurve);
                }
                Destroy(this);
            }
        }
    }
}

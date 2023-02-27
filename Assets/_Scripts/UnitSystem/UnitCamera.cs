using System;
using Cinemachine;
using CommandSystem;
using UnityEngine;

namespace UnitSystem
{
    public class UnitCamera : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        [SerializeField] private CinemachineVirtualCamera shootCamera;


        private void Start()
        {
            if (unit.TryGetCommand(out ShootCommand shootCommand))
            {
                shootCommand.OnStart += ShootCommand_OnStart;
                shootCommand.OnComplete += ShootCommand_OnComplete;
            }
        }

        private void OnDestroy()
        {
            if (unit.TryGetCommand(out ShootCommand shootCommand))
            {
                shootCommand.OnStart -= ShootCommand_OnStart;
                shootCommand.OnComplete -= ShootCommand_OnComplete;
            }
        }

        
        private void ShootCommand_OnStart()
        {
            GameCamera.ActivateCamera(shootCamera);
        }
        
        private void ShootCommand_OnComplete()
        {
            GameCamera.DeactivateCamera(shootCamera);
        }
    }
}
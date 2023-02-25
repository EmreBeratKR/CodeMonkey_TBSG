using System.Collections.Generic;
using GridSystem;
using UnityEngine;

namespace CommandSystem
{
    public class SpinCommand : BaseCommand
    {
        private float m_TargetSpinAngle;
        private float m_SpinnedAngle;


        private void Update()
        {
            PerformSpinning();
        }


        public override void Execute(CommandArgs args)
        {
            StartSpinning();
        }

        public override IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            yield return Unit.GridPosition;
        }

        public override string GetName()
        {
            const string commandName = "Spin";
            return commandName;
        }


        private void PerformSpinning()
        {
            if (!isActive) return;

            var hasReachedTargetSpinAngle = m_SpinnedAngle >= m_TargetSpinAngle;
            
            if (hasReachedTargetSpinAngle)
            {
                StopSpinning();
                return;
            }

            const float spinSpeed = 360f;
            var deltaSpinAngle = Time.deltaTime * spinSpeed;
            transform.eulerAngles += Vector3.up * deltaSpinAngle;
            m_SpinnedAngle += deltaSpinAngle;
        }
        
        private void StartSpinning()
        {
            const float targetSpinAngle = 360f;
            m_TargetSpinAngle = targetSpinAngle;
            m_SpinnedAngle = 0f;
            isActive = true;
            InvokeOnStart();
        }

        private void StopSpinning()
        {
            isActive = false;
            InvokeOnComplete();
        }
    }
}
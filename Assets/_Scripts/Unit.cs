using UnityEngine;

public class Unit : MonoBehaviour
{
    private Vector3 m_TargetPosition;


    private void Update()
    {
        MoveTowardsTargetPosition();
    }


    public void Move(Vector3 position)
    {
        m_TargetPosition = position;
    }
    
    
    private void MoveTowardsTargetPosition()
    {
        if (HasReachedToTargetPosition()) return;
        
        var directionNormalized = (m_TargetPosition - transform.position).normalized;
        const float moveSpeed = 4f;
        var motion = directionNormalized * (Time.deltaTime * moveSpeed);
        transform.position += motion;
    }

    private bool HasReachedToTargetPosition()
    {
        const float maxSqrDistanceError = 0.0001f;
        var sqrDistanceToTarget = Vector3.SqrMagnitude(m_TargetPosition - transform.position);

        return sqrDistanceToTarget <= maxSqrDistanceError;
    }
}
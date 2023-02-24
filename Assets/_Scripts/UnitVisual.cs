using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private static readonly int IsMovingID = Animator.StringToHash("IsMoving");


    [SerializeField] private Unit unit;
    [SerializeField] private Animator animator;


    private void Awake()
    {
        unit.OnStartMoving += OnStartMoving;
        unit.OnStopMoving += OnStopMoving;
    }

    private void OnDestroy()
    {
        unit.OnStartMoving -= OnStartMoving;
        unit.OnStopMoving -= OnStopMoving;
    }

    
    private void OnStartMoving()
    {
        SetIsMoving(true);
    }
    
    private void OnStopMoving()
    {
        SetIsMoving(false);
    }
    
    
    private void SetIsMoving(bool value)
    {
        animator.SetBool(IsMovingID, value);
    }
}
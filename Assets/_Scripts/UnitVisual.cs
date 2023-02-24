using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private static readonly int IsMovingID = Animator.StringToHash("IsMoving");


    [SerializeField] private Unit unit;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject selectedVisual;


    private void Awake()
    {
        unit.OnStartMoving += OnStartMoving;
        unit.OnStopMoving += OnStopMoving;
        
        UnitCommander.OnSelectedUnitChanged += OnSelectedUnitChanged;
    }

    private void OnDestroy()
    {
        unit.OnStartMoving -= OnStartMoving;
        unit.OnStopMoving -= OnStopMoving;
        
        UnitCommander.OnSelectedUnitChanged -= OnSelectedUnitChanged;
    }

    
    private void OnStartMoving()
    {
        SetIsMoving(true);
    }
    
    private void OnStopMoving()
    {
        SetIsMoving(false);
    }
    
    private void OnSelectedUnitChanged(UnitCommander.SelectedUnitChangedArgs args)
    {
        var isSelected = args.unit == unit;
        SetActiveSelectedVisual(isSelected);
    }


    private void SetIsMoving(bool value)
    {
        animator.SetBool(IsMovingID, value);
    }

    private void SetActiveSelectedVisual(bool value)
    {
        selectedVisual.SetActive(value);
    }
}
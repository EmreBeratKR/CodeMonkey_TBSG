using TMPro;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitStatusUI : MonoBehaviour
    {
        [SerializeField] private GameObject main;
        [SerializeField] private Unit unit;
        [SerializeField] private Health health;
        [SerializeField] private TMP_Text commandPointField;


        private void Awake()
        {
            unit.OnUnitUsedCommandPoint += OnUnitUsedCommandPoint;
            
            health.OnDead += OnDead;
        }

        private void OnDestroy()
        {
            unit.OnUnitUsedCommandPoint -= OnUnitUsedCommandPoint;
            
            health.OnDead -= OnDead;
        }


        private void OnUnitUsedCommandPoint()
        {
            SetCommandPoint(unit.CommandPoint);
        }

        private void OnDead()
        {
            SetActive(false);
        }
        
        
        private void SetCommandPoint(int value)
        {
            commandPointField.text = value.ToString();
        }

        private void SetActive(bool value)
        {
            main.SetActive(value);
        }
    }
}
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
        [SerializeField] private Image healthBar;
        [SerializeField] private TMP_Text commandPointField;


        private void Awake()
        {
            unit.OnUnitUsedCommandPoint += OnUnitUsedCommandPoint;
            
            health.OnHealthChanged += OnHealthChanged;
            health.OnDead += OnDead;
        }

        private void OnDestroy()
        {
            unit.OnUnitUsedCommandPoint -= OnUnitUsedCommandPoint;
            
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDead -= OnDead;
        }


        private void OnUnitUsedCommandPoint()
        {
            SetCommandPoint(unit.CommandPoint);
        }
        
        private void OnHealthChanged(Health.HealthChangedArgs args)
        {
            SetHealthBarFillAmount(args.healthNormalized);
        }
        
        private void OnDead()
        {
            SetActive(false);
        }
        
        
        private void SetCommandPoint(int value)
        {
            commandPointField.text = value.ToString();
        }

        private void SetHealthBarFillAmount(float value)
        {
            healthBar.fillAmount = value;
        }

        private void SetActive(bool value)
        {
            main.SetActive(value);
        }
    }
}
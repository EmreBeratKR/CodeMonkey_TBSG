using EmreBeratKR.ServiceLocator;
using TMPro;
using UnitSystem;
using UnityEngine;

namespace UI
{
    public class UnitCommanderUI : MonoBehaviour
    {
        [SerializeField] private GameObject commandsPanel;
        [SerializeField] private GameObject busyPanel;
        [SerializeField] private GameObject waitForYourTurnPanel;
        [SerializeField] private TMP_Text commandPointField;
        
        
        private CommandButtonUI[] m_CommandButtons;


        private void Awake()
        {
            m_CommandButtons = GetComponentsInChildren<CommandButtonUI>(true);
            
            UnitCommander.OnSelectedUnitChanged += UnitCommander_OnSelectedUnitChanged;
            UnitCommander.OnBusyChanged += UnitCommander_OnBusyChanged;
            
            Unit.OnAnyUnitUsedCommandPoint += OnAnyUnitUsedCommandPoint;
            
            TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
        }

        private void OnDestroy()
        {
            UnitCommander.OnSelectedUnitChanged -= UnitCommander_OnSelectedUnitChanged;
            UnitCommander.OnBusyChanged -= UnitCommander_OnBusyChanged;
            
            Unit.OnAnyUnitUsedCommandPoint -= OnAnyUnitUsedCommandPoint;
            
            TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
        }

        private void UnitCommander_OnSelectedUnitChanged(UnitCommander.SelectedUnitChangedArgs args)
        {
            SetBusyVisual(false);
            SetCommandPoint(args.unit.CommandPoint);
            
            var commands = args.unit.GetAllCommands();
            
            for (var i = 0; i < m_CommandButtons.Length; i++)
            {
                if (i >= commands.Count)
                {
                    m_CommandButtons[i].ClearCommand();
                    continue;
                }
                
                m_CommandButtons[i].SetCommand(commands[i]);
            }
        }
        
        private void UnitCommander_OnBusyChanged(UnitCommander.BusyChangedArgs args)
        {
            SetBusyVisual(args.isBusy);
        }
        
        private void OnAnyUnitUsedCommandPoint(Unit.UnitUsedCommandPointArgs args)
        {
            var unitCommander = GetUnitCommander();
            
            if (!unitCommander.SelectedUnit) return;
            
            SetCommandPoint(args.unit.CommandPoint);

            foreach (var commandButton in m_CommandButtons)
            {
                commandButton.UpdateInteractable();
            }
        }
        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            SetWaitForYourTurnVisual(args.team != TeamType.Player);
        }


        private void SetBusyVisual(bool value)
        {
            busyPanel.SetActive(value);
            commandsPanel.SetActive(!value);
        }

        private void SetWaitForYourTurnVisual(bool value)
        {
            waitForYourTurnPanel.SetActive(value);
            commandsPanel.SetActive(!value);
        }

        private void SetCommandPoint(int value)
        {
            commandPointField.text = $"Command Count: {value}";
        }


        private static UnitCommander GetUnitCommander()
        {
            return ServiceLocator.Get<UnitCommander>();
        }
    }
}
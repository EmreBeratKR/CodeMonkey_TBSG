using System.Collections.Generic;
using TMPro;
using UnitSystem;
using UnityEngine;

namespace UI
{
    public class UnitCommanderUI : MonoBehaviour
    {
        [SerializeField] private CommandButtonUI commandButtonPrefab;
        [SerializeField] private Transform commandButtonsParent;
        [SerializeField] private GameObject commandsPanel;
        [SerializeField] private GameObject busyPanel;
        [SerializeField] private GameObject waitForYourTurnPanel;
        [SerializeField] private TMP_Text commandPointField;
        
        
        private readonly List<CommandButtonUI> m_CommandButtons = new();


        private void Awake()
        {
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

            if (!args.unit)
            {
                SetCommandPointAsUnknown();
                HideAllCommandButtons();
                return;
            }
            
            SetCommandPoint(args.unit.CommandPoint);
            
            HideAllCommandButtons();
            var commands = args.unit.GetAllCommands();
            
            for (var i = 0; i < commands.Count; i++)
            {
                if (i >= m_CommandButtons.Count)
                {
                    SpawnCommandButton();
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
            if (!UnitCommander.GetSelectedUnit()) return;
            
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


        private void SpawnCommandButton()
        {
            var commandButton = Instantiate(commandButtonPrefab, commandButtonsParent);
            m_CommandButtons.Add(commandButton);
        }

        private void HideAllCommandButtons()
        {
            foreach (var commandButton in m_CommandButtons)
            {
                commandButton.Hide();
            }
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

        private void SetCommandPointAsUnknown()
        {
            commandPointField.text = $"Command Count: ?";
        }
    }
}
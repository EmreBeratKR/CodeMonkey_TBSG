using UnitSystem;
using UnityEngine;

namespace UI
{
    public class UnitCommanderUI : MonoBehaviour
    {
        [SerializeField] private GameObject commandButtonsPanel;
        [SerializeField] private GameObject busyPanel;
        
        
        private CommandButtonUI[] m_CommandButtons;


        private void Awake()
        {
            m_CommandButtons = GetComponentsInChildren<CommandButtonUI>(true);
            
            UnitCommander.OnSelectedUnitChanged += UnitCommander_OnSelectedUnitChanged;
            UnitCommander.OnBusyChanged += UnitCommander_OnBusyChanged;
        }

        private void OnDestroy()
        {
            UnitCommander.OnSelectedUnitChanged -= UnitCommander_OnSelectedUnitChanged;
            UnitCommander.OnBusyChanged -= UnitCommander_OnBusyChanged;
        }

        private void UnitCommander_OnSelectedUnitChanged(UnitCommander.SelectedUnitChangedArgs args)
        {
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


        private void SetBusyVisual(bool value)
        {
            busyPanel.SetActive(value);
            commandButtonsPanel.SetActive(!value);
        }
    }
}
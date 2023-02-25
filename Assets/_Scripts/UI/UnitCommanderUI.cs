using UnitSystem;
using UnityEngine;

namespace UI
{
    public class UnitCommanderUI : MonoBehaviour
    {
        private CommandButtonUI[] m_CommandButtons;


        private void Awake()
        {
            m_CommandButtons = GetComponentsInChildren<CommandButtonUI>(true);
            
            UnitCommander.OnSelectedUnitChanged += OnSelectedUnitChanged;
        }

        private void OnDestroy()
        {
            UnitCommander.OnSelectedUnitChanged -= OnSelectedUnitChanged;
        }

        private void OnSelectedUnitChanged(UnitCommander.SelectedUnitChangedArgs args)
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
    }
}
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using TMPro;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CommandButtonUI : MonoBehaviour
    {
        [SerializeField] private GameObject selectedVisual;
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private Button button;


        private BaseCommand m_Command;
        

        private void Awake()
        {
            button.onClick.AddListener(OnClicked);
            UnitCommander.OnSelectedCommandChanged += OnSelectedCommandChanged;
        }

        private void OnSelectedCommandChanged(UnitCommander.SelectedCommandChangedArgs args)
        {
            var isSelected = m_Command == args.command;
            SetActiveSelectedVisual(isSelected);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClicked);
        }


        private void OnClicked()
        {
            var unitCommander = GetUnitCommander();
            unitCommander.SetSelectedCommand(m_Command);
        }


        public void SetCommand(BaseCommand command)
        {
            m_Command = command;
            gameObject.SetActive(true);
            nameField.text = command.GetName();
        }

        public void ClearCommand()
        {
            m_Command = null;
            gameObject.SetActive(false);
        }


        private void SetActiveSelectedVisual(bool value)
        {
            selectedVisual.SetActive(value);
        }
        
        
        private static UnitCommander GetUnitCommander()
        {
            return ServiceLocator.Get<UnitCommander>();
        }
    }
}
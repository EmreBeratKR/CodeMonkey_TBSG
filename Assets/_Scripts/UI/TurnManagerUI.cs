using TMPro;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TurnManagerUI : MonoBehaviour
    {
        [SerializeField] private Button nextTurnButton;
        [SerializeField] private TMP_Text turnField;


        private void Awake()
        {
            nextTurnButton.onClick.AddListener(OnClickedNextTurn);
            
            TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
            
            UnitCommander.OnBusyChanged += UnitCommander_OnBusyChanged;
        }

        private void OnDestroy()
        {
            nextTurnButton.onClick.RemoveListener(OnClickedNextTurn);
            
            TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
            
            UnitCommander.OnBusyChanged -= UnitCommander_OnBusyChanged;
        }


        private void OnClickedNextTurn()
        {
            TurnManager.NextTurn();
        }
        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            SetTurnField(args.turn);
            SetActiveNextTurnButton(args.team == TeamType.Player);
        }
        
        private void UnitCommander_OnBusyChanged(UnitCommander.BusyChangedArgs args)
        {
            SetActiveNextTurnButton(!args.isBusy);
        }
        
        
        private void SetTurnField(int turn)
        {
            turnField.text = $"Turn {turn}";
        }

        private void SetActiveNextTurnButton(bool value)
        {
            nextTurnButton.gameObject.SetActive(value);
        }
    }
}
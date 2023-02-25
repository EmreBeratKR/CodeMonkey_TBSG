using EmreBeratKR.ServiceLocator;
using TMPro;
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
        }

        private void OnDestroy()
        {
            nextTurnButton.onClick.RemoveListener(OnClickedNextTurn);
            
            TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
        }


        private void OnClickedNextTurn()
        {
            var turnManager = GetTurnManager();
            turnManager.NextTurn();
        }
        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            SetTurnField(args.turn);
        }


        private void SetTurnField(int turn)
        {
            turnField.text = $"Turn {turn}";
        }


        private static TurnManager GetTurnManager()
        {
            return ServiceLocator.Get<TurnManager>();
        }
    }
}
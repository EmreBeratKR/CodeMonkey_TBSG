using System;
using EmreBeratKR.ServiceLocator;

public class TurnManager : ServiceBehaviour
{
    public static event Action<TurnChangedArgs> OnTurnChanged;
    public struct TurnChangedArgs
    {
        public int turn;
    }
    
    
    private int m_Turn = 1;


    private void Start()
    {
        OnTurnChanged?.Invoke(new TurnChangedArgs
        {
            turn = m_Turn
        });
    }

    public void NextTurn()
    {
        m_Turn += 1;
        OnTurnChanged?.Invoke(new TurnChangedArgs
        {
            turn = m_Turn
        });
    }
}
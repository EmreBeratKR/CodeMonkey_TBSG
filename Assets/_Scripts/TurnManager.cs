using System;
using EmreBeratKR.ServiceLocator;

public class TurnManager : ServiceBehaviour
{
    public static event Action<TurnChangedArgs> OnTurnChanged;
    public struct TurnChangedArgs
    {
        public int turn;
    }
    
    
    private int m_Turn;


    private void Start()
    {
        SetTurn(1);
    }

    public void NextTurn()
    {
        SetTurn(m_Turn + 1);
    }


    private void SetTurn(int value)
    {
        m_Turn = value;
        OnTurnChanged?.Invoke(new TurnChangedArgs
        {
            turn = value
        });
    }
}
using System;
using EmreBeratKR.ServiceLocator;

public class TurnManager : ServiceBehaviour
{
    private static readonly int TeamCount = Enum.GetValues(typeof(TeamType)).Length;
    
    
    public static event Action<TurnChangedArgs> OnTurnChanged;
    public struct TurnChangedArgs
    {
        public int turn;
        public TeamType team;
    }
    
    
    private int m_Turn;


    private void Start()
    {
        SetTurn(1);
    }
    

    private void SetTurn(int value)
    {
        m_Turn = value;
        OnTurnChanged?.Invoke(new TurnChangedArgs
        {
            turn = value,
            team = GetCurrentTeam()
        });
    }
    
    private TeamType GetCurrentTeam()
    {
        return (TeamType) ((m_Turn - 1) % TeamCount);
    }

    
    public static void NextTurn()
    {
        var instance = GetInstance();
        instance.SetTurn(instance.m_Turn + 1);
    }
    
    
    private static TurnManager GetInstance()
    {
        return ServiceLocator.Get<TurnManager>();
    }
}
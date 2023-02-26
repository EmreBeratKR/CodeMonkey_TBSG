using System;
using EmreBeratKR.ServiceLocator;
using UnityEngine;

public class EnemyAI : ServiceBehaviour
{
    private bool m_IsActive;
    private float m_Timer;


    private void Awake()
    {
        TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
    }

    private void Update()
    {
        if (!m_IsActive) return;
        
        TickTimer();
    }


    private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
    {
        if (args.team != TeamType.Enemy) return;

        m_Timer = 1f;
        m_IsActive = true;
    }


    private void OnTimerDone()
    {
        var turnManager = GetTurnManager();
        turnManager.NextTurn();
        m_IsActive = false;
    }
    
    private void TickTimer()
    {
        m_Timer -= Time.deltaTime;
        
        if (m_Timer > 0f) return;
        
        OnTimerDone();
    }


    private static TurnManager GetTurnManager()
    {
        return ServiceLocator.Get<TurnManager>();
    }
}
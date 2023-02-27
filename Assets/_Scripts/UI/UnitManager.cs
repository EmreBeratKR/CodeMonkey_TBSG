using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using UnitSystem;

namespace UI
{
    public class UnitManager : ServiceBehaviour
    {
        private readonly Dictionary<TeamType, List<Unit>> m_UnitTeams = new();
        private readonly List<Unit> m_AllUnits = new();


        private void Awake()
        {
            Unit.OnAnyUnitSpawned += OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += OnAnyUnitDead;
        }

        private void OnDestroy()
        {
            Unit.OnAnyUnitSpawned -= OnAnyUnitSpawned;
            Unit.OnAnyUnitDead -= OnAnyUnitDead;
        }

        
        private void OnAnyUnitSpawned(Unit unit)
        {
            AddUnit(unit);
        }

        private void OnAnyUnitDead(Unit unit)
        {
            RemoveUnit(unit);
        }


        private void AddUnit(Unit unit)
        {
            m_AllUnits.Add(unit);
            
            var teamType = unit.GetTeamType();
            
            if (!DoesTeamExists(teamType))
            {
                m_UnitTeams[teamType] = new List<Unit>();
            }
            
            m_UnitTeams[teamType].Add(unit);
        }

        private void RemoveUnit(Unit unit)
        {
            m_AllUnits.Remove(unit);
            
            var teamType = unit.GetTeamType();
            
            if (!DoesTeamExists(teamType)) return;

            m_UnitTeams[teamType].Remove(unit);
        }

        private bool DoesTeamExists(TeamType teamType)
        {
            return m_UnitTeams.ContainsKey(teamType);
        }


        public static IReadOnlyList<Unit> GetAllUnits()
        {
            return GetInstance().m_AllUnits;
        }

        public static IReadOnlyList<Unit> GetUnitsWithTeamType(TeamType teamType)
        {
            return GetInstance().m_UnitTeams[teamType];
        }


        private static UnitManager GetInstance()
        {
            return ServiceLocator.Get<UnitManager>();
        }
    }
}
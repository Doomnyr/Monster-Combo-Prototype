using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TeamSlotConfiguration
{
    public MonsterDefinition MonsterDef;
    public GridPosition GridPos; // Column (0-1), Row (0-2)
}



public class CombatLoader : MonoBehaviour
{

    [Header("Dependencies")]
    [Tooltip("The engine that will execute the combat rules once data is ready.")]
    [SerializeField] private CombatManager _combatManager;
    
    [Header("Team Configurations")]
    [SerializeField] private List<TeamSlotConfiguration> _playerTeam; 
    [SerializeField] private List<TeamSlotConfiguration> _enemyTeam;
    
    void Start()
    {
        LoadMonsterData();
    }

    void LoadMonsterData()
    {
        List<MonsterInstance> activePlayerTeam = HydrateTeam(_playerTeam, CombatTeam.Player);
        List<MonsterInstance> activeEnemyTeam = HydrateTeam(_enemyTeam, CombatTeam.Enemy);

        _combatManager?.PrepareMatch(activePlayerTeam, activeEnemyTeam);

    List<MonsterInstance> HydrateTeam(List<TeamSlotConfiguration> setupSlots, CombatTeam team)
    {
        List<MonsterInstance> instantiatedTeam = new List<MonsterInstance>();

        foreach (var slot in setupSlots)
        {
            if (slot.MonsterDef == null) continue;

            MonsterInstance instance = new MonsterInstance(slot.MonsterDef, team, slot.GridPos);
            instantiatedTeam.Add(instance);
        }

        return instantiatedTeam;
    }
    }
}

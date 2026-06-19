using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    private List<MonsterInstance> _playerTeam = new List<MonsterInstance>();
    private List<MonsterInstance> _enemyTeam = new List<MonsterInstance>();

    private List<MonsterInstance> _turnTimeline = new List<MonsterInstance>();
    private int _currentTimelineIndex = 0;

    public void PrepareMatch(List<MonsterInstance> readyPlayerTeam, List<MonsterInstance> readyEnemyTeam)
    {
        _playerTeam = readyPlayerTeam;
        _enemyTeam = readyEnemyTeam;

        Debug.Log("CombatManager: Teams received. Generating timeline...");
        
        GenerateTurnTimeline();
        StartMatch();
        PrintMonsterInstances();
    }

    private void GenerateTurnTimeline()
    {
        _turnTimeline.Clear();
        _currentTimelineIndex = 0;

        _turnTimeline.AddRange(_playerTeam);
        _turnTimeline.AddRange(_enemyTeam);


        _turnTimeline = _turnTimeline.OrderByDescending(monster => monster.MonsterDef.BaseStats.speed).ToList();
    }

    private void StartMatch()
    {
        Debug.Log("CombatManager: Match started!");
    }

    public void PrintMonsterInstances()
    {
        List<MonsterInstance> allMonsters = _turnTimeline;

        foreach (var monster in allMonsters)
        {
            // Gather references for cleaner string syntax
            var stats = monster.MonsterDef.BaseStats;

            string monsterProfile = 
                $"====== [ {monster.MonsterDef.MonsterName} ] ======\n" +
                $"• Instance ID: {monster.InstanceId}\n" +
                $"• Allegiance:  {monster.Team}\n" +
                $"• Grid Slot:   [Column {monster.Position.Column}, Row {monster.Position.Row}]\n" +
                $"• Traits:      Race: {monster.MonsterDef.Race} | Element: {monster.MonsterDef.Element}\n" +
                $"--------------------------------------------------\n" +
                $"[Current Vitals]\n" +
                $"  - HP:   {monster.CurrentHP} / {stats.maxHP}\n" +
                $"  - Mana: {monster.CurrentMana} / {stats.maxMana}\n" +
                $"[Base Parameters]\n" +
                $"  - ATK:  {stats.attack}  |  DEF:  {stats.defense}\n" +
                $"  - INT:  {stats.intelligence}  |  SPD:  {stats.speed}\n" +
                $"  - CRIT: {stats.critChance * 100}% |  MULT: {stats.critDamageMult}x\n" +
                $"==================================================";

            Debug.Log(monsterProfile);
        }
    }
}
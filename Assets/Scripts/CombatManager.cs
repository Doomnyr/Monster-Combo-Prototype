using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public List<MonsterInstance> PlayerTeam { get; private set; } = new List<MonsterInstance>();
    public List<MonsterInstance> EnemyTeam { get; private set; } = new List<MonsterInstance>();

    private List<MonsterInstance> _turnTimeline = new List<MonsterInstance>();
    private Queue<MonsterInstance> _turnQueue = new Queue<MonsterInstance>();


    public event Action OnCombatDataReady;

    public void PrepareMatch(List<MonsterInstance> readyPlayerTeam, List<MonsterInstance> readyEnemyTeam)
    {
        PlayerTeam = readyPlayerTeam;
        EnemyTeam = readyEnemyTeam;

        Debug.Log("CombatManager: Teams received. Generating timeline...");
        
        StartMatch();
        BuildTurnOrder();

        PrintMonsterInstances();
        OnCombatDataReady?.Invoke();
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

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ExecuteNextTurn();
        }
    }

    private void BuildTurnOrder()
    {
        _turnQueue.Clear();

        // Alternating entries between teams for a simple turn order demonstration
        int maxCount = Mathf.Max(PlayerTeam.Count, EnemyTeam.Count);
        for (int i = 0; i < maxCount; i++)
        {
            if (i < PlayerTeam.Count) _turnQueue.Enqueue(PlayerTeam[i]);
            if (i < EnemyTeam.Count) _turnQueue.Enqueue(EnemyTeam[i]);
        }
        
        Debug.Log($"Turn queue built with {_turnQueue.Count} monsters. Press SPACE to execute turns.");
    }

    private void ExecuteNextTurn()
    {
        if (_turnQueue.Count == 0) return;

        // 1. Get the monster whose turn it currently is
        MonsterInstance activeMonster = _turnQueue.Dequeue();

        // Skip defeated monsters
        if (activeMonster.IsDefeated)
        {
            ExecuteNextTurn();
            return;
        }

        // 2. Grab its first skill (the Basic Attack we added to the pool)
        if (activeMonster.MonsterDef.CommandPriorityList.Count == 0)
        {
            Debug.LogWarning($"{activeMonster.MonsterDef.MonsterName} has no skills assigned!");
            _turnQueue.Enqueue(activeMonster); // Put back in queue
            return;
        }
        SkillDefinition chosenSkill = activeMonster.MonsterDef.CommandPriorityList[0];

        // 3. Simple Target Selection: Attack the first living enemy on the opposing team
        MonsterInstance target = GetDefaultTarget(activeMonster);

        if (target != null)
        {
            Debug.Log($"[TURN] {activeMonster.MonsterDef.MonsterName} is using {chosenSkill.SkillName} on {target.MonsterDef.MonsterName}!");

            // 4. Run all the modular data-driven effects on the asset!
            foreach (SkillEffect effect in chosenSkill.ExecutionEffects)
            {
                effect.Execute(activeMonster, target);
            }
        }

        // 5. Put the monster back at the end of the line for its next turn
        if (!activeMonster.IsDefeated)
        {
            _turnQueue.Enqueue(activeMonster);
        }
    }

    private MonsterInstance GetDefaultTarget(MonsterInstance caster)
    {
        // If a player monster is acting, target the first alive enemy. Otherwise, target the first alive player.
        List<MonsterInstance> opposingTeam = (caster.Team == CombatTeam.Player) ? EnemyTeam : PlayerTeam;

        foreach (var monster in opposingTeam)
        {
            if (!monster.IsDefeated) return monster;
        }
        return null; // No alive targets left
    }
}
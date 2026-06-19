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
                $"• Grid Slot:   [Column {monster.GridPosition.Column}, Row {monster.GridPosition.Row}]\n" +
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

    // 2. Grab its first skill (the Basic Attack)
    if (activeMonster.MonsterDef.CommandPriorityList.Count == 0)
    {
        Debug.LogWarning($"{activeMonster.MonsterDef.MonsterName} has no skills assigned!");
        _turnQueue.Enqueue(activeMonster); 
        return;
    }
    SkillDefinition chosenSkill = activeMonster.MonsterDef.CommandPriorityList[0];

    Debug.Log($"[TURN] {activeMonster.MonsterDef.MonsterName} is using {chosenSkill.SkillName}!");

    // 3. Loop through effects and let each effect pull its own distinct target list
    foreach (SkillEffect effect in chosenSkill.ExecutionEffects)
    {
        // Dynamically resolve targets specifically for this individual effect block
        List<MonsterInstance> targets = ResolveTargets(activeMonster, effect.TargetScope);

        if (targets.Count > 0)
        {
            effect.Execute(activeMonster, targets);
        }
    }

    // 4. Put the monster back at the end of the line for its next turn
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

    private List<MonsterInstance> ResolveTargets(MonsterInstance caster, TargetScope scope)
    {
        List<MonsterInstance> targets = new List<MonsterInstance>();
        
        List<MonsterInstance> allies = (caster.Team == CombatTeam.Player) ? PlayerTeam : EnemyTeam;
        List<MonsterInstance> enemies = (caster.Team == CombatTeam.Player) ? EnemyTeam : PlayerTeam;

        List<MonsterInstance> livingAllies = allies.FindAll(m => !m.IsDefeated);
        List<MonsterInstance> livingEnemies = enemies.FindAll(m => !m.IsDefeated);

        switch (scope)
        {
            case TargetScope.Caster:
                targets.Add(caster);
                break;

            case TargetScope.RandomEnemy:
                if (livingEnemies.Count > 0)
                    targets.Add(livingEnemies[UnityEngine.Random.Range(0, livingEnemies.Count)]);
                break;

            case TargetScope.AllEnemy:
                targets.AddRange(livingEnemies);
                break;

            case TargetScope.RandomFrontRowEnemy:
                List<MonsterInstance> frontEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 0);
                if (frontEnemies.Count > 0)
                    targets.Add(frontEnemies[UnityEngine.Random.Range(0, frontEnemies.Count)]);
                break;

            case TargetScope.AllFrontRowEnemy:
                targets.AddRange(livingEnemies.FindAll(m => m.GridPosition.Column == 0));
                break;

            case TargetScope.RandomBackRowEnemy:
                List<MonsterInstance> backEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 1);
                if (backEnemies.Count > 0)
                    targets.Add(backEnemies[UnityEngine.Random.Range(0, backEnemies.Count)]);
                break;

            case TargetScope.AllBackRowEnemy:
                targets.AddRange(livingEnemies.FindAll(m => m.GridPosition.Column == 1));
                break;

            case TargetScope.TopLaneEnemy:
                targets.AddRange(livingEnemies.FindAll(m => m.GridPosition.Row == 0));
                break;

            case TargetScope.MidLaneEnemy:
                targets.AddRange(livingEnemies.FindAll(m => m.GridPosition.Row == 1));
                break;

            case TargetScope.BotLaneEnemy:
                targets.AddRange(livingEnemies.FindAll(m => m.GridPosition.Row == 2));
                break;

            case TargetScope.RandomAlly:
                if (livingAllies.Count > 0)
                    targets.Add(livingAllies[UnityEngine.Random.Range(0, livingAllies.Count)]);
                break;

            case TargetScope.AllAlly:
                targets.AddRange(livingAllies);
                break;

            case TargetScope.RandomFrontRowAlly:
                List<MonsterInstance> frontAllies = livingAllies.FindAll(m => m.GridPosition.Column == 0);
                if (frontAllies.Count > 0)
                    targets.Add(frontAllies[UnityEngine.Random.Range(0, frontAllies.Count)]);
                break;

            case TargetScope.AllFrontRowAlly:
                targets.AddRange(livingAllies.FindAll(m => m.GridPosition.Column == 0));
                break;

            case TargetScope.RandomBackRowAlly:
                List<MonsterInstance> backAllies = livingAllies.FindAll(m => m.GridPosition.Column == 1);
                if (backAllies.Count > 0)
                    targets.Add(backAllies[UnityEngine.Random.Range(0, backAllies.Count)]);
                break;

            case TargetScope.AllBackRowAlly:
                targets.AddRange(livingAllies.FindAll(m => m.GridPosition.Column == 1));
                break;

            case TargetScope.TopLaneAlly:
                targets.AddRange(livingAllies.FindAll(m => m.GridPosition.Row == 0));
                break;

            case TargetScope.MidLaneAlly:
                targets.AddRange(livingAllies.FindAll(m => m.GridPosition.Row == 1));
                break;

            case TargetScope.BotLaneAlly:
                targets.AddRange(livingAllies.FindAll(m => m.GridPosition.Row == 2));
                break;

            case TargetScope.RandomFrontRowEnemyThenRandomBackRowEnemy:
                List<MonsterInstance> preferredFrontEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 0);
                if (preferredFrontEnemies.Count > 0)
                {
                    targets.Add(preferredFrontEnemies[UnityEngine.Random.Range(0, preferredFrontEnemies.Count)]);
                }
                else // Fallback to Back Row if Front Row is empty
                {
                    List<MonsterInstance> fallbackBackEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 1);
                    if (fallbackBackEnemies.Count > 0)
                        targets.Add(fallbackBackEnemies[UnityEngine.Random.Range(0, fallbackBackEnemies.Count)]);
                }
                break;

            case TargetScope.RandomBackRowEnemyThenRandomFrontRowEnemy:
                List<MonsterInstance> preferredBackEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 1);
                if (preferredBackEnemies.Count > 0)
                {
                    targets.Add(preferredBackEnemies[UnityEngine.Random.Range(0, preferredBackEnemies.Count)]);
                }
                else // Fallback to Front Row if Back Row is empty
                {
                    List<MonsterInstance> fallbackFrontEnemies = livingEnemies.FindAll(m => m.GridPosition.Column == 0);
                    if (fallbackFrontEnemies.Count > 0)
                        targets.Add(fallbackFrontEnemies[UnityEngine.Random.Range(0, fallbackFrontEnemies.Count)]);
                }
                break;

            case TargetScope.RandomFrontRowAllyThenRandomBackRowAlly:
                List<MonsterInstance> preferredFrontAllies = livingAllies.FindAll(m => m.GridPosition.Column == 0);
                if (preferredFrontAllies.Count > 0)
                {
                    targets.Add(preferredFrontAllies[UnityEngine.Random.Range(0, preferredFrontAllies.Count)]);
                }
                else // Fallback to Back Row
                {
                    List<MonsterInstance> fallbackBackAllies = livingAllies.FindAll(m => m.GridPosition.Column == 1);
                    if (fallbackBackAllies.Count > 0)
                        targets.Add(fallbackBackAllies[UnityEngine.Random.Range(0, fallbackBackAllies.Count)]);
                }
                break;

            case TargetScope.RandomBackRowAllyThenRandomFrontRowAlly:
                List<MonsterInstance> preferredBackAllies = livingAllies.FindAll(m => m.GridPosition.Column == 1);
                if (preferredBackAllies.Count > 0)
                {
                    targets.Add(preferredBackAllies[UnityEngine.Random.Range(0, preferredBackAllies.Count)]);
                }
                else // Fallback to Front Row
                {
                    List<MonsterInstance> fallbackFrontAllies = livingAllies.FindAll(m => m.GridPosition.Column == 0);
                    if (fallbackFrontAllies.Count > 0)
                        targets.Add(fallbackFrontAllies[UnityEngine.Random.Range(0, fallbackFrontAllies.Count)]);
                }
                break;
        }

        return targets;
    }
}
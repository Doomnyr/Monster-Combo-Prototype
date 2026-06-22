using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public List<MonsterInstance> PlayerTeam { get; private set; } = new List<MonsterInstance>();
    public List<MonsterInstance> EnemyTeam { get; private set; } = new List<MonsterInstance>();

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
        Queue<MonsterInstance> allMonsters = _turnQueue;

        foreach (var monster in allMonsters)
        {
            var baseStats = monster.MonsterDef.BaseStats;

            // 1. Build the Buffs/Debuffs display string dynamically
            string buffString = "";
            if (monster.ActiveBuffs == null || monster.ActiveBuffs.Count == 0)
            {
                buffString = "  (None)";
            }
            else
            {
                foreach (var buff in monster.ActiveBuffs)
                {
                    string durationText = buff.RemainingDuration == -1 ? "Permanent" : $"{buff.RemainingDuration} turns left";
                    string typeTag = buff.BuffDef.isDebuff ? "[DEBUFF]" : "[BUFF]";
                    
                    buffString += $"  • {typeTag} {buff.BuffDef.buffName} x{buff.CurrentStacks} ({durationText})\n";
                }
                // Trim trailing newline for clean formatting
                buffString = buffString.TrimEnd('\n');
            }

            // 2. Build the final profile layout
            // Note: Changed stats.strength/defense to monster.Attack/Defense to see buff impacts!
            string monsterProfile = 
                $"====== [ {monster.MonsterDef.MonsterName} ] ======\n" +
                $"• Instance ID: {monster.InstanceId}\n" +
                $"• Allegiance:  {monster.Team}\n" +
                $"• Grid Slot:   [Column {monster.GridPosition.Column}, Row {monster.GridPosition.Row}]\n" +
                $"• Traits:      Race: {monster.MonsterDef.Race} | Element: {monster.MonsterDef.Element}\n" +
                $"--------------------------------------------------\n" +
                $"[Active Buffs / Debuffs]\n" +
                $"{buffString}\n" +
                $"--------------------------------------------------\n" +
                $"[Current Vitals]\n" +
                $"  - HP:   {monster.CurrentHP} / {baseStats.maxHP}\n" +
                $"  - Mana: {monster.CurrentMana} / {baseStats.maxMana}\n" +
                $"[Combat Parameters (Modified)]\n" +
                $"  - ATK:  {monster.Strength} (Base: {baseStats.strength})  |  DEF:  {monster.Defense} (Base: {baseStats.defense})\n" +
                $"  - INT:  {baseStats.intelligence}  |  SPD:  {baseStats.speed}\n" +
                $"  - CRIT: {baseStats.critChance * 100}% |  MULT: {baseStats.critDamageMult}x\n" +
                $"==================================================";

            Debug.Log(monsterProfile);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ExecuteNextTurn();
            PrintMonsterInstances();
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

        MonsterInstance activeMonster = _turnQueue.Dequeue();

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
        
        SkillDefinitionSO chosenSkill = activeMonster.MonsterDef.CommandPriorityList[0];
        Debug.Log($"--- [TURN] {activeMonster.MonsterDef.MonsterName} is using {chosenSkill.SkillName}! ---");

        // Combine teams into one master list to pass to our TargetFinders
        List<MonsterInstance> battlefield = new List<MonsterInstance>();
        battlefield.AddRange(PlayerTeam);
        battlefield.AddRange(EnemyTeam);

        // 3. Loop through the paired actions (The Magic Happens Here)
        foreach (SkillAction action in chosenSkill.Actions)
        {
            // Safety check in case you forgot to plug an asset into the inspector slot
            if (action.targetFinder == null || action.executionEffect == null)
            {
                Debug.LogWarning($"A SkillAction in {chosenSkill.SkillName} is missing a finder or effect asset!");
                continue;
            }

            // A. Ask the Radar (TargetFinder) who to hit
            List<MonsterInstance> targets = action.targetFinder.FindTargets(activeMonster, battlefield);

            // B. Drop the Payload (SkillEffect) on every target found
            foreach (MonsterInstance target in targets)
            {
                action.executionEffect.Apply(activeMonster, target);
            }
        }

        // 4. Put the monster back at the end of the line for its next turn
        if (!activeMonster.IsDefeated)
        {
            _turnQueue.Enqueue(activeMonster);
        }
    }

}
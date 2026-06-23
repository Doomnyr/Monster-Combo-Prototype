using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public List<MonsterInstance> PlayerTeam { get; private set; } = new List<MonsterInstance>();
    public List<MonsterInstance> EnemyTeam { get; private set; } = new List<MonsterInstance>();

    private Queue<MonsterInstance> _turnQueue = new Queue<MonsterInstance>();

    public event Action OnCombatDataReady;
    public event Action<int> OnDamageTaken;
    public event Action<int> OnHealed;
    public event Action<BuffDefinitionSO, int> OnBuffApplied;
    public event Action<BuffDefinitionSO> OnBuffRemoved;

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
        foreach (var monster in _turnQueue)
        {
            var baseStats = monster.MonsterDef.BaseStats;
            var buffs = monster.ActiveBuffs;

            string buffString;
            if (buffs.Count == 0)
            {
                buffString = "  (None)";
            }
            else
            {
                buffString = string.Empty;
                foreach (var buff in buffs)
                {
                    string durationText = buff.RemainingDuration == -1 ? "Permanent" : $"{buff.RemainingDuration} turns left";
                    string typeTag = buff.BuffDef.isDebuff ? "[DEBUFF]" : "[BUFF]";
                    buffString += $"  • {typeTag} {buff.BuffDef.buffName} x{buff.CurrentStacks} ({durationText})\n";
                }
                buffString = buffString.TrimEnd('\n');
            }

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
                $"  - HP:   {monster.CurrentHP} / {monster.MaxHP}\n" +
                $"  - Mana: {monster.CurrentMana} / {monster.MaxMana}\n" +
                $"[Combat Parameters (Modified)]\n" +
                $"  - ATK:  {monster.Strength} (Base: {baseStats.strength})  |  DEF:  {monster.Defense} (Base: {baseStats.defense})\n" +
                $"  - INT:  {baseStats.intelligence}  |  SPD:  {monster.Speed}\n" +
                $"  - CRIT: {monster.CritChance * 100}% |  MULT: {monster.CritDamageMult}x\n" +
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
        while (_turnQueue.Count > 0 && _turnQueue.Peek().IsDefeated)
        {
            _turnQueue.Dequeue();
        }

        if (_turnQueue.Count == 0) return;

        MonsterInstance activeMonster = _turnQueue.Dequeue();

        // 1. Gather all living units on the battlefield for target finding
        List<MonsterInstance> battlefield = new List<MonsterInstance>();
        battlefield.AddRange(PlayerTeam);
        battlefield.AddRange(EnemyTeam);

        // 2. TRIGGER: OnTurnStart Buffs (e.g., Regen heals here!)
        EvaluateBuffTriggers(activeMonster, BuffTriggerTime.OnTurnStart, battlefield);

        if (activeMonster.IsDefeated) return; // Guard in case turn-start debuffs fainted them

        // 3. Grab and use its first assigned skill
        if (activeMonster.MonsterDef.CommandPriorityList.Count == 0)
        {
            Debug.LogWarning($"{activeMonster.MonsterDef.MonsterName} has no skills assigned!");
            _turnQueue.Enqueue(activeMonster); 
            return;
        }
        
        SkillDefinitionSO chosenSkill = activeMonster.MonsterDef.CommandPriorityList[0];
        Debug.Log($"--- [TURN] {activeMonster.MonsterDef.MonsterName} is using {chosenSkill.SkillName}! ---");

        foreach (SkillAction action in chosenSkill.Actions)
        {
            ExecuteSkillAction(action, activeMonster, battlefield);
        }

        // 4. TRIGGER: OnTurnEnd Buffs (e.g., Poison ticks damage here!)
        EvaluateBuffTriggers(activeMonster, BuffTriggerTime.OnTurnEnd, battlefield);

        // 5. Tick down status durations at the end of the monster's turn
        activeMonster.TickBuffDurations();

        // 6. Put the monster back at the end of the line if they survived
        if (!activeMonster.IsDefeated)
        {
            _turnQueue.Enqueue(activeMonster);
        }
    }

    /// <summary>
    /// Evaluates and runs active actions registered directly on the active unit's active buffs.
    /// </summary>
    private void EvaluateBuffTriggers(MonsterInstance monster, BuffTriggerTime triggerTime, List<MonsterInstance> battlefield)
    {
        List<SkillAction> triggeredActions = monster.GetTriggeredBuffActions(triggerTime);
        if (triggeredActions.Count == 0) return;

        Debug.Log($"[BUFF TRIGGER] Evaluating {triggerTime} triggers for {monster.MonsterDef.MonsterName}...");
        foreach (SkillAction action in triggeredActions)
        {
            ExecuteSkillAction(action, monster, battlefield);
        }
    }

    /// <summary>
    /// Highly unified skill execution logic. Reused for both standard Skills AND Buff triggers!
    /// </summary>
    private void ExecuteSkillAction(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        if (action.targetFinder == null || action.executionEffect == null)
        {
            Debug.LogWarning("Skipping Action: Missing finder or effect asset!");
            return;
        }

        // A. Ask the Radar (TargetFinder) who to hit
        List<MonsterInstance> targets = action.targetFinder.FindTargets(caster, battlefield);

        // B. Drop the Payload (SkillEffect) on every target found
        foreach (MonsterInstance target in targets)
        {
            action.executionEffect.Apply(caster, target);
        }
    }
}
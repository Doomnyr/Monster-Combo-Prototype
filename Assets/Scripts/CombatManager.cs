using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public List<MonsterInstance> PlayerTeam { get; private set; } = new List<MonsterInstance>();
    public List<MonsterInstance> EnemyTeam { get; private set; } = new List<MonsterInstance>();

    private TurnManager _turnManager = new TurnManager();

    public event Action OnCombatDataReady;

    public void PrepareMatch(List<MonsterInstance> readyPlayerTeam, List<MonsterInstance> readyEnemyTeam)
    {
        PlayerTeam = readyPlayerTeam;
        EnemyTeam = readyEnemyTeam;

        StartMatch();
        
        // Use our consolidated trigger evaluator
        TriggerCombatTriggers(CombatTriggerTime.OnCombatStart);
        
        _turnManager.Initialize(PlayerTeam, EnemyTeam);
        OnCombatDataReady?.Invoke();
    }

    private void StartMatch() => Debug.Log("CombatManager: Match started!");

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ExecuteNextTurn();
        }
    }

    /// <summary>
    /// Consolidated trigger system: Polls buffs and traits for any monster on the battlefield.
    /// </summary>
    private void TriggerCombatTriggers(CombatTriggerTime triggerTime)
    {
        List<MonsterInstance> allMonsters = new List<MonsterInstance>();
        allMonsters.AddRange(PlayerTeam);
        allMonsters.AddRange(EnemyTeam);

        foreach (var monster in allMonsters)
        {
            EvaluateAllTriggers(monster, triggerTime, allMonsters);
        }
    }

    /// <summary>
    /// Runs all actions (Buffs + Traits) for a specific monster at a specific timing.
    /// </summary>
    private void EvaluateAllTriggers(MonsterInstance monster, CombatTriggerTime triggerTime, List<MonsterInstance> battlefield)
    {
        // 1. Buff Actions (Accessing via Buffs collection directly)
        var buffActions = monster.Buffs.GetTriggeredActions(triggerTime);
        foreach (var action in buffActions) ExecuteSkillAction(action, monster, battlefield);

        // 2. Trait Actions
        var traitActions = monster.Traits.GetTriggeredActions(triggerTime);
        foreach (var action in traitActions) ExecuteSkillAction(action, monster, battlefield);
    }

    private void ExecuteNextTurn()
    {
        MonsterInstance activeMonster = _turnManager.GetNextTurn();
        if (activeMonster == null) return;

        List<MonsterInstance> battlefield = new List<MonsterInstance>();
        battlefield.AddRange(PlayerTeam);
        battlefield.AddRange(EnemyTeam);

        // 1. Turn Start Triggers
        EvaluateAllTriggers(activeMonster, CombatTriggerTime.OnTurnStart, battlefield);

        if (activeMonster.IsDefeated) return;

        // 2. Execute Primary Skill
        if (activeMonster.MonsterDef.CommandPriorityList.Count > 0)
        {
            ExecuteSkill(activeMonster.MonsterDef.CommandPriorityList[0], activeMonster, battlefield);
        }

        // 3. Turn End Triggers
        EvaluateAllTriggers(activeMonster, CombatTriggerTime.OnTurnEnd, battlefield);

        // 4. Cleanup (Accessing Buffs collection directly)
        activeMonster.Buffs.TickDurations();
        _turnManager.RequeueCombatant(activeMonster);
    }

    public void ExecuteSkill(SkillDefinitionSO skill, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> lastSuccessfulTargets = new List<MonsterInstance>();

        foreach (SkillAction action in skill.Actions)
        {
            List<MonsterInstance> currentActionTargets = action.targetFinder.FindTargets(
                action, caster, battlefield, lastSuccessfulTargets
            );

            foreach (MonsterInstance target in currentActionTargets)
            {
                foreach (SkillEffectSO effect in action.executionEffect)
                
                if (target != null && target.IsAlive)
                {
                    effect.Apply(action, caster, target);
                }
            }

            if (currentActionTargets.Count > 0)
            {
                lastSuccessfulTargets = currentActionTargets;
            }
        }
    }

    private void ExecuteSkillAction(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> targets = action.targetFinder.FindTargets(action, caster, battlefield, null);
        
        foreach (MonsterInstance target in targets)
        {
            foreach (SkillEffectSO effect in action.executionEffect)
            {
                
                if (target != null && target.IsAlive)
                {
                    effect.Apply(action, caster, target);
                }
            }
        }
    }
}
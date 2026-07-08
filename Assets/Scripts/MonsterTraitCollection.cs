using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterTraitCollection
{
    private readonly List<TraitDefinitionSO> _traits;
    private MonsterInstance _owner;

    public MonsterTraitCollection(List<TraitDefinitionSO> traits)
    {
        _traits = traits ?? new List<TraitDefinitionSO>();
    }

    public void RegisterCombatEvents(CombatEventDispatcher dispatcher, MonsterInstance owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        dispatcher.TurnStarted += HandleTurnStarted;
        dispatcher.TurnEnded += HandleTurnEnded;
        dispatcher.CombatStarted += HandleCombatStarted;
    }

    private void HandleCombatStarted(CombatStartedEvent combatEvent)
    {
        if (combatEvent == null || _owner == null) return;
        ProcessTriggers(CombatTriggerTime.OnCombatStart, combatEvent.Battlefield);
    }

    private void HandleTurnStarted(CombatTurnStartedEvent turnEvent)
    {
        if (turnEvent == null || _owner == null) return;
        if (turnEvent.ActiveMonster != _owner) return;
        ProcessTriggers(CombatTriggerTime.OnTurnStart, turnEvent.Battlefield);
    }

    private void HandleTurnEnded(CombatTurnEndedEvent turnEvent)
    {
        if (turnEvent == null || _owner == null) return;
        if (turnEvent.ActiveMonster != _owner) return;
        ProcessTriggers(CombatTriggerTime.OnTurnEnd, turnEvent.Battlefield);
    }

    private void ProcessTriggers(CombatTriggerTime triggerTime, IReadOnlyList<MonsterInstance> battlefield)
    {
        var actions = GetTriggeredActions(triggerTime);
        foreach (var action in actions)
        {
            CombatActionExecutor.ExecuteSkillAction(action, _owner, new List<MonsterInstance>(battlefield));
        }
    }

    public List<SkillAction> GetTriggeredActions(CombatTriggerTime triggerTime)
    {
        List<SkillAction> actions = new List<SkillAction>();
        foreach (var trait in _traits)
        {
            foreach (var trigger in trait.triggeredActions)
            {
                if (trigger.triggerTime == triggerTime)
                {
                    actions.Add(trigger.actionToTrigger);
                }
            }
        }
        return actions;
    }
}
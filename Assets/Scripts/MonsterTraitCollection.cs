using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterTraitCollection
{
    private readonly List<TraitDefinitionSO> _traits;

    public MonsterTraitCollection(List<TraitDefinitionSO> traits)
    {
        _traits = traits ?? new List<TraitDefinitionSO>();
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
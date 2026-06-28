using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterBuffCollection
{
    private readonly List<BuffInstance> activeBuffs = new List<BuffInstance>();
    public IReadOnlyList<BuffInstance> ActiveBuffs => activeBuffs.AsReadOnly();

    public event Action OnBuffsChanged;
    public event Action<BuffDefinitionSO, int> OnBuffApplied;
    public event Action<BuffDefinitionSO> OnBuffRemoved;

    public void AddBuff(BuffDefinitionSO buffDef, int stacks)
    {
        if (buffDef == null) throw new ArgumentNullException(nameof(buffDef));

        bool isPermanent = stacks == -1;
        
        BuffInstance existingBuff = activeBuffs.Find(b => b.BuffDef == buffDef);
        if (existingBuff != null)
        {
            existingBuff.AddStacks(stacks);
        }
        else
        {
            activeBuffs.Add(new BuffInstance(buffDef, stacks, isPermanent));
        }

        OnBuffApplied?.Invoke(buffDef, stacks);
        OnBuffsChanged?.Invoke();
    }

    public void TickDurations()
    {
        foreach (var buff in activeBuffs)
        {
            buff.Tick();
        }

        if (RemoveExpiredBuffs())
        {
            OnBuffsChanged?.Invoke();
        }
    }

    public bool RemoveExpiredBuffs()
    {
        int countBefore = activeBuffs.Count;
        activeBuffs.RemoveAll(buff => buff.IsExpired);
        
        if (activeBuffs.Count < countBefore)
        {
            // Note: If you need to know exactly which was removed for events,
            // iterate and check IsExpired before removal.
            OnBuffsChanged?.Invoke();
            return true;
        }
        return false;
    }

    public List<SkillAction> GetTriggeredActions(CombatTriggerTime triggerTime)
    {
        List<SkillAction> actionsToRun = new List<SkillAction>();
        foreach (var buff in activeBuffs)
        {
            foreach (var trigger in buff.BuffDef.triggeredActions)
            {
                if (trigger.triggerTime == triggerTime)
                {
                    // Run the action once per stack, or just once? 
                    // For Poison (2% max HP per stack), the effect calculation handles multiplier,
                    // so we only need to fire the action once.
                    actionsToRun.Add(trigger.actionToTrigger);
                }
            }
        }
        return actionsToRun;
    }

    public float CalculateModifiedStat(StatType statType, float baseValue)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;
        float multiplier = 1f;

        foreach (var buff in activeBuffs)
        {
            foreach (var modifier in buff.BuffDef.statModifiers)
            {
                if (modifier.statToModify != statType) continue;

                float totalModValue = modifier.valuePerStack * buff.CurrentStacks;
                switch (modifier.modifierType)
                {
                    case ModifierType.FlatAdd:
                        flatBonus += totalModValue;
                        break;
                    case ModifierType.PercentAdd:
                        percentBonus += totalModValue;
                        break;
                    case ModifierType.PercentMultiply:
                        multiplier *= Mathf.Pow(modifier.valuePerStack, buff.CurrentStacks);
                        break;
                }
            }
        }

        float finalValue = (baseValue + flatBonus) * (1f + percentBonus) * multiplier;
        return Mathf.Max(0f, finalValue);
    }
}
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
    

    public bool AddBuff(BuffDefinitionSO buffDef, int stacks, int duration)
    {
        if (buffDef == null) throw new ArgumentNullException(nameof(buffDef));

        if (duration == 0)
        {
            return RemoveBuff(buffDef);
        }

        BuffInstance existingBuff = activeBuffs.Find(b => b.BuffDef == buffDef);
        if (existingBuff != null)
        {
            existingBuff.AddStacks(stacks);
            existingBuff.SetRemainingDuration(duration);
        }
        else
        {
            activeBuffs.Add(new BuffInstance(buffDef, stacks, duration));
        }

        OnBuffApplied?.Invoke(buffDef, stacks);
        OnBuffsChanged?.Invoke();
        return true;
    }

    public bool RemoveBuff(BuffDefinitionSO buffDef)
    {
        if (buffDef == null) return false;

        bool removed = activeBuffs.RemoveAll(buff => buff.BuffDef == buffDef) > 0;
        if (removed)
        {
            OnBuffRemoved?.Invoke(buffDef);
            OnBuffsChanged?.Invoke();
        }

        return removed;
    }

    public bool SetBuffDuration(BuffDefinitionSO buffDef, int duration)
    {
        BuffInstance buff = activeBuffs.Find(b => b.BuffDef == buffDef);
        if (buff == null) return false;

        buff.SetRemainingDuration(duration);
        if (duration == 0 || RemoveExpiredBuffs())
        {
            OnBuffsChanged?.Invoke();
        }

        return !buff.IsExpired;
    }

    public void TickDurations()
    {
        foreach (var buff in activeBuffs)
        {
            buff.DecreaseDuration();
        }

        if (RemoveExpiredBuffs())
        {
            OnBuffsChanged?.Invoke();
        }
    }

    public bool RemoveExpiredBuffs()
    {
        return activeBuffs.RemoveAll(buff => buff.IsExpired) > 0;
    }

    /// <summary>
    /// Gathers all SkillActions registered under a specific trigger window.
    /// Multiplies execution runs if a buff stacks (optional, currently runs once per trigger).
    /// </summary>
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
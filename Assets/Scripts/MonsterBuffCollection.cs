using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterBuffCollection
{
    private readonly List<BuffInstance> activeBuffs = new List<BuffInstance>();

    public IReadOnlyList<BuffInstance> ActiveBuffs => activeBuffs.AsReadOnly();

    // The missing events causing your CS1061 compiler error!
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
            OnBuffApplied?.Invoke(buffDef, stacks); // Fire Event!
        }
        else
        {
            activeBuffs.Add(new BuffInstance(buffDef, stacks, duration));
            OnBuffApplied?.Invoke(buffDef, stacks); // Fire Event!
        }

        OnBuffsChanged?.Invoke();
        return true;
    }

    public bool RemoveBuff(BuffDefinitionSO buffDef)
    {
        if (buffDef == null) return false;

        bool removed = activeBuffs.RemoveAll(buff => buff.BuffDef == buffDef) > 0;
        if (removed)
        {
            OnBuffRemoved?.Invoke(buffDef); // Fire Event!
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
        bool anyRemoved = false;
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].IsExpired)
            {
                OnBuffRemoved?.Invoke(activeBuffs[i].BuffDef); // Fire Event!
                activeBuffs.RemoveAt(i);
                anyRemoved = true;
            }
        }
        return anyRemoved;
    }

    // --- METHODS FOR STACK MANAGEMENT (Used by Burn Effect) ---

    public int GetBuffStacks(BuffDefinitionSO buffDef)
    {
        BuffInstance buff = activeBuffs.Find(b => b.BuffDef == buffDef);
        return buff != null ? buff.CurrentStacks : 0;
    }

    public void RemoveBuffStacks(BuffDefinitionSO buffDef, int amount)
    {
        BuffInstance buff = activeBuffs.Find(b => b.BuffDef == buffDef);
        if (buff != null)
        {
            buff.RemoveStacks(amount);
            if (buff.CurrentStacks <= 0)
            {
                RemoveBuff(buffDef); // Completely remove the buff if stacks hit 0
            }
            else
            {
                OnBuffApplied?.Invoke(buffDef, -amount); // Broadcast the stack reduction for UI updates
                OnBuffsChanged?.Invoke();
            }
        }
    }

    // -----------------------------------------------------------

    public List<SkillAction> GetTriggeredActions(BuffTriggerTime triggerTime)
    {
        List<SkillAction> actionsToRun = new List<SkillAction>();
        foreach (var buff in activeBuffs)
        {
            foreach (var trigger in buff.BuffDef.triggeredActions)
            {
                if (trigger.triggerTime == triggerTime)
                {
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
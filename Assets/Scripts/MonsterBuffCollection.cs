using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterBuffCollection
{
    private readonly List<BuffInstance> activeBuffs = new List<BuffInstance>();

    public IReadOnlyList<BuffInstance> ActiveBuffs => activeBuffs.AsReadOnly();

    public event Action OnBuffsChanged;

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

        OnBuffsChanged?.Invoke();
        return true;
    }

    public bool RemoveBuff(BuffDefinitionSO buffDef)
    {
        if (buffDef == null) return false;

        bool removed = activeBuffs.RemoveAll(buff => buff.BuffDef == buffDef) > 0;
        if (removed)
        {
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
}

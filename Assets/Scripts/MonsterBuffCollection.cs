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

    [NonSerialized] private MonsterInstance _owner;
    [NonSerialized] private CombatEventDispatcher _dispatcher;

    public void RegisterCombatEvents(CombatEventDispatcher dispatcher, MonsterInstance owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        dispatcher.TurnStarted += HandleTurnStarted;
        dispatcher.TurnEnded += HandleTurnEnded;
        dispatcher.CombatStarted += HandleCombatStarted;
    }

    public void AddBuff(BuffDefinitionSO buffDef, int stacks, MonsterInstance caster)
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
            activeBuffs.Add(new BuffInstance(buffDef, stacks, isPermanent, caster));
        }

        OnBuffApplied?.Invoke(buffDef, stacks);
        //_owner?.OnBuffApplied?.Invoke(buffDef, stacks);
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
        TickDurations();
    }

    private void ProcessTriggers(CombatTriggerTime triggerTime, IReadOnlyList<MonsterInstance> battlefield)
    {
        var actionsToRun = GetTriggeredActions(triggerTime);
        foreach (var action in actionsToRun)
        {
            CombatActionExecutor.ExecuteSkillAction(action, _owner, new List<MonsterInstance>(battlefield));
        }
    }

    public bool RemoveExpiredBuffs()
    {
        int countBefore = activeBuffs.Count;
        var expiredBuffs = new List<BuffInstance>();

        foreach (var buff in activeBuffs)
        {
            if (buff.IsExpired)
            {
                expiredBuffs.Add(buff);
            }
        }

        activeBuffs.RemoveAll(buff => buff.IsExpired);
        
        if (expiredBuffs.Count > 0)
        {
            foreach (var expired in expiredBuffs)
            {
                OnBuffRemoved?.Invoke(expired.BuffDef);
                //_owner?.OnBuffRemoved?.Invoke(expired.BuffDef);
                _dispatcher?.PublishBuffRemoved(_owner, expired.BuffDef);
            }

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

    public int GetBuffStacks(BuffType buffType)
    {
        var buff = activeBuffs.Find(b => b.BuffDef.buffType == buffType);
        return buff != null ? buff.CurrentStacks : 0;
    }
}
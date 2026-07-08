using System;
using System.Collections.Generic;

public class CombatEventDispatcher
{
    public event Action<CombatStartedEvent> CombatStarted;
    public event Action<CombatTurnStartedEvent> TurnStarted;
    public event Action<CombatTurnEndedEvent> TurnEnded;
    public event Action<SkillExecutedEvent> SkillExecuted;
    public event Action<DamageEvent> DamageApplied;
    public event Action<BuffAppliedEvent> BuffApplied;
    public event Action<BuffRemovedEvent> BuffRemoved;
    public event Action<MonsterDefeatedEvent> MonsterDefeated;

    public void PublishCombatStarted(IReadOnlyList<MonsterInstance> playerTeam, IReadOnlyList<MonsterInstance> enemyTeam)
    {
        var battlefield = new List<MonsterInstance>(playerTeam.Count + enemyTeam.Count);
        battlefield.AddRange(playerTeam);
        battlefield.AddRange(enemyTeam);
        CombatStarted?.Invoke(new CombatStartedEvent(playerTeam, enemyTeam, battlefield));
    }

    public void PublishTurnStarted(MonsterInstance activeMonster, IReadOnlyList<MonsterInstance> battlefield)
    {
        TurnStarted?.Invoke(new CombatTurnStartedEvent(activeMonster, battlefield));
    }

    public void PublishTurnEnded(MonsterInstance activeMonster, IReadOnlyList<MonsterInstance> battlefield)
    {
        TurnEnded?.Invoke(new CombatTurnEndedEvent(activeMonster, battlefield));
    }

    public void PublishSkillExecuted(SkillDefinitionSO skill, MonsterInstance caster, IReadOnlyList<MonsterInstance> targets)
    {
        SkillExecuted?.Invoke(new SkillExecutedEvent(skill, caster, targets));
    }

    public void PublishDamageApplied(MonsterInstance source, MonsterInstance target, int amount)
    {
        DamageApplied?.Invoke(new DamageEvent(source, target, amount));
    }

    public void PublishBuffApplied(MonsterInstance target, BuffDefinitionSO buffDef, int stacks, MonsterInstance caster)
    {
        BuffApplied?.Invoke(new BuffAppliedEvent(target, buffDef, stacks, caster));
    }

    public void PublishBuffRemoved(MonsterInstance target, BuffDefinitionSO buffDef)
    {
        BuffRemoved?.Invoke(new BuffRemovedEvent(target, buffDef));
    }

    public void PublishMonsterDefeated(MonsterInstance monster)
    {
        MonsterDefeated?.Invoke(new MonsterDefeatedEvent(monster));
    }
}

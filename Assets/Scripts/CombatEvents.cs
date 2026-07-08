using System.Collections.Generic;

public class CombatStartedEvent
{
    public IReadOnlyList<MonsterInstance> PlayerTeam { get; }
    public IReadOnlyList<MonsterInstance> EnemyTeam { get; }
    public IReadOnlyList<MonsterInstance> Battlefield { get; }

    public CombatStartedEvent(IReadOnlyList<MonsterInstance> playerTeam, IReadOnlyList<MonsterInstance> enemyTeam, IReadOnlyList<MonsterInstance> battlefield)
    {
        PlayerTeam = playerTeam;
        EnemyTeam = enemyTeam;
        Battlefield = battlefield;
    }
}

public class CombatTurnStartedEvent
{
    public MonsterInstance ActiveMonster { get; }
    public IReadOnlyList<MonsterInstance> Battlefield { get; }

    public CombatTurnStartedEvent(MonsterInstance activeMonster, IReadOnlyList<MonsterInstance> battlefield)
    {
        ActiveMonster = activeMonster;
        Battlefield = battlefield;
    }
}

public class CombatTurnEndedEvent
{
    public MonsterInstance ActiveMonster { get; }
    public IReadOnlyList<MonsterInstance> Battlefield { get; }

    public CombatTurnEndedEvent(MonsterInstance activeMonster, IReadOnlyList<MonsterInstance> battlefield)
    {
        ActiveMonster = activeMonster;
        Battlefield = battlefield;
    }
}

public class SkillExecutedEvent
{
    public SkillDefinitionSO Skill { get; }
    public MonsterInstance Caster { get; }
    public IReadOnlyList<MonsterInstance> Targets { get; }

    public SkillExecutedEvent(SkillDefinitionSO skill, MonsterInstance caster, IReadOnlyList<MonsterInstance> targets)
    {
        Skill = skill;
        Caster = caster;
        Targets = targets;
    }
}

public class DamageEvent
{
    public MonsterInstance Source { get; }
    public MonsterInstance Target { get; }
    public int Amount { get; }

    public DamageEvent(MonsterInstance source, MonsterInstance target, int amount)
    {
        Source = source;
        Target = target;
        Amount = amount;
    }
}

public class BuffAppliedEvent
{
    public MonsterInstance Target { get; }
    public BuffDefinitionSO BuffDef { get; }
    public int Stacks { get; }
    public MonsterInstance Caster { get; }

    public BuffAppliedEvent(MonsterInstance target, BuffDefinitionSO buffDef, int stacks, MonsterInstance caster)
    {
        Target = target;
        BuffDef = buffDef;
        Stacks = stacks;
        Caster = caster;
    }
}

public class BuffRemovedEvent
{
    public MonsterInstance Target { get; }
    public BuffDefinitionSO BuffDef { get; }

    public BuffRemovedEvent(MonsterInstance target, BuffDefinitionSO buffDef)
    {
        Target = target;
        BuffDef = buffDef;
    }
}

public class MonsterDefeatedEvent
{
    public MonsterInstance Monster { get; }

    public MonsterDefeatedEvent(MonsterInstance monster)
    {
        Monster = monster;
    }
}

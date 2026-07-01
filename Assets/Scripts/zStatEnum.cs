using System;


public enum StatType
{
    MaxHP,
    MaxMana,
    Strength,
    Defense,
    Intelligence,
    Speed,
    DodgeChance,
    CritChance,
    CritDamageMult
}

public enum ModifierType
{
    FlatAdd,       // e.g., +5 Attack
    PercentAdd,    // e.g., +2% Attack (0.02)
    PercentMultiply// e.g., x1.5 Attack
}

public enum CombatTriggerTime
{
    OnCombatStart,
    OnTurnStart,
    OnTurnEnd,
    OnTakeDamage,
    OnDealDamage
}
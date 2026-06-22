using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterInstance : IHealthObservable, IManaObservable
{
    // Identity & Setup
    public string InstanceId { get; private set; }
    public MonsterDefinitionSO MonsterDef { get; private set; }
    public CombatTeam Team { get; private set; }
    
    // Position on the 2x3 Grid
    public GridPosition gridPosition { get; set; }

    public bool IsDefeated => CurrentHP <= 0;
    public GridPosition GridPosition => gridPosition;
    
    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnManaChanged;

    private List<BuffInstance> activeBuffs = new List<BuffInstance>();
    public List<BuffInstance> ActiveBuffs => activeBuffs;

// --- Backing Fields & Mutators ---
    private float currentHP;
    public float CurrentHP
    {
        get => currentHP;
        set
        {
            currentHP = Math.Clamp(value, 0f, MaxHP);
            OnHPChanged?.Invoke(currentHP, MaxHP);
        }
    }

    private float currentMana;
    public float CurrentMana
    {
        get => currentMana;
        set
        {
            currentMana = Math.Clamp(value, 0f, MaxMana);
            OnManaChanged?.Invoke(currentMana, MaxMana);
        }
    }

    public float MaxHP => CalculateStat(StatType.MaxHP, MonsterDef.BaseStats.maxHP);
    public float MaxMana => CalculateStat(StatType.MaxMana, MonsterDef.BaseStats.maxMana);

    public float Strength => CalculateStat(StatType.Strength, MonsterDef.BaseStats.strength);
    public float Defense => CalculateStat(StatType.Defense, MonsterDef.BaseStats.defense);
    public float Intelligence => CalculateStat(StatType.Intelligence, MonsterDef.BaseStats.intelligence);
    public float Speed => CalculateStat(StatType.Defense, MonsterDef.BaseStats.speed);
    public float CritChance => CalculateStat(StatType.CritChance, MonsterDef.BaseStats.critChance);
    public float CritDamageMult => CalculateStat(StatType.CritDamageMult, MonsterDef.BaseStats.critDamageMult);
    public float DodgeChance => CalculateStat(StatType.DodgeChance, MonsterDef.BaseStats.DodgeChance);
    public void TakeDamage(int damageAmount)
    {
        // 1. Subtract the damage from current health
        CurrentHP -= damageAmount;

        // 2. Clamp the health so it doesn't go below 0
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }

        Debug.Log($"{this.MonsterDef.MonsterName} took {damageAmount} damage! Current HP: {CurrentHP}/{MaxHP}");

        // 3. Handle death if health hits zero
        if (CurrentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{this.MonsterDef.MonsterName} has fainted!");
    }

    public void AddBuff(BuffDefinitionSO buffDef, int stacks, int duration)
{
    // Check if we already have this exact buff
    BuffInstance existingBuff = activeBuffs.Find(b => b.BuffDef == buffDef);

    if (existingBuff != null)
    {
        existingBuff.AddStacks(stacks);
        // Optional: Refresh duration when re-applied
        // existingBuff.RemainingDuration = duration; 
    }
    else
    {
        BuffInstance newBuff = new BuffInstance(buffDef, stacks, duration);
        activeBuffs.Add(newBuff);
        Debug.Log($"{MonsterDef.MonsterName} received {buffDef.buffName}!");
    }
}

    private float CalculateStat(StatType statType, float baseValue)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;
        float multiplier = 1f;

        // Loop through all active buffs to find modifiers for THIS specific stat
        foreach (var buff in activeBuffs)
        {
            foreach (var modifier in buff.BuffDef.statModifiers)
            {
                if (modifier.statToModify == statType)
                {
                    // Multiply the modifier value by the number of stacks!
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
                            // For multiply, 1 stack of 1.5x = 1.5. 2 stacks = 1.5 * 1.5 = 2.25
                            multiplier *= Mathf.Pow(modifier.valuePerStack, buff.CurrentStacks);
                            break;
                    }
                }
            }
        }

        // Standard RPG Math: (Base + Flat) * (1 + PercentBonus) * Multiplier
        float finalValue = (baseValue + flatBonus) * (1f + percentBonus) * multiplier;
        
        // Prevent stats from dropping below zero due to heavy debuffs
        return Mathf.Max(0, finalValue);
    }

    public MonsterInstance(MonsterDefinitionSO monsterDef, CombatTeam team, GridPosition startingPosition)
    {
        MonsterDef = monsterDef ?? throw new ArgumentNullException(nameof(monsterDef));
        InstanceId = Guid.NewGuid().ToString();
        Team = team;
        gridPosition = startingPosition;

        CurrentHP = monsterDef.BaseStats.maxHP;
        CurrentMana = monsterDef.BaseStats.maxMana;
    }
}
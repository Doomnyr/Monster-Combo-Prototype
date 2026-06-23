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

    private float _cachedMaxHP;
    private float _cachedMaxMana;
    private float _cachedStrength;
    private float _cachedDefense;
    private float _cachedIntelligence;
    private float _cachedSpeed;
    private float _cachedCritChance;
    private float _cachedCritDamageMult;
    private float _cachedDodgeChance;

    private void RecalculateDerivedStats()
    {
        _cachedMaxHP = CalculateStat(StatType.MaxHP, MonsterDef.BaseStats.maxHP);
        _cachedMaxMana = CalculateStat(StatType.MaxMana, MonsterDef.BaseStats.maxMana);
        _cachedStrength = CalculateStat(StatType.Strength, MonsterDef.BaseStats.strength);
        _cachedDefense = CalculateStat(StatType.Defense, MonsterDef.BaseStats.defense);
        _cachedIntelligence = CalculateStat(StatType.Intelligence, MonsterDef.BaseStats.intelligence);
        _cachedSpeed = CalculateStat(StatType.Speed, MonsterDef.BaseStats.speed);
        _cachedCritChance = CalculateStat(StatType.CritChance, MonsterDef.BaseStats.critChance);
        _cachedCritDamageMult = CalculateStat(StatType.CritDamageMult, MonsterDef.BaseStats.critDamageMult);
        _cachedDodgeChance = CalculateStat(StatType.DodgeChance, MonsterDef.BaseStats.DodgeChance);

        _currentHP = Math.Clamp(_currentHP, 0f, _cachedMaxHP);
        _currentMana = Math.Clamp(_currentMana, 0f, _cachedMaxMana);
    }

// --- Backing Fields & Mutators ---
    private float _currentHP;
    public float CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = Math.Clamp(value, 0f, MaxHP);
            OnHPChanged?.Invoke(_currentHP, MaxHP);
        }
    }

    private float _currentMana;
    public float CurrentMana
    {
        get => _currentHP;
        set
        {
            _currentMana = Math.Clamp(value, 0f, MaxMana);
            OnManaChanged?.Invoke(_currentMana, MaxMana);
        }
    }

    public float MaxHP => _cachedMaxHP;
    public float MaxMana => _cachedMaxMana;

    public float Strength => _cachedStrength;
    public float Defense => _cachedDefense;
    public float Intelligence => _cachedIntelligence;
    public float Speed => _cachedSpeed;
    public float CritChance => _cachedCritChance;
    public float CritDamageMult => _cachedCritDamageMult;
    public float DodgeChance => _cachedDodgeChance;
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

        RecalculateDerivedStats();
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

        _currentHP = monsterDef.BaseStats.maxHP;
        _currentMana = monsterDef.BaseStats.maxMana;
        RecalculateDerivedStats();
    }
}

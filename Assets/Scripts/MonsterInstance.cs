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

    public MonsterBuffCollection Buffs { get; private set; }
    public IReadOnlyList<BuffInstance> ActiveBuffs => Buffs.ActiveBuffs;

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

        OnHPChanged?.Invoke(_currentHP, _cachedMaxHP);
        OnManaChanged?.Invoke(_currentMana, _cachedMaxMana);
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
        get => _currentMana;
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
        Buffs.AddBuff(buffDef, stacks, duration);
    }

    public bool RemoveBuff(BuffDefinitionSO buffDef)
    {
        return Buffs.RemoveBuff(buffDef);
    }

    public bool SetBuffDuration(BuffDefinitionSO buffDef, int duration)
    {
        return Buffs.SetBuffDuration(buffDef, duration);
    }

    public void TickBuffDurations()
    {
        Buffs.TickDurations();
    }

    private float CalculateStat(StatType statType, float baseValue)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;
        float multiplier = 1f;

        foreach (var buff in Buffs.ActiveBuffs)
        {
            foreach (var modifier in buff.BuffDef.statModifiers)
            {
                if (modifier.statToModify == statType)
                {
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
        }

        float finalValue = (baseValue + flatBonus) * (1f + percentBonus) * multiplier;
        return Mathf.Max(0, finalValue);
    }

    public MonsterInstance(MonsterDefinitionSO monsterDef, CombatTeam team, GridPosition startingPosition)
    {
        MonsterDef = monsterDef ?? throw new ArgumentNullException(nameof(monsterDef));
        InstanceId = Guid.NewGuid().ToString();
        Team = team;
        gridPosition = startingPosition;

        Buffs = new MonsterBuffCollection();
        Buffs.OnBuffsChanged += RecalculateDerivedStats;

        _currentHP = monsterDef.BaseStats.maxHP;
        _currentMana = monsterDef.BaseStats.maxMana;
        RecalculateDerivedStats();
    }
}

        
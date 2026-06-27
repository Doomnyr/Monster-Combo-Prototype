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
    public bool IsAlive => CurrentHP > 0;
    public GridPosition GridPosition => gridPosition;

    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<int> OnDamageTaken;
    public event Action<int> OnHealed;
    
    public MonsterBuffCollection Buffs { get; private set; }
    public IReadOnlyList<BuffInstance> ActiveBuffs => Buffs.ActiveBuffs;
    //public List<TraitDefinitionSO> Traits { get; private set; }
    public MonsterTraitCollection Traits { get; private set; }

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
        _cachedMaxHP = Buffs.CalculateModifiedStat(StatType.MaxHP, MonsterDef.BaseStats.maxHP);
        _cachedMaxMana = Buffs.CalculateModifiedStat(StatType.MaxMana, MonsterDef.BaseStats.maxMana);
        _cachedStrength = Buffs.CalculateModifiedStat(StatType.Strength, MonsterDef.BaseStats.strength);
        _cachedDefense = Buffs.CalculateModifiedStat(StatType.Defense, MonsterDef.BaseStats.defense);
        _cachedIntelligence = Buffs.CalculateModifiedStat(StatType.Intelligence, MonsterDef.BaseStats.intelligence);
        _cachedSpeed = Buffs.CalculateModifiedStat(StatType.Speed, MonsterDef.BaseStats.speed);
        _cachedCritChance = Buffs.CalculateModifiedStat(StatType.CritChance, MonsterDef.BaseStats.critChance);
        _cachedCritDamageMult = Buffs.CalculateModifiedStat(StatType.CritDamageMult, MonsterDef.BaseStats.critDamageMult);
        _cachedDodgeChance = Buffs.CalculateModifiedStat(StatType.DodgeChance, MonsterDef.BaseStats.DodgeChance);

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
        CurrentHP -= damageAmount;

        OnDamageTaken?.Invoke(damageAmount);
        Debug.Log($"{this.MonsterDef.MonsterName} took {damageAmount} damage! Current HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void TakeHeal(int healAmount)
    {
        CurrentHP -= healAmount;

        OnHealed?.Invoke(healAmount);
        Debug.Log($"{this.MonsterDef.MonsterName} was healed {healAmount}! Current HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{this.MonsterDef.MonsterName} has fainted!");
    }

    public MonsterInstance(MonsterDefinitionSO monsterDef, CombatTeam team, GridPosition startingPosition, List<TraitDefinitionSO> traits)
    {
        MonsterDef = monsterDef ?? throw new ArgumentNullException(nameof(monsterDef));
        InstanceId = Guid.NewGuid().ToString();
        Team = team;
        gridPosition = startingPosition;

        Buffs = new MonsterBuffCollection();
        Buffs.OnBuffsChanged += RecalculateDerivedStats;

        Traits = new MonsterTraitCollection(traits);

        _currentHP = monsterDef.BaseStats.maxHP;
        _currentMana = monsterDef.BaseStats.maxMana;
        RecalculateDerivedStats();
    }
}
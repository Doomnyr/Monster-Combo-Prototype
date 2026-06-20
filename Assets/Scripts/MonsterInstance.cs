using System;

public enum CombatTeam { Player, Enemy }

[System.Serializable]
public struct GridPosition
{
    public int Column; // 0 or 1
    public int Row;    // 0, 1, or 2

    public GridPosition(int column, int row)
    {
        Column = column;
        Row = row;
    }
}

[System.Serializable]
public class MonsterInstance : IHealthObservable, IManaObservable
{
    // Identity & Setup
    public string InstanceId { get; private set; }
    public MonsterDefinitionSO MonsterDef { get; private set; }
    public CombatTeam Team { get; private set; }
    
    // Position on the 2x3 Grid
    public GridPosition gridPosition { get; set; }

    public float MaxHP => MonsterDef.BaseStats.maxHP;
    public float MaxMana => MonsterDef.BaseStats.maxMana;
    public bool IsDefeated => CurrentHP <= 0;
    public GridPosition GridPosition => gridPosition;
    
    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnManaChanged;

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
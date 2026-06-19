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
public class MonsterInstance
{
    // Identity & Setup
    public string InstanceId { get; private set; }
    public MonsterDefinition MonsterDef { get; private set; }
    public CombatTeam Team { get; private set; }
    
    // Position on the 2x3 Grid
    public GridPosition Position { get; set; }

    // Live Resources Only
    public float CurrentHP { get; set; }
    public float CurrentMana { get; set; }
    public bool IsDead => CurrentHP <= 0;

    public MonsterInstance(MonsterDefinition monsterDef, CombatTeam team, GridPosition startingPosition)
    {
        MonsterDef = monsterDef ?? throw new ArgumentNullException(nameof(monsterDef));
        InstanceId = Guid.NewGuid().ToString();
        Team = team;
        Position = startingPosition;

        CurrentHP = monsterDef.BaseStats.maxHP;
        CurrentMana = monsterDef.BaseStats.maxMana;
    }
}
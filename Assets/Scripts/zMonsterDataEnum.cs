[System.Serializable]
public struct StatBlock {
    public float maxHP;
    public float maxMana;
    public float strength;
    public float defense;
    public float intelligence;
    public float speed;

    public float critChance;
    public float critDamageMult;
    public float DodgeChance;
}

// Traits

public enum MonsterElement {
    Default,
    Water,
    Fire,
    Nature,
    Earth,
    Light,
    Dark
}

public enum MonsterRace {
    Beast,
    Demon,
    Undead,
    Dragon,
    Mech,
    Elemental
}
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct StatBlock {
    public float maxHP;
    public float maxMana;
    public float attack;
    public float defense;
    public float intelligence;
    public float speed;

    public float critChance;
    public float critDamageMult;
}

// Traits

public enum MonsterElement {
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

[CreateAssetMenu(fileName = "NewMonsterDefinition", menuName = "MonsterCombo/Monster Definition")]
public class MonsterDefinition : ScriptableObject
{
    [Header("Monster Identity")]
    [SerializeField] private string monsterName;
    [SerializeField] private MonsterRace race;
    [SerializeField] private MonsterElement element;

    [Header("Base Attribute Settings")]
    [SerializeField] private StatBlock baseStats;

    [Header("AI Command Priority List")]
    [Tooltip("The engine reads this from top to bottom. It will execute the first skill it can afford and/or is valid.")]
    [SerializeField] private List<SkillDefinition> commandPriorityList = new List<SkillDefinition>();

    public string MonsterName => monsterName;
    public MonsterRace Race => race;
    public MonsterElement Element => element;
    public StatBlock BaseStats => baseStats;
    public List<SkillDefinition> UsableSkills => usableSkills;
}
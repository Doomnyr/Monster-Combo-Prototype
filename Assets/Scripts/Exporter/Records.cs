using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatBlockRecord
{
    public float maxHP;
    public float maxMana;
    public float strength;
    public float defense;
    public float intelligence;
    public float speed;
    public float dodgeChance;
    public float critChance;
    public float critDamageMult;
}

[System.Serializable]
public class MonsterDatabase
{
    public MonsterRecord[] monsters;
}

[Serializable]
public class MonsterRecord
{
    [SerializeField] public string monsterID;
    [SerializeField] public string monsterName;
    [SerializeField] public string monsterSprite;
    [SerializeField] public MonsterRace race;
    [SerializeField] public ElementType element;
    [SerializeField] public StatBlock baseStats;
    [SerializeField] public List<TraitDefinitionSO> traits;
    [SerializeField] public string[] commandList;
}

[Serializable]
public class ElementRecord
{
    public ElementType element;
    
}

[Serializable]
public class RaceRecord
{
    public MonsterRace race;
    
}

[Serializable]
public class MonsterDatabaseSchema
{
    public MonsterRecord[] monsters;
}
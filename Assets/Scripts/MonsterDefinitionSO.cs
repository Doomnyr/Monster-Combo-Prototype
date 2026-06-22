using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMonsterDefinition", menuName = "MonsterCombo/Monster Definition")]
public class MonsterDefinitionSO : ScriptableObject
{
    [Header("Monster Identity")]
    [SerializeField] private string monsterName;
    [SerializeField] private MonsterRace race;
    [SerializeField] private MonsterElement element;

    [Header("Base Attribute Settings")]
    [SerializeField] private StatBlock baseStats;

    [Header("AI Command Priority List")]
    [Tooltip("The engine reads this from top to bottom. It will execute the first skill it can afford and/or is valid.")]
    [SerializeField] private List<SkillDefinitionSO> commandPriorityList = new List<SkillDefinitionSO>();

    public string MonsterName => monsterName;
    public MonsterRace Race => race;
    public MonsterElement Element => element;
    public StatBlock BaseStats => baseStats;
    public List<SkillDefinitionSO> CommandPriorityList => commandPriorityList;
}
using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

[CreateAssetMenu(fileName = "NewMonsterDefinition", menuName = "MonsterCombo/Monster Definition")]
public class MonsterDefinitionSO : ScriptableObject
{
    [Header("Monster Identity")]
    [SerializeField] private string monsterName;
    [SerializeField] private Sprite monsterSprite;
    [SerializeField] private MonsterRace race;
    [SerializeField] private ElementType element;

    [Header("Base Attribute Settings")]
    [SerializeField] private StatBlock baseStats;

    [Header("Traits")]
    public List<TraitDefinitionSO> traits;

    [Header("AI Command Priority List")]
    [Tooltip("The engine reads this from top to bottom. It will execute the first skill it can afford and/or is valid.")]
    [SerializeField] private List<SkillDefinitionSO> commandPriorityList = new List<SkillDefinitionSO>();

    public string MonsterName => monsterName;
    public Sprite MonsterSprite => monsterSprite;
    public MonsterRace Race => race;
    public ElementType Element => element;
    public StatBlock BaseStats => baseStats;
    public List<SkillDefinitionSO> CommandPriorityList => commandPriorityList;
    public List<TraitDefinitionSO> Traits => traits;
}
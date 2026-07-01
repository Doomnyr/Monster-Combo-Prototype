using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

[CreateAssetMenu(fileName = "NewMonsterDefinition", menuName = "MonsterCombo/Monster Definition")]
public class MonsterDefinitionSO : ScriptableObject
{
    [Header("Monster Identity")]
    [SerializeField] private string _monsterID;
    [SerializeField] private string _monsterName;
    [SerializeField] private Sprite _monsterSprite;
    [SerializeField] private MonsterRace _race;
    [SerializeField] private ElementType _element;

    [Header("Base Attribute Settings")]
    [SerializeField] private StatBlock _baseStats;

    [Header("Traits")]
    public List<TraitDefinitionSO> _traits;

    [Header("AI Command Priority List")]
    [Tooltip("The engine reads this from top to bottom. It will execute the first skill it can afford and/or is valid.")]
    [SerializeField] private List<SkillDefinitionSO> _commandPriorityList = new List<SkillDefinitionSO>();

    public string MonsterID => _monsterID;
    public string MonsterName => _monsterName;
    public Sprite MonsterSprite => _monsterSprite;
    public MonsterRace Race => _race;
    public ElementType Element => _element;
    public StatBlock BaseStats => _baseStats;
    public List<SkillDefinitionSO> CommandPriorityList => _commandPriorityList;
    public List<TraitDefinitionSO> Traits => _traits;

    #if UNITY_EDITOR
    /// <summary>
    /// Editor-only method to populate data from the JSON importer.
    /// This method is stripped out of the final game build.
    /// </summary>
    public void Editor_ImportData(
        string id, 
        string monsterName, 
        Sprite sprite, 
        MonsterRace race, 
        ElementType element, 
        StatBlock stats,
        List<TraitDefinitionSO> traits,
        List<SkillDefinitionSO> skills)
    {
        _monsterID = id;
        _monsterName = monsterName;
        _monsterSprite = sprite;
        _race = race;
        _element = element;
        _baseStats = stats;
        
        _traits = traits != null ? new List<TraitDefinitionSO>(traits) : new List<TraitDefinitionSO>();
        _commandPriorityList = skills != null ? new List<SkillDefinitionSO>(skills) : new List<SkillDefinitionSO>();
        
        UnityEditor.EditorUtility.SetDirty(this);
    }
    #endif
}
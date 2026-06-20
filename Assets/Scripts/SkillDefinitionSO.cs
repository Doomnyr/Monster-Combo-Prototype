using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill", menuName = "MonsterCombo/Skill Definition")]
public class SkillDefinitionSO : ScriptableObject
{
    [Header("Identity Elements")]
    [SerializeField] private string skillName;
    [SerializeField] [TextArea(2, 4)] private string description;

    [Header("Requirements")]
    [SerializeField] private float manaCost;

    // [SerializeReference] tells Unity to draw an interface picker for abstract/polymorphic sub-types
    [SerializeReference] 
    private List<SkillEffect> _executionEffects = new List<SkillEffect>();

    // Clean public contracts
    public string SkillName => skillName;
    public string Description => description;
    public float ManaCost => manaCost;
    public List<SkillEffect> ExecutionEffects => _executionEffects;
}
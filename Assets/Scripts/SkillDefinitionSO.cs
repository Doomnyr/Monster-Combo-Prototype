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

    [Header("Skill Sequence")]
    [Tooltip("The steps this skill takes when executed, in order.")]
    [SerializeField] 
    private List<SkillAction> _actions = new List<SkillAction>();

    // Clean public contracts
    public string SkillName => skillName;
    public string Description => description;
    public float ManaCost => manaCost;
    
    // This exposes your new paired actions to the Combat Manager
    public List<SkillAction> Actions => _actions; 
}
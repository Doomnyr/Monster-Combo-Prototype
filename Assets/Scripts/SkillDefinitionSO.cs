using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill", menuName = "MonsterCombo/Skill Definition")]
public class SkillDefinitionSO : ScriptableObject
{
    [Header("Identity Elements")]
    [SerializeField] private string _name;
    [SerializeField] [TextArea(2, 4)] private string _description;
    [SerializeField] private ElementType _element;

    [Header("Requirements")]
    [SerializeField] private float _manaCost;


    [Header("Skill Sequence")]
    [Tooltip("The steps this skill takes when executed, in order.")]
    [SerializeField] 
    private List<SkillAction> _actions = new List<SkillAction>();

    // Clean public contracts
    public string SkillName => _name;
    public string Description => _description;
    public float ManaCost => _manaCost;
    public ElementType Element => _element;
    public List<SkillAction> Actions => _actions; 
}
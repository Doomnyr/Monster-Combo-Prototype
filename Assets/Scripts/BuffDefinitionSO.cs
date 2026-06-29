using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

[Serializable]
public struct StatModifierData
{
    public StatType statToModify;
    public ModifierType modifierType;
    public float valuePerStack;
}

[Serializable]
public struct BuffTriggerData
{
    public CombatTriggerTime triggerTime;
    public SkillAction actionToTrigger; 
}

[CreateAssetMenu(fileName = "NewBuff", menuName = "MonsterCombo/Buff Definition")]
public class BuffDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private BuffType _type;
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _isDebuff; 

    [Header("Stacking Rules")]
    [SerializeField] private int _maxStacks = 1;
    [SerializeField] private bool _isPermanent = false;
    
    [Header("Passive Stat Modifiers")]
    [Tooltip("Stats modified passively while this status is active.")]
    [SerializeField] private List<StatModifierData> _statModifiers = new List<StatModifierData>();

    [Header("Active Triggers")]
    [Tooltip("Actions that happen at specific moments (like Poison at Turn End).")]
    [SerializeField] private List<BuffTriggerData> _triggerActions = new List<BuffTriggerData>();

    public string Name => _name;
    public Sprite Icon => _icon;
    public BuffType Type => _type;
    public bool IsHidden => _isHidden;
    public bool IsDebuff => _isDebuff;
    
    public int MaxStacks => _maxStacks;
    public bool IsPermanent => _isPermanent;
    
    public List<StatModifierData> StatModifiers => _statModifiers;
    public List<BuffTriggerData> TriggerActions => _triggerActions;
}


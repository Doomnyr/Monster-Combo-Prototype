using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "MonsterCombo/Buff Definition")]
public class BuffDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    public string buffName;
    public bool isDebuff; 
    public Sprite buffIcon;
    public BuffType buffType;

    [Header("Stacking Rules")]
    public int maxStacks = 1;
    public bool infiniteStacks = false;
    
    [Header("Passive Stat Modifiers")]
    [Tooltip("Stats modified passively while this status is active.")]
    public List<StatModifierData> statModifiers = new List<StatModifierData>();

    [Header("Active Triggers")]
    [Tooltip("Actions that happen at specific moments (like Poison at Turn End).")]
    public List<BuffTriggerData> triggeredActions = new List<BuffTriggerData>();
}

// --- Helper Structs for the Inspector ---

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
    
    [Tooltip("Reusing our SkillAction! E.g., TargetSelf + Effect_Damage")]
    public SkillAction actionToTrigger; 
}
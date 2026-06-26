using System;
using UnityEngine;

[Serializable]
public struct SkillAction
{
    public TargetFinderSO targetFinder;
    public SkillEffectSO executionEffect;

    public MonsterElement _skillElement;

    [Header("Registry")]
    [Tooltip("If set, the results of this finder will be saved to the context under this key.")]
    public string saveTargetsAsKey; // e.g., "UnitA"
    [Tooltip("If set, this effect will use targets stored in the context under this key.")]
    public string useTargetsFromKey; // e.g., "UnitA"

    // Damage Mod
    public float baseValue;
    public float scaleCoefficient;
    public StatType scaleStat;

    // Buff Mod
    public BuffDefinitionSO buffToApply;
    public int buffDuration;
    public int buffCount;
    
}
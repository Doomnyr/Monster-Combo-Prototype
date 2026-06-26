using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SkillAction
{
    public TargetFinderSO targetFinder;
    public SkillEffectSO executionEffect;
    public ElementType element = ElementType.None;

    // Damage Mod
    public float baseValue;
    public StatType scalingStat;
    public float scalingCoefficient;

    // Buffs
    public BuffDefinitionSO buffToApply;
    public int buffCount = 1;
    public int buffduration = 3;

}
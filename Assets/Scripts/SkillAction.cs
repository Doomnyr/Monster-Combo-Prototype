using System;
using UnityEngine;

[Serializable]
public struct SkillAction
{
    [Tooltip("Who gets hit by this specific step?")]
    public TargetFinderSO targetFinder;

    [Tooltip("What happens to them?")]
    public SkillEffectSO executionEffect;
}
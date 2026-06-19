using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SkillEffect
{
    [Header("Targeting Configuration")]
    [SerializeField] private TargetScope _targetScope;

    // Public property so the CombatManager can see who this effect wants to target
    public TargetScope TargetScope => _targetScope;
    /// <summary>
    /// Executes the core logic of this specific effect sequence.
    /// </summary>
    /// <param name="caster">The monster executing the action.</param>
    /// <param name="target">The monster receiving the consequence of this effect block.</param>
    public abstract void Execute(MonsterInstance caster, List<MonsterInstance> targets);
}
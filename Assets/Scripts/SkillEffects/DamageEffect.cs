using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageEffect : SkillEffect
{
    [Tooltip("Multiplier scaled against the caster's attack parameter.")]
    [SerializeField] private float _attackMultiplier = 1.0f;

    public override void Execute(MonsterInstance caster, List<MonsterInstance> targets)
    {
        float totalAttack = caster.MonsterDef.BaseStats.attack * _attackMultiplier;

        // Loop through every target provided by the targeting system
        foreach (MonsterInstance target in targets)
        {
            if (target == null || target.IsDefeated) continue;

            float totalDefense = target.MonsterDef.BaseStats.defense;
            
            // Ensure damage never drops below 1 point
            float damageDealt = Mathf.Max(1f, totalAttack - totalDefense);
            
            target.CurrentHP -= damageDealt;
            Debug.Log($"[Skill Engine] {caster.MonsterDef.MonsterName} dealt {damageDealt} damage to {target.MonsterDef.MonsterName}.");
        }
    }
}
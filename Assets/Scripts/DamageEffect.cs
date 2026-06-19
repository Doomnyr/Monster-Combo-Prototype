using System;
using UnityEngine;

[Serializable]
public class DamageEffect : SkillEffect
{
    [Tooltip("Multiplier scaled against the caster's attack parameter.")]
    [SerializeField] private float _attackMultiplier = 1.0f;

    public override void Execute(MonsterInstance caster, MonsterInstance target)
    {
        float totalAttack = caster.MonsterDef.BaseStats.attack * _attackMultiplier;
        float totalDefense = target.MonsterDef.BaseStats.defense;

        // Mitigate damage safely, forcing at least 1 point of impact
        float damageDealt = Mathf.Max(1f, totalAttack - totalDefense);
        
        target.CurrentHP -= damageDealt;
        Debug.Log($"[Skill Engine] {caster.MonsterDef.MonsterName} hit {target.MonsterDef.MonsterName} for {damageDealt} damage.");
    }
}
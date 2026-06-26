using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage", menuName = "Combat/Effects/Effect_Damage")]
public class Effect_Damage : SkillEffectSO
{
    public override void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;

        // Route calculation through our centralized Accountant (DamageCalculator)
        int finalDamage = DamageCalculator.CalculateDamage(
            caster, 
            target, 
            (int)skill.baseValue, 
            skill.scalingStat,
            skill.scalingCoefficient, 
            skill.element
        );

        Debug.Log($"[COMBAT] {caster.MonsterDef.MonsterName} ({caster.MonsterDef.Element}) used a " +
                  $"{skill.element} Skill scaling with {skill.scalingStat} on {target.MonsterDef.MonsterName} ({target.MonsterDef.Element}) for {finalDamage} damage!");

        target.TakeDamage(finalDamage);
    }
}
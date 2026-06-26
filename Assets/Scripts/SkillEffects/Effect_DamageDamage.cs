using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage", menuName = "Combat/Effects/Damage")]
public class Effect_Damage : SkillEffectSO
{
    public override void Apply(SkillExecutionContext context, SkillAction skill)
    {
        if (!target.IsAlive) return;

        MonsterElement elementToUse = (skill._skillElement == MonsterElement.Default) 
            ? context.Caster.MonsterDef.Element 
            : skill._skillElement;

        int finalDamage = DamageCalculator.CalculateDamage(
            context.Caster, 
            context.LastTargets, 
            skill.baseValue, 
            skill.scaleStat,
            skill.scaleCoefficient, 
            elementToUse
        );

        Debug.Log($"[COMBAT] {caster.MonsterDef.MonsterName} ({caster.MonsterDef.Element}) used a " +
                  $"{elementToUse} Skill scaling with {skill.scaleStat} on {target.MonsterDef.MonsterName} ({target.MonsterDef.Element}) for {finalDamage} damage!");

        target.TakeDamage(finalDamage);
    }
}
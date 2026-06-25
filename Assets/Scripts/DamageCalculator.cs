using UnityEngine;

public static class DamageCalculator
{
    /// <summary>
    /// Calculates the final damage value between a caster and a target using our unified damage pipeline.
    /// </summary>
    public static int CalculateDamage(MonsterInstance caster, MonsterInstance target, int baseDamageValue, StatType statScaling, float strengthScaling, MonsterElement attackElement)
    {
        // 1. Calculate raw base damage using the caster's Strength and skill values
        float rawStatDamage = baseDamageValue + (caster.Strength * strengthScaling);

        // 2. Reduce based on the defender's modified Defense (Clamped to a minimum of 1 damage)
        float postDefenseDamage = Mathf.Max(1f, rawStatDamage - target.Defense);

        // 3. Initialize our PoE-style Damage Pipeline
        DamageContext damagePipeline = new DamageContext(postDefenseDamage);

        // 4. Gather Elemental Modifiers (Using our static ElementalUtility)
        float elementalBonus = ElementalUtility.GetElementalAdditiveBonus(
            attackElement,
            target.MonsterDef.Element,
            caster.MonsterDef.Element
        );

        if (elementalBonus != 0f)
        {
            damagePipeline.AddAdditiveModifier(elementalBonus);
        }

        // ========================================================
        // 🚀 FUTURE TRAIT PIPELINE HOOKS:
        // You can easily poll traits or special status conditions here:
        //
        // float beastMultiplier = TraitUtility.GetBeastBonus(caster);
        // damagePipeline.AddAdditiveModifier(beastMultiplier);
        //
        // float damageDebuff = caster.Buffs.CalculateModifiedStat(StatType.DamageDealtDebuff, 0f);
        // damagePipeline.AddAdditiveModifier(-damageDebuff);
        // ========================================================

        // 5. Compute the final total
        return damagePipeline.CalculateFinalDamage();
    }
}
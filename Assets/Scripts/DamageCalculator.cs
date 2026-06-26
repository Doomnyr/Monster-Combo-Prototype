using UnityEngine;

/// <summary>
/// Central math engine ("The Accountant") responsible for assembling the PoE-style
/// DamageContext and compiling all modifiers from elements, stats, and eventually traits.
/// </summary>
public static class DamageCalculator
{

    public static int CalculateFlatDamage(MonsterInstance caster, MonsterInstance target, int baseDamageValue)
    {
        DamageContext damagePipeline = new DamageContext(baseDamageValue);
        
        return damagePipeline.CalculateFinalDamage();
    }
    
    /// <summary>
    /// Calculates the final damage value between a caster and a target using our unified damage pipeline.
    /// </summary>
    public static int CalculateDamage(MonsterInstance caster, MonsterInstance target, int baseDamageValue, StatType scalingStat, float scalingCoefficient, ElementType attackElement)
    {
        // 1. DYNAMIC STAT FETCHING: Get the value for the specific StatType
        float statValue = GetStatValue(caster, scalingStat);

        // 2. Calculate raw base damage using the fetched stat and scaling coefficient
        float rawStatDamage = baseDamageValue + (statValue * scalingCoefficient);

        // 3. Reduce based on the defender's modified Defense (Clamped to a minimum of 1 damage)
        float postDefenseDamage = Mathf.Max(1f, rawStatDamage - target.Defense);

        // 4. Initialize our PoE-style Damage Pipeline
        DamageContext damagePipeline = new DamageContext(postDefenseDamage);

        // 5. Gather Elemental Modifiers (Using our static ElementalUtility)
        float elementalBonus = ElementalUtility.GetElementalAdditiveBonus(
            attackElement,
            target.MonsterDef.Element,
            caster.MonsterDef.Element
        );

        if (elementalBonus != 0f)
        {
            damagePipeline.AddAdditiveModifier(elementalBonus);
        }

        // 6. Compute the final total
        return damagePipeline.CalculateFinalDamage();
    }

    /// <summary>
    /// Routes the StatType to the correct property on the MonsterInstance.
    /// </summary>
    private static float GetStatValue(MonsterInstance caster, StatType stat)
    {
        return stat switch
        {
            StatType.Strength    => caster.Strength,
            StatType.Defense   => caster.Defense,
            StatType.Speed     => caster.Speed,
            StatType.MaxHP     => caster.MaxHP,
            _                  => caster.Strength // Default fallback
        };
    }
}
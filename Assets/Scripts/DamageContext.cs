using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A temporary payload that collects all damage bonuses and penalties 
/// from elements, traits, and buffs before executing the final math.
/// </summary>
public class DamageContext
{
    public float BaseDamage;

    // Additive Modifiers (PoE's "Increased" / "Reduced" damage)
    // Example: +0.5f (Weakness), +0.5f (Beast Trait), -0.25f (Damage Debuff)
    public List<float> AdditiveModifiers = new List<float>();

    // Multiplicative Modifiers (PoE's "More" / "Less" damage)
    // Example: x1.5f (A rare, powerful multiplier that ignores additive diminishing returns)
    public List<float> MultiplicativeModifiers = new List<float>();

    public DamageContext(float baseDamage)
    {
        BaseDamage = baseDamage;
    }

    public void AddAdditiveModifier(float percentBonus)
    {
        AdditiveModifiers.Add(percentBonus);
    }

    public void AddMultiplicativeModifier(float multiplier)
    {
        MultiplicativeModifiers.Add(multiplier);
    }

    public int CalculateFinalDamage()
    {
        // 1. Sum up all Additive Percentages (Starting at a base of 1.0, or 100%)
        float additiveSum = 1.0f;
        foreach (float mod in AdditiveModifiers)
        {
            additiveSum += mod;
        }

        // Clamp to 0 so massive debuffs don't cause negative damage (which would heal enemies)
        additiveSum = Mathf.Max(0f, additiveSum);

        // 2. Apply additive sum to base damage
        float totalDamage = BaseDamage * additiveSum;

        // 3. Apply pure Multipliers
        foreach (float mult in MultiplicativeModifiers)
        {
            totalDamage *= mult;
        }

        return Mathf.RoundToInt(totalDamage);
    }
}
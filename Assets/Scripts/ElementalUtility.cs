using UnityEngine;

/// <summary>
/// A static helper class to handle all elemental logic globally.
/// This keeps our math separated from our state managers and config files!
/// </summary>
public static class ElementalUtility
{
    /// <summary>
    /// Evaluates the elemental damage modifier and returns an ADDITIVE bonus (+0.5f or -0.5f)
    /// </summary>
    public static float GetElementalAdditiveBonus(ElementType attackElement, ElementType targetElement, ElementType casterElement)
    {
        ElementType finalAttackElement = attackElement == ElementType.None ? casterElement : attackElement;

        if (finalAttackElement == ElementType.None || targetElement == ElementType.None)
        {
            return 0f; // 0% Bonus
        }

        if (IsWeakAgainst(targetElement, finalAttackElement))
        {
            return CombatConfigSO.Instance.weaknessMultiplier; // Target is weak -> +50% Damage
        }

        if (IsWeakAgainst(finalAttackElement, targetElement))
        {
            return CombatConfigSO.Instance.resistanceMultiplier; // Attacker is weak -> -50% Damage
        }

        return 0f; // 0% Bonus
    }

    /// <summary>
    /// Core Elemental Cycle Matrix
    /// </summary>
    public static bool IsWeakAgainst(ElementType target, ElementType attacker)
    {
        switch (target)
        {
            case ElementType.Nature: return attacker == ElementType.Fire;
            case ElementType.Earth:  return attacker == ElementType.Nature;
            case ElementType.Light:  return attacker == ElementType.Earth;
            case ElementType.Dark:   return attacker == ElementType.Light;
            case ElementType.Water:  return attacker == ElementType.Dark;
            case ElementType.Fire:   return attacker == ElementType.Water;
            default: return false;
        }
    }
}
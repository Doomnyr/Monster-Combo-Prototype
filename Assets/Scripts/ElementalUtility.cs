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
    public static float GetElementalAdditiveBonus(MonsterElement attackElement, MonsterElement targetElement, MonsterElement casterElement)
    {
        MonsterElement finalAttackElement = attackElement == MonsterElement.Default ? casterElement : attackElement;

        if (finalAttackElement == MonsterElement.Default || targetElement == MonsterElement.Default)
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
    public static bool IsWeakAgainst(MonsterElement target, MonsterElement attacker)
    {
        switch (target)
        {
            case MonsterElement.Nature: return attacker == MonsterElement.Fire;
            case MonsterElement.Earth:  return attacker == MonsterElement.Nature;
            case MonsterElement.Light:  return attacker == MonsterElement.Earth;
            case MonsterElement.Dark:   return attacker == MonsterElement.Light;
            case MonsterElement.Water:  return attacker == MonsterElement.Dark;
            case MonsterElement.Fire:   return attacker == MonsterElement.Water;
            default: return false;
        }
    }
}
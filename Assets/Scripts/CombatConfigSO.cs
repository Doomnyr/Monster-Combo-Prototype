using UnityEngine;

/// <summary>
/// Centralized Configuration ScriptableObject.
/// Save this inside a folder named "Resources" as "CombatConfig" to enable automated static loading.
/// </summary>
[CreateAssetMenu(fileName = "CombatConfig", menuName = "MonsterCombo/Combat Config")]
public class CombatConfigSO : ScriptableObject
{
    private static CombatConfigSO _instance;
    public static CombatConfigSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<CombatConfigSO>("CombatConfig");
                if (_instance == null)
                {
                    // Fallback runtime allocation so the game never crashes if the asset hasn't been created
                    _instance = CreateInstance<CombatConfigSO>();
                    Debug.LogWarning("CombatConfigSO: No config asset found in 'Assets/Resources/CombatConfig'. Creating temporary default settings in memory.");
                }
            }
            return _instance;
        }
    }

    [Header("Elemental Damage Matrix Settings")]
    [Tooltip("The multiplier applied when an attack hits a target's elemental weakness (e.g., 1.5 for +50% damage)")]
    public float weaknessMultiplier = 1.5f;
    public float resistanceMultiplier = 0.5f;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float masterVolume = 1.0f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Global Floating Text Overrides")]
    [Tooltip("If checked, floating text will read its lifetime and speed parameters from this config instead of individual prefab scripts.")]
    public bool overrideFloatingTextSettings = true;
    public float floatingTextLifetime = 1.2f;
    public float floatingTextUpwardSpeed = 75f;

    /// <summary>
    /// Evaluates the elemental damage modifier.
    /// If attackElement is Default, it translates the attack type to match the caster's element.
    /// </summary>
    public float GetElementalMultiplier(MonsterElement attackElement, MonsterElement targetElement, MonsterElement casterElement)
    {
        // 1. Resolve "Default" to the caster's inherent type
        MonsterElement finalAttackElement = attackElement;
        if (attackElement == MonsterElement.Default)
        {
            finalAttackElement = casterElement;
        }

        // 2. Evaluate Weakness Matrix
        if (IsWeakAgainst(targetElement, finalAttackElement))
        {
            Debug.Log($"[ELEMENTAL WEAKNESS] Attack element {finalAttackElement} hit {targetElement}! Applying {weaknessMultiplier * 100}% Damage.");
            return weaknessMultiplier;
        }

        return 1.0f;
    }

    /// <summary>
    /// Matrix Evaluation Rules:
    /// - Fire -> Nature
    /// - Nature -> Earth
    /// - Earth -> Light
    /// - Light -> Dark
    /// - Dark -> Water
    /// - Water -> Fire
    /// </summary>
    private bool IsWeakAgainst(MonsterElement target, MonsterElement attacker)
    {
        switch (target)
        {
            case MonsterElement.Nature: return attacker == MonsterElement.Fire;
            case MonsterElement.Earth:  return attacker == MonsterElement.Nature;
            case MonsterElement.Light:  return attacker == MonsterElement.Earth;
            case MonsterElement.Dark:   return attacker == MonsterElement.Light;
            case MonsterElement.Water:  return attacker == MonsterElement.Dark;
            case MonsterElement.Fire:   return attacker == MonsterElement.Water;
            default: return false; // Default, non-matching, or identical elements receive normal damage
        }
    }
}
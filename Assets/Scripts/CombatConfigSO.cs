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

    public static float BURN_DAMAGE = 8;
}
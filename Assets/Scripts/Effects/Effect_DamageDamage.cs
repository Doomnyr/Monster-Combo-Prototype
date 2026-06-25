using UnityEngine;

/// <summary>
/// A highly generic, reusable SkillEffect SO that calculates combat damage
/// based on any chosen scaling stat (e.g. Strength, Intelligence, Speed)
/// using our centralized DamageCalculator and elemental relationships.
/// </summary>
[CreateAssetMenu(fileName = "Effect_Damage", menuName = "Effects/Damage")]
public class DamageEffect : SkillEffectSO
{
    [Header("Base Calculations")]
    [Tooltip("Base flat damage value of the skill before statistics or elements are calculated.")]
    [SerializeField] private int baseDamageValue = 20;

    [Tooltip("Select which stat this skill uses to scale its bonus damage.")]
    [SerializeField] private StatType scalingStat = StatType.Strength;

    [Tooltip("Scale coefficient for the selected stat (e.g., 1.5 adds 150% of the stat to damage).")]
    [SerializeField] private float statScalingCoefficient = 1.0f;

    [Header("Element Settings")]
    [Tooltip("The element type of the attack. Select 'Default' to make it automatically match the caster's inherent element.")]
    [SerializeField] private MonsterElement attackElement = MonsterElement.Default;

    public override void Apply(MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;

        // Route calculation through our centralized Accountant (DamageCalculator)
        int finalDamage = DamageCalculator.CalculateDamage(
            caster, 
            target, 
            baseDamageValue, 
            scalingStat,
            statScalingCoefficient, 
            attackElement
        );

        Debug.Log($"[COMBAT] {caster.MonsterDef.MonsterName} ({caster.MonsterDef.Element}) used a " +
                  $"{attackElement} Skill scaling with {scalingStat} on {target.MonsterDef.MonsterName} ({target.MonsterDef.Element}) for {finalDamage} damage!");

        target.TakeDamage(finalDamage);
    }
}
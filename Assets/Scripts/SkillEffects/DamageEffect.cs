using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage", menuName = "Combat/Effects/Damage")]
public class DamageEffect : SkillEffectSO
{
    [Tooltip("Base power of the attack before modifiers.")]
    [SerializeField] float _damageMultiplier = 1.0f;

        [Header("Element Settings")]
    [Tooltip("The element type of the attack. Select 'Default' to make it automatically match the caster's inherent element.")]
    [SerializeField] private MonsterElement attackElement = MonsterElement.Default;

    public override void Apply(MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;

        // 1. Calculate Base Parameter Stats
        float rawStatDamage = (caster.Strength * _damageMultiplier);
        
        // 2. Apply target defense reduction (simple subtraction clamped to a minimum of 1 damage)
        float reducedDamage = Mathf.Max(1f, rawStatDamage - target.Defense);

        // 3. Evaluate Elemental Relationship using our centralized configuration
        float elementalMod = CombatConfigSO.Instance.GetElementalMultiplier(
            attackElement, 
            target.MonsterDef.Element, 
            caster.MonsterDef.Element
        );

        // 4. Calculate Final Damage Clamped Value
        int finalDamage = Mathf.RoundToInt(reducedDamage * elementalMod);

        Debug.Log($"[COMBAT] {caster.MonsterDef.MonsterName} hit {target.MonsterDef.MonsterName} for {finalDamage} damage! " +
                  $"(Base: {rawStatDamage}, Post-Def: {reducedDamage}, Element Multiplier: {elementalMod}x)");

        target.TakeDamage(finalDamage);
    }
}
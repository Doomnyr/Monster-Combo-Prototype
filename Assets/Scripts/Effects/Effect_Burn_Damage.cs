using UnityEngine;

[CreateAssetMenu(fileName = "Effect_BurnDamage", menuName = "Effects/Burn Damage")]
public class Effect_Burn_Damage : SkillEffectSO
{
    [Tooltip("The definition of the Burn buff so we know which one to look for.")]
    public BuffDefinitionSO burnBuffDef;

    public override void Apply(MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;

        // 1. Find how many stacks of burn the target has
        int currentStacks = target.Buffs.GetBuffStacks(burnBuffDef);
        if (currentStacks <= 0) return;

        // 2. Calculate damage (Requires adding burnDamagePerStack to CombatConfigSO)
        int damage = currentStacks * CombatConfigSO.Instance.BURN_DAMAGE;

        Debug.Log($"[BURN] {target.MonsterDef.MonsterName} took {damage} burn damage from {currentStacks} stacks!");
        
        // 3. Deal true damage (DOTs usually bypass the DamageCalculator and Defense)
        target.TakeDamage(damage);

        // 4. Reduce stacks by 1
        target.Buffs.RemoveBuffStacks(burnBuffDef, 1);
    }
}
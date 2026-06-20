using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage", menuName = "Combat/Effects/Damage")]
public class DamageEffect : SkillEffectSO
{
    [Tooltip("Base power of the attack before modifiers.")]
    [SerializeField] float _damageMultiplier = 1.0f;

    public override void Apply(MonsterInstance caster, MonsterInstance target)
    {
        if (target.IsDefeated) return;

        // Update this damage calculator
        float finalDamage = caster.MonsterDef.BaseStats.attack * _damageMultiplier - target.MonsterDef.BaseStats.defense;
        finalDamage = Mathf.Max(1, finalDamage);
        target.TakeDamage((int)finalDamage);
        
        Debug.Log($"{caster.MonsterDef.MonsterName} dealt {finalDamage} damage to {target.MonsterDef.MonsterName}!");
    }
}
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage_Burn", menuName = "Combat/Effects/Effect_Damage_Burn")]
public class Effect_Damage_Burn : SkillEffectSO
{
    public BuffType _requiredBuffType = BuffType.Burn;
    
    public override void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;

        int stackMultiplier = target.Buffs.GetBuffStacks(_requiredBuffType);
        float finalDamage = skill.baseValue * stackMultiplier;

        Debug.Log($"[END OF TURN] {caster.MonsterDef.MonsterName} takes ({finalDamage}) of burn damage!");

        target.ApplyDamage((int)finalDamage, caster);
    }
}
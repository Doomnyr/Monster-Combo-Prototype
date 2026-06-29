using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Damage_Burn", menuName = "Combat/Effects/Effect_Damage_Burn")]
public class Effect_Damage_Burn : SkillEffectSO
{
    public BuffType _requiredBuffType = BuffType.Burn;
    
    public override void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsAlive) return;
        
        var matchingBuff = target.ActiveBuffs.FirstOrDefault(b => b.BuffDef.Type == _requiredBuffType);
        
        int stackMultiplier = (matchingBuff != null) ? matchingBuff.CurrentStacks : 0;
        float finalDamage = skill.baseValue * stackMultiplier;
        
        // Route calculation through our centralized Accountant (DamageCalculator)
        //int finalDamage = DamageCalculator.CalculateFlatDamage(
        //    caster, 
        //    target, 
        //   (int)skill.baseValue
        //);

        Debug.Log($"[END OF TURN] {caster.MonsterDef.Name} takes ({finalDamage}) of burn damage!");

        target.TakeDamage((int)finalDamage);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifyMana", menuName = "Combat/Effects/ModifyMana")]
public class Effect_ModifyMana : SkillEffectSO
{
    public override void Apply(MonsterInstance caster, MonsterInstance target, SkillAction skill)
    {
        if (target == null || target.IsDefeated)
            return;

        float manaGained = target.MaxMana * skill.scaleCoefficient;
            
        target.CurrentMana += manaGained;
        Debug.Log($"[Skill Engine] {target.MonsterDef.MonsterName} recovered {manaGained} mana.");

    }
}
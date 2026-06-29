using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifyMana", menuName = "Effects/ModifyMana")]
public class ModifyManaEffect : SkillEffectSO
{
    [Tooltip("Percentage of Max Mana to restore (e.g., 0.10 equals 10%).")]
    [SerializeField] private float maxManaPercentage = 0.10f;

    public override void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target)
    {
        if (target == null || target.IsDefeated)
            return;

        float manaGained = target.MaxMana * maxManaPercentage;
            
        target.CurrentMana += manaGained;
        Debug.Log($"[Skill Engine] {target.MonsterDef.Name} recovered {manaGained} mana.");

    }
}
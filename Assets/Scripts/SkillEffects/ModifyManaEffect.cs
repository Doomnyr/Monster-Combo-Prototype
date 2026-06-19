using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifyManaEffect : SkillEffect
{
    [Tooltip("Percentage of Max Mana to restore (e.g., 0.10 equals 10%).")]
    [SerializeField] private float maxManaPercentage = 0.10f;

    public override void Execute(MonsterInstance caster, List<MonsterInstance> targets)
    {
        // Loops through whatever targets the system provides based on the dropdown choice
        foreach (MonsterInstance target in targets)
        {
            if (target == null || target.IsDefeated) continue;

            float manaGained = target.MaxMana * maxManaPercentage;
            
            target.CurrentMana += manaGained;
            Debug.Log($"[Skill Engine] {target.MonsterDef.MonsterName} recovered {manaGained} mana.");
        }
    }
}
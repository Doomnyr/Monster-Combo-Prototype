using System;
using UnityEngine;

[Serializable]
public class ModifyManaEffect : SkillEffect
{
    [Tooltip("Percentage value of Max Mana to restore (e.g., 0.10 equals 10%).")]
    [SerializeField] private float maxManaPercentage = 0.10f;

    public override void Execute(MonsterInstance caster, MonsterInstance target)
    {
        // This calculates modifications based on the target of the effect execution block
        float manaGained = target.MaxMana * maxManaPercentage;
        
        target.CurrentMana += manaGained;
        Debug.Log($"[Skill Engine] {target.MonsterDef.MonsterName} recovered {manaGained} mana.");
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_ApplyBuff", menuName = "MonsterCombo/Effects/Apply_Buff")]
public class Effect_ApplyBuff : SkillEffectSO
{
    public override void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target)
    {
        if (target.IsDefeated) return;

        Debug.Log("EFFECT: Apply Buff");
        target.Buffs.AddBuff(skill.buffToApply, skill.buffCount);
    }
}
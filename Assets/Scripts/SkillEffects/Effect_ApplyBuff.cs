using UnityEngine;

[CreateAssetMenu(fileName = "Effect_ApplyBuff", menuName = "Combat/Effects/Apply Buff")]
public class Effect_ApplyBuff : SkillEffectSO
{
    public override void Apply(SkillExecutionContext context, SkillAction skill)
    {
        // Use the targets stored in our execution memory
        foreach (var target in context.LastTargets)
        {
            if (target.IsDefeated) continue;

            // Use the parameters passed in via the Action object
            target.AddBuff(skill.buffToApply, skill.buffCount, skill.buffDuration);
        }
    }
}
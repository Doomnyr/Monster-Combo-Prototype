using UnityEngine;

/// <summary>
/// The SkillEffect payload that actually puts the status onto a monster.
/// </summary>
[CreateAssetMenu(fileName = "Effect_ApplyBuff", menuName = "MonsterCombo/Effects/Apply Status")]
public class Effect_ApplyBuff : SkillEffectSO
{
    [Tooltip("The Status Blueprint to apply")]
    public BuffDefinitionSO buffToApply;
    
    [Tooltip("How many stacks to apply per hit")]
    public int stacksToApply = 1;

    [Tooltip("How many turns it lasts (-1 for infinite)")]
    public int duration = 3;

    public override void Apply(MonsterInstance caster, MonsterInstance target)
    {
        if (!target.IsDefeated) return;

        // Note: You will need to add a method on MonsterInstance like AddStatus()
        // that creates the StatusInstance and holds it in a List<StatusInstance>
        target.AddBuff(buffToApply, stacksToApply, duration);
    }
}
using System.Collections.Generic;
using UnityEngine;

public static class CombatActionExecutor
{
    public static void ExecuteSkillAction(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        if (action == null || action.targetFinder == null) return;

        List<MonsterInstance> targets = action.targetFinder.FindTargets(action, caster, battlefield, null);
        foreach (MonsterInstance target in targets)
        {
            foreach (SkillEffectSO effect in action.executionEffect)
            {
                if (target != null && target.IsAlive)
                {
                    effect.Apply(action, caster, target);
                }
            }
        }
    }
}

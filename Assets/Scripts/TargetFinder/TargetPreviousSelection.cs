using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_PreviousSelection", menuName = "TargetFinder/Target_PreviousSelection")]
public class Target_PreviousSelection : TargetFinderSO
{
    // 1. You MUST implement this because it is 'abstract' in the base class
    public override List<MonsterInstance> FindTargets(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        // If someone calls the old version, just pass an empty list 
        // (or handle it however you prefer if no previous targets exist)
        return FindTargets(action, caster, battlefield, new List<MonsterInstance>());
    }

    // 2. This is your new logic
    public override List<MonsterInstance> FindTargets(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield, List<MonsterInstance> sessionTargets)
    {
        if (sessionTargets != null && sessionTargets.Count > 0)
        {
            return sessionTargets;
        }
        
        Debug.LogWarning("Target_PreviousSelection: No session targets found!");
        return new List<MonsterInstance>();
    }
}

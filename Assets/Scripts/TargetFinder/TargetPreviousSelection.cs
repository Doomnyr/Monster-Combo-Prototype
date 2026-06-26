using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_PreviousSelection", menuName = "TargetFinder/Target_PreviousSelection")]
public class Target_PreviousSelection : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(SkillAction action, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        // Simply return the list we saved in the previous step
        if (action.previousTargets != null && action.previousTargets.Count > 0)
        {
            return action.previousTargets;
        }

        Debug.LogWarning("TargetFinder_Previous: No previous targets found! Returning empty list.");
        return new List<MonsterInstance>();
    }
}

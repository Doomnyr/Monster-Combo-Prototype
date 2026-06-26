using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetFollowUp", menuName = "TargetFinder/TargetFollow")]
public class TargetFinder_FollowUp : TargetFinderSO
{
    // You'll need to pass the context into your FindTargets method
    public override List<MonsterInstance> FindTargets(SkillExecutionContext context, List<MonsterInstance> battlefield)
    {
        // Simply return what was hit last time!
        return context.LastTargets;
    }
}

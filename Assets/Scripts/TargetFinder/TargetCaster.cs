using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetFrontRowFirst_Enemy_Random", menuName = "TargetFinder/TargetFrontRowFirst_Enemy_Random")]
public class TargetSelf : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        selectedTargets.Add(caster);

        return selectedTargets;
    }
}

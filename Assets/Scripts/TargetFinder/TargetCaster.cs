using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetCaster", menuName = "TargetFinder/TargetCaster")]
public class TargetCaster : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        selectedTargets.Add(caster);

        return selectedTargets;
    }
}

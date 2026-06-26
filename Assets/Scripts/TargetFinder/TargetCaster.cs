using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_Caster", menuName = "TargetFinder/Target_Caster")]
public class Target_Caster : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(SkillAction skill, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        selectedTargets.Add(caster);

        return selectedTargets;
    }
}

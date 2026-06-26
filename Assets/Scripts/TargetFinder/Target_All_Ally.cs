using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_All_Ally", menuName = "TargetFinder/Target_All_Ally")]
public class Target_All_Ally : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(SkillAction skill, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        
        foreach (var monster in battlefield)
        {
            if (monster.Team == caster.Team && !monster.IsDefeated)
            {
                selectedTargets.Add(monster);
            }
        }

        return selectedTargets;
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_FrontRowAll_Enemy", menuName = "TargetFinder/Target_FrontRowAll_Enemy")]
public class Target_FrontRowAll_Enemy : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(SkillAction skill, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        List<MonsterInstance> validFrontlineEnemies = new List<MonsterInstance>();
        List<MonsterInstance> validBacklineEnemies = new List<MonsterInstance>();

        // Sort living enemies into frontline or backline pools
        foreach (var monster in battlefield)
        {
            if (monster.Team != caster.Team && !monster.IsDefeated)
            {
                if (monster.gridPosition.Column == 0)
                {
                    selectedTargets.Add(monster);
                }
            }
        }

        return selectedTargets;
    }
}

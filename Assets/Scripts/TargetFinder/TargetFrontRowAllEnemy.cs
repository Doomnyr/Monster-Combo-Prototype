using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_FrontRow_All_Enemy", menuName = "TargetFinder/Target_FrontRow_All_Enemy")]
public class Target_FrontRow_All_Enemy : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(MonsterInstance caster, List<MonsterInstance> battlefield)
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

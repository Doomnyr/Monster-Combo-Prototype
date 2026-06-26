using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target_FrontRowFirst_Enemy_Random", menuName = "TargetFinder/Target_FrontRowFirst_Enemy_Random")]
public class Target_FrontRowFirst_Enemy_Random : TargetFinderSO
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
                    validFrontlineEnemies.Add(monster);
                }
                else if (monster.gridPosition.Column == 1)
                {
                    validBacklineEnemies.Add(monster);
                }
            }
        }

        // Prioritize Frontline
        if (validFrontlineEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, validFrontlineEnemies.Count);
            selectedTargets.Add(validFrontlineEnemies[randomIndex]);
        }
        // Fallback to Backline
        else if (validBacklineEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, validBacklineEnemies.Count);
            selectedTargets.Add(validBacklineEnemies[randomIndex]);
        }

        return selectedTargets;
    }
}

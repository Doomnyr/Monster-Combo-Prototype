using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetFrontRowFirst_Enemy_Random", menuName = "TargetFinder/TargetFrontRowFirst_Enemy_Random")]
public class TargetFrontRowFirst_Enemy_Random : TargetFinderSO
{
    public override List<MonsterInstance> FindTargets(SkillExecutionContext context, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> selectedTargets = new List<MonsterInstance>();
        List<MonsterInstance> validFrontlineEnemies = new List<MonsterInstance>();
        List<MonsterInstance> validBacklineEnemies = new List<MonsterInstance>();

        // Sort living enemies into frontline or backline pools
        foreach (var monster in battlefield)
        {
            if (monster.Team != context.Caster.Team && !monster.IsDefeated)
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

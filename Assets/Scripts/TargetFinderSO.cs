using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFinderSO : ScriptableObject
{
    public abstract List<MonsterInstance> FindTargets
    (
        SkillAction skill, 
        MonsterInstance caster, 
        List<MonsterInstance> battlefield);
    public virtual List<MonsterInstance> FindTargets
    (
        SkillAction action, 
        MonsterInstance caster, 
        List<MonsterInstance> battlefield, 
        List<MonsterInstance> lastKnownTargets
    )
        {
            return FindTargets(action, caster, battlefield);
        }
}

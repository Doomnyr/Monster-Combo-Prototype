using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFinderSO : ScriptableObject
{
    public abstract List<MonsterInstance> FindTargets(SkillExecutionContext context, List<MonsterInstance> battlefield);
}

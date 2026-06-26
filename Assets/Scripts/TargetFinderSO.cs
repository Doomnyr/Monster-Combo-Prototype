using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFinderSO : ScriptableObject
{
    public abstract List<MonsterInstance> FindTargets(SkillAction skill, MonsterInstance caster, List<MonsterInstance> battlefield);
}

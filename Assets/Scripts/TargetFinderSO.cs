using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFinderSO : ScriptableObject
{
    /// <summary>
    /// The universal blueprint for finding targets. 
    /// Every custom targeting style you create will override this method.
    /// </summary>
    /// <param name="caster">The monster casting the skill.</param>
    /// <param name="battlefield">All active monsters currently in the fight.</param>
    /// <returns>A list of monsters that match the targeting rules.</returns>
    public abstract List<MonsterInstance> FindTargets(MonsterInstance caster, List<MonsterInstance> battlefield);
}

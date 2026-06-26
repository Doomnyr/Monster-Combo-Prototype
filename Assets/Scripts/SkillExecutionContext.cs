using System.Collections.Generic;

public class SkillExecutionContext
{
    public MonsterInstance Caster;
    
    // A dictionary where you can store lists of monsters under names like "TargetA", "Hit_1", "Debuffed_Units"
    private Dictionary<string, List<MonsterInstance>> _blackboard = new Dictionary<string, List<MonsterInstance>>();

    public void RegisterTargets(string key, List<MonsterInstance> targets) => _blackboard[key] = targets;
    public List<MonsterInstance> GetTargets(string key) => _blackboard.ContainsKey(key) ? _blackboard[key] : new List<MonsterInstance>();
}
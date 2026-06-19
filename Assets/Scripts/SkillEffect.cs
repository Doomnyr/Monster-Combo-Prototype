using System;

[Serializable]
public abstract class SkillEffect
{
    /// <summary>
    /// Executes the core logic of this specific effect sequence.
    /// </summary>
    /// <param name="caster">The monster executing the action.</param>
    /// <param name="target">The monster receiving the consequence of this effect block.</param>
    public abstract void Execute(MonsterInstance caster, MonsterInstance target);
}
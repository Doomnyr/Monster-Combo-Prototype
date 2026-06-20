using UnityEngine;

// We do not add [CreateAssetMenu] here because this is the abstract blueprint file!
public abstract class SkillEffectSO : ScriptableObject
{
    /// <summary>
    /// Applies a single instance of this effect to an individual target.
    /// The high-level combat loop handles iterating this for multi-target skills.
    /// </summary>
    /// <param name="caster">The runtime instance of the monster casting the skill.</param>
    /// <param name="target">The individual runtime instance receiving this effect.</param>
    public abstract void Apply(MonsterInstance caster, MonsterInstance target);
}
using UnityEngine;

// We do not add [CreateAssetMenu] here because this is the abstract blueprint file!
public abstract class SkillEffectSO : ScriptableObject
{
    public abstract void Apply(SkillAction skill, MonsterInstance caster, MonsterInstance target);
}
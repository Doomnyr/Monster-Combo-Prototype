using UnityEngine;

public class BuffInstance
{
    public BuffDefinitionSO BuffDef { get; private set; }
    public MonsterInstance BuffCaster;
    public int CurrentStacks { get; private set; }
    public bool IsPermanent;

    public BuffInstance(BuffDefinitionSO definition, int initialStacks, bool isPermanent, MonsterInstance caster)
    {
        BuffDef = definition;
        CurrentStacks = initialStacks;
        IsPermanent = isPermanent;
        BuffCaster = caster;
    }

    public void AddStacks(int amount)
    {
        if (!BuffDef.infiniteStacks && CurrentStacks > BuffDef.maxStacks)
        {
            CurrentStacks = BuffDef.maxStacks;
        }
        else
        {
            CurrentStacks += amount;
        }
        
        Debug.Log($"[{BuffDef.buffName}] stacked to {CurrentStacks}!");
    }

    public void RemoveStacks(int amount)
    {
        CurrentStacks = Mathf.Max(0, CurrentStacks - amount);
    }

    public void Tick()
    {
        if (!IsPermanent && CurrentStacks > 0)
        {
            CurrentStacks--;
        }
    }

    public bool IsExpired => !IsPermanent && CurrentStacks <= 0;
}
using UnityEngine;

public class BuffInstance
{
    public BuffDefinitionSO BuffDef { get; private set; }
    public int CurrentStacks { get; private set; }

    // Duration is now intrinsically linked to stacks. 
    // If this is -1, it is a permanent buff (never ticks down).
    public int RemainingDuration => CurrentStacks; 
    private bool _isPermanent;

    public BuffInstance(BuffDefinitionSO definition, int initialStacks, bool isPermanent)
    {
        BuffDef = definition;
        CurrentStacks = initialStacks;
        _isPermanent = isPermanent;
    }

    public void AddStacks(int amount)
    {
        CurrentStacks += amount;

        if (!BuffDef.infiniteStacks && CurrentStacks > BuffDef.maxStacks)
        {
            CurrentStacks = BuffDef.maxStacks;
        }
        
        Debug.Log($"[{BuffDef.buffName}] stacked to {CurrentStacks}!");
    }

    public void RemoveStacks(int amount)
    {
        CurrentStacks = Mathf.Max(0, CurrentStacks - amount);
    }

    public void Tick()
    {
        if (!_isPermanent && CurrentStacks > 0)
        {
            CurrentStacks--;
        }
    }

    public bool IsExpired => !_isPermanent && CurrentStacks <= 0;
}
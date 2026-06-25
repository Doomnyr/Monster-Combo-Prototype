using UnityEngine;

public class BuffInstance
{
    public BuffDefinitionSO BuffDef { get; private set; }
    public int CurrentStacks { get; private set; }
    public int RemainingDuration { get; private set; } // -1 for infinite

    // Constructor to create it
    public BuffInstance(BuffDefinitionSO definition, int initialStacks, int duration)
    {
        BuffDef = definition;
        CurrentStacks = initialStacks;
        RemainingDuration = duration;
    }

    // Handles adding stacks while respecting the SO rules
    public void AddStacks(int amount)
    {
        CurrentStacks += amount;

        if (!BuffDef.infiniteStacks && CurrentStacks > BuffDef.maxStacks)
        {
            CurrentStacks = BuffDef.maxStacks;
        }
        
        Debug.Log($"[{BuffDef.buffName}] stacked to {CurrentStacks}!");
    }

    public void DecreaseDuration()
    {
        if (RemainingDuration > 0)
        {
            RemainingDuration--;
        }
    }

    public void RemoveStacks(int amount)
    {
        CurrentStacks -= amount;
        if (CurrentStacks < 0) CurrentStacks = 0;
    }

    public void SetRemainingDuration(int duration)
    {
        RemainingDuration = Mathf.Max(0, duration);
    }

    public bool IsExpired => RemainingDuration == 0;
}
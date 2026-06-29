using System;
using UnityEngine;

public class BuffInstance
{
    public BuffDefinitionSO BuffDef { get; private set; }
    public int CurrentStacks { get; private set; }
    public bool IsPermanent;

    public BuffInstance(BuffDefinitionSO buffDef, int stacks, bool isPermanent)
    {
        BuffDef = buffDef;
        CurrentStacks = Math.Clamp(stacks, 1, BuffDef.MaxStacks);
        IsPermanent = isPermanent;
    }

    public void AddStacks(int amount)
    {   
        if(CurrentStacks + amount > BuffDef.MaxStacks)
        {
            CurrentStacks = BuffDef.MaxStacks;
        }
        else
        {
            CurrentStacks += amount;
        }
        
        Debug.Log($"[{BuffDef.name}] stacked to {CurrentStacks}!");
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
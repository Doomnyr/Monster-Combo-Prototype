using UnityEngine;

public class BuffInstance
{
    public BuffDefinitionSO BuffDef { get; private set; }
    public int CurrentStacks { get; private set; }
    private bool _isPermanent;

    public BuffInstance(BuffDefinitionSO definition, int initialStacks, bool isPermanent)
    {
        BuffDef = definition;
        CurrentStacks = initialStacks;
        _isPermanent = isPermanent;
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
        if (!_isPermanent && CurrentStacks > 0)
        {
            CurrentStacks--;
        }
    }

    public bool IsExpired => !_isPermanent && CurrentStacks <= 0;
}
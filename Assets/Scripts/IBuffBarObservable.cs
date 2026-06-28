using System;
using System.Collections.Generic;

public interface IBuffBarObservable
{
    // Exposes the list of active buffs for the UI to iterate over and draw
    IReadOnlyList<BuffInstance> ActiveBuffs { get; }
    
    // Fires whenever a buff is added, removed, or has its stacks/duration changed
    event Action OnBuffsChanged; 
}

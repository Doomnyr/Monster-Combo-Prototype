using System;

public interface IManaObservable
{
    float CurrentMana { get; }
    float MaxMana { get; }
    event Action<float, float> OnManaChanged; // Passes (current, max)
}
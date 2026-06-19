using System;

public interface IHealthObservable
{
    float CurrentHP { get; }
    float MaxHP { get; }
    event Action<float, float> OnHPChanged; // Passes (current, max)
}

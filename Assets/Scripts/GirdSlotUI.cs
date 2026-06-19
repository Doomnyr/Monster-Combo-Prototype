using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridSlotUI : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TextMeshProUGUI nameText;

    // The visual slot holds references to the contracts, not the concrete class
    private IHealthObservable boundHealthTarget;
    private IManaObservable boundManaTarget;

    /// <summary>
    /// Binds the UI elements to any data entities that implement health and mana monitoring.
    /// </summary>
    public void Bind(string displayName, IHealthObservable healthTarget, IManaObservable manaTarget)
    {
        Unbind(); // Clean up existing listeners safely

        nameText.text = displayName;

        // Bind Health Pipeline
        boundHealthTarget = healthTarget;
        if (boundHealthTarget != null)
        {
            boundHealthTarget.OnHPChanged += UpdateHealthVisuals;
            UpdateHealthVisuals(boundHealthTarget.CurrentHP, boundHealthTarget.MaxHP);
        }

        // Bind Mana Pipeline
        boundManaTarget = manaTarget;
        if (boundManaTarget != null)
        {
            boundManaTarget.OnManaChanged += UpdateManaVisuals;
            UpdateManaVisuals(boundManaTarget.CurrentMana, boundManaTarget.MaxMana);
        }
    }

    private void UpdateHealthVisuals(float currentHP, float maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }

    private void UpdateManaVisuals(float currentMana, float maxMana)
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;
    }

    public void Unbind()
    {
        if (boundHealthTarget != null)
        {
            boundHealthTarget.OnHPChanged -= UpdateHealthVisuals;
            boundHealthTarget = null;
        }

        if (boundManaTarget != null)
        {
            boundManaTarget.OnManaChanged -= UpdateManaVisuals;
            boundManaTarget = null;
        }
    }

    private void OnDestroy()
    {
        Unbind();
    }
}
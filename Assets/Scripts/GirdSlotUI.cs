using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TextMeshProUGUI currentHealthAmount;
    [SerializeField] private TextMeshProUGUI currentManaAmount;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image _monsterSprite;
    
    [Header("Buff Bar")]
    [SerializeField] private GameObject _buffBar; // Must have a Horizontal/Grid Layout Group!
    [SerializeField] private GameObject _buffIconPrefab; // Drag your BuffIconUI prefab here

    private IHealthObservable boundHealthTarget;
    private IManaObservable boundManaTarget;
    private IBuffBarObservable boundBuffTarget;

    public void Bind(string displayName, Sprite sprite, IHealthObservable healthTarget, IManaObservable manaTarget, IBuffBarObservable buffTarget)
    {
        Unbind(); 

        nameText.text = displayName;
        _monsterSprite.sprite = sprite;

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

        // Bind Buff Pipeline
        boundBuffTarget = buffTarget;
        if (boundBuffTarget != null)
        {
            boundBuffTarget.OnBuffsChanged += UpdateBuffVisuals;
            UpdateBuffVisuals(); // Draw them immediately on spawn
        }
    }

    private void UpdateBuffVisuals()
    {
        // 1. Wipe the old icons
        foreach (Transform child in _buffBar.transform)
        {
            Destroy(child.gameObject);
        }

        if (boundBuffTarget == null) return;

        // 2. Spawn the new updated icons
        foreach (var buff in boundBuffTarget.ActiveBuffs)
        {
            GameObject iconObj = Instantiate(_buffIconPrefab, _buffBar.transform);
            BuffIconUI iconUI = iconObj.GetComponent<BuffIconUI>();
            if (iconUI == null)
            {
                iconUI = iconObj.GetComponentInChildren<BuffIconUI>();
            }

            if (iconUI != null)
            {
                iconUI.Setup(buff.BuffDef, buff.CurrentStacks);
            }
            else
            {
                // Fallback: if prefab is a simple Image (older prefab), try to set sprite directly
                var img = iconObj.GetComponent<UnityEngine.UI.Image>() ?? iconObj.GetComponentInChildren<UnityEngine.UI.Image>();
                if (img != null)
                {
                    img.sprite = buff.BuffDef?.buffIcon;
                    img.gameObject.SetActive(buff.BuffDef?.buffIcon != null);
                    img.color = buff.BuffDef != null && buff.BuffDef.isDebuff ? new UnityEngine.Color(1f, 0.5f, 0.5f) : UnityEngine.Color.white;

                    // Try to find a TMP text for stacks
                    var stacksTMP = iconObj.GetComponent<TMPro.TextMeshProUGUI>() ?? iconObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (stacksTMP != null)
                    {
                        if (buff.CurrentStacks > 1)
                        {
                            stacksTMP.text = buff.CurrentStacks.ToString();
                            stacksTMP.gameObject.SetActive(true);
                        }
                        else
                        {
                            stacksTMP.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Spawned buff icon prefab does not contain a BuffIconUI or Image component.");
                }
            }
        }
    }

    // ... [UpdateHealthVisuals and UpdateManaVisuals remain exactly the same] ...
    private void UpdateHealthVisuals(float currentHP, float maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
        currentHealthAmount.text = currentHP.ToString();
    }

    private void UpdateManaVisuals(float currentMana, float maxMana)
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;
        currentManaAmount.text = currentMana.ToString();
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

        if (boundBuffTarget != null)
        {
            boundBuffTarget.OnBuffsChanged -= UpdateBuffVisuals;
            boundBuffTarget = null;
        }
    }

    private void OnDestroy() => Unbind();
}
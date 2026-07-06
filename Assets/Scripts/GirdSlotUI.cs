using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways] // Force this script to run in the Editor without clicking Play!
[RequireComponent(typeof(RectTransform))]
public class GridSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TextMeshProUGUI nameText;
    
    [Header("In-Editor Preview Target")]
    [Tooltip("Drag your HUDAnchor from the Scene View here to see them align live!")]
    [SerializeField] private Transform _hudAnchorTarget;

    [Header("Buff Bar")]
    [SerializeField] private GameObject _buffBar; // Must have a Horizontal/Grid Layout Group!
    [SerializeField] private GameObject _buffIconPrefab; // Drag your BuffIconUI prefab here

    [Header("Turn Management Visuals")]
    [Tooltip("Drag the hollow highlight border image gameobject here")]
    [SerializeField] private GameObject _turnHighlightOverlay; 

    [Header("Floating Settings")]
    [Tooltip("The offset distance above the monster's head in world coordinates")]
    [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1.5f, 0f);

    // Caches the bound monster reference so we can easily compare who is active
    public MonsterInstance BoundMonster { get; private set; }

    private IHealthObservable boundHealthTarget;
    private IManaObservable boundManaTarget;
    private IBuffBarObservable boundBuffTarget;

    // Trackers for screen-space following
    private RectTransform _rectTransform;
    private Transform _worldTarget;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Binds the UI elements to the monster's statistics and links it to a physical world target to follow.
    /// </summary>
    public void Bind(string displayName, Transform worldTarget, IHealthObservable healthTarget, IManaObservable manaTarget, IBuffBarObservable buffTarget)
    {
        Unbind(); 

        nameText.text = displayName;
        _worldTarget = worldTarget;

        // Cache the concrete monster instance safely using the interface conversion
        BoundMonster = buffTarget as MonsterInstance;

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

        // Default the turn highlight to inactive on binding
        SetTurnHighlight(false);
    }

    public void SetTurnHighlight(bool isCurrentTurn)
    {
        if (_turnHighlightOverlay != null)
        {
            _turnHighlightOverlay.SetActive(isCurrentTurn);
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
            if (iconObj.TryGetComponent<BuffIconUI>(out var iconUI))
            {
                iconUI.Setup(buff.BuffDef, buff.CurrentStacks);
            }
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
        BoundMonster = null;
        _worldTarget = null;

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
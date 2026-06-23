using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.InputSystem;

public class CombatTooltipManager : MonoBehaviour
{
    public static CombatTooltipManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI buffsText;

    [Header("Pin Settings")]
    [SerializeField] private float screenMargin = 40f; // Gap between the tooltip and the screen edge

    private Canvas _canvas;
    private RectTransform _panelRect;
    private MonsterInstance _activeMonster;
    private Vector2 _lastHoverScreenPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _canvas = GetComponentInParent<Canvas>();
        _panelRect = tooltipPanel.GetComponent<RectTransform>();

        // Force anchors to the bottom-left (0,0) of the parent canvas so our manual positioning math works perfectly
        if (_panelRect != null)
        {
            _panelRect.anchorMin = new Vector2(0f, 0f);
            _panelRect.anchorMax = new Vector2(0f, 0f);
            _panelRect.pivot = new Vector2(0f, 1f); // Force Top-Left pivot for vertical clamping math
        }

        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf && _activeMonster != null)
        {
            UpdateTooltipPosition();
        }
    }

    public void ShowTooltip(MonsterInstance monster, Vector2 screenPosition)
    {
        if (monster == null) return;

        _activeMonster = monster;
        _lastHoverScreenPosition = screenPosition;
        tooltipPanel.SetActive(true);
        var baseStats = monster.MonsterDef.BaseStats;

        // 1. Set Identity
        nameText.text = $"{monster.MonsterDef.MonsterName}";

        // 2. Format Core Stats string
        StringBuilder statsSb = new StringBuilder();
        statsSb.AppendLine($"HP: {monster.CurrentHP} / {baseStats.maxHP}");
        statsSb.AppendLine($"Mana: {monster.CurrentMana} / {baseStats.maxMana}");
        statsSb.AppendLine($"ATK: {monster.Strength} <size=80%>(Base: {baseStats.strength})</size>");
        statsSb.AppendLine($"DEF: {monster.Defense} <size=80%>(Base: {baseStats.defense})</size>");
        statsSb.AppendLine($"SPD: {baseStats.speed}");
        statsText.text = statsSb.ToString();

        // 3. Format Buffs/Debuffs string with rich text color tags
        StringBuilder buffsSb = new StringBuilder();
        if (monster.ActiveBuffs == null || monster.ActiveBuffs.Count == 0)
        {
            buffsSb.Append("<color=#888888>No Active Buffs</color>");
        }
        else
        {
            foreach (var buff in monster.ActiveBuffs)
            {
                string colorTag = buff.BuffDef.isDebuff ? "<color=#FF4444>" : "<color=#44FF44>";
                string duration = buff.RemainingDuration == -1 ? "Perm" : $"{buff.RemainingDuration}t";
                buffsSb.AppendLine($"{colorTag}• {buff.BuffDef.buffName} x{buff.CurrentStacks}</color> ({duration})");
            }
        }
        buffsText.text = buffsSb.ToString();
        
        UpdateTooltipPosition();
    }

    public void HideTooltip()
    {
        _activeMonster = null;
        tooltipPanel.SetActive(false);
    }

    private void UpdateTooltipPosition()
    {
        Vector2 mousePos = _lastHoverScreenPosition;

        // Safely poll the active pointing hardware using the New Input System, but keep a fallback to the last known hover point.
        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
            _lastHoverScreenPosition = mousePos;
        }
        else if (Pointer.current != null)
        {
            mousePos = Pointer.current.position.ReadValue();
            _lastHoverScreenPosition = mousePos;
        }

        // Get the parent RectTransform (usually the Canvas RectTransform)
        RectTransform parentRect = _panelRect.parent as RectTransform;
        if (parentRect == null) return;

        // Convert the screen mouse position into local space relative to the Canvas parent rect
        Camera uiCamera = (_canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _canvas.worldCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mousePos, uiCamera, out Vector2 localPoint))
        {
            return;
        }

        // Convert local point coordinate system so that (0,0) is strictly the bottom-left of the canvas
        Vector2 localMousePosFromBottomLeft = localPoint - parentRect.rect.min;

        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float panelWidth = _panelRect.rect.width;
        float panelHeight = _panelRect.rect.height;

        float targetX = 0f;

        bool hoverOnLeft = mousePos.x < (parentWidth * 0.5f);
        if (hoverOnLeft)
        {
            // Hovering on the left side => display the tooltip on the right.
            targetX = parentWidth - panelWidth - screenMargin;
        }
        else
        {
            // Hovering on the right side => display the tooltip on the left.
            targetX = screenMargin;
        }

        // Clamp the vertical Y position within the safe canvas bounds (using our Top-Left pivot rules)
        float clampedY = Mathf.Clamp(localMousePosFromBottomLeft.y, panelHeight + screenMargin, parentHeight - screenMargin);

        // Update the anchored position safely
        _panelRect.anchoredPosition = new Vector2(targetX, clampedY);
    }
}
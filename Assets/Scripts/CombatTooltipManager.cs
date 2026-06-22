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

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(15f, 15f);

    private Canvas _canvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _canvas = GetComponentInParent<Canvas>();
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void ShowTooltip(MonsterInstance monster)
    {
        if (monster == null) return;

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
        tooltipPanel.SetActive(false);
    }

    private void UpdateTooltipPosition()
    {
        Vector2 mousePos = Vector2.zero;

        // Safely poll the active pointing hardware using the New Input System
        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
        }
        else if (Pointer.current != null)
        {
            mousePos = Pointer.current.position.ReadValue();
        }
        
        // Offset the window slightly so it doesn't clip directly underneath the mouse cursor
        tooltipPanel.transform.position = mousePos + offset;
    }
}
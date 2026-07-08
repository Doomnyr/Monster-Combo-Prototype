using System;
using UnityEngine;

[DisallowMultipleComponent]
public class CombatFloatingTextManager : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Drag your CombatManager from the scene here, or let the manager find it automatically.")]
    [SerializeField] private CombatManager _combatManager;
    [Tooltip("Optional reference to CombatUIController for world-space anchor lookup.")]
    [SerializeField] private CombatUIController _combatUIController;

    [Header("Floating Text")]
    [Tooltip("Drag a FloatingText prefab here. It must contain a RectTransform and FloatingText/FloatingTextController component.")]
    [SerializeField] private GameObject _floatingTextPrefab;
    [Tooltip("If this is left blank, the manager will try to spawn under the assigned CombatUIController or itself.")]
    [SerializeField] private Transform _defaultSpawnParent;
    [Tooltip("Optional UI Camera used for screen-space Canvas position conversion.")]
    [SerializeField] private Camera _uiCamera;

    private void Awake()
    {
        if (_combatManager == null)
        {
            _combatManager = FindAnyObjectByType<CombatManager>();
        }

        if (_combatUIController == null)
        {
            _combatUIController = FindAnyObjectByType<CombatUIController>();
        }

        if (_defaultSpawnParent == null)
        {
            _defaultSpawnParent = this.transform;
        }

        if (_uiCamera == null)
        {
            _uiCamera = Camera.main;
        }
    }

    private void Start()
    {
        if (_combatManager == null)
        {
            Debug.LogError("[CombatFloatingTextManager] No CombatManager found in the scene.", this);
            return;
        }

        if (_floatingTextPrefab == null)
        {
            Debug.LogError("[CombatFloatingTextManager] No FloatingText prefab assigned.", this);
            return;
        }

        _combatManager.EventDispatcher.DamageApplied += OnDamageApplied;
        _combatManager.EventDispatcher.BuffApplied += OnBuffApplied;
        _combatManager.EventDispatcher.BuffRemoved += OnBuffRemoved;
    }

    private void OnDestroy()
    {
        if (_combatManager?.EventDispatcher != null)
        {
            _combatManager.EventDispatcher.DamageApplied -= OnDamageApplied;
            _combatManager.EventDispatcher.BuffApplied -= OnBuffApplied;
            _combatManager.EventDispatcher.BuffRemoved -= OnBuffRemoved;
        }
    }

    private void OnDamageApplied(DamageEvent damageEvent)
    {
        if (damageEvent == null || damageEvent.Target == null) return;

        string message = $"-{damageEvent.Amount}";
        Color color = Color.red;
        SpawnFloatingText(damageEvent.Target, message, color, null);
    }

    private void OnBuffApplied(BuffAppliedEvent buffEvent)
    {
        if (buffEvent == null || buffEvent.Target == null || buffEvent.BuffDef == null) return;

        Color textColor = buffEvent.BuffDef.isDebuff ? new Color(0.7f, 0.2f, 1f) : Color.cyan;
        string prefix = buffEvent.BuffDef.isDebuff ? "▼" : "▲";
        string message = $"{prefix} {buffEvent.BuffDef.buffName} +{buffEvent.Stacks}";
        SpawnFloatingText(buffEvent.Target, message, textColor, buffEvent.BuffDef.buffIcon);
    }

    private void OnBuffRemoved(BuffRemovedEvent buffEvent)
    {
        if (buffEvent == null || buffEvent.Target == null || buffEvent.BuffDef == null) return;

        string message = $"{buffEvent.BuffDef.buffName} Expired";
        SpawnFloatingText(buffEvent.Target, message, Color.gray, buffEvent.BuffDef.buffIcon);
    }

    private void SpawnFloatingText(MonsterInstance monster, string message, Color color, Sprite icon)
    {
        MonsterWorldVisuals worldVisual = null;
        if (_combatUIController != null)
        {
            _combatUIController.TryGetWorldVisual(monster, out worldVisual);
        }

        Transform spawnParent = GetSpawnParentFor(worldVisual);
        if (spawnParent == null)
        {
            spawnParent = _defaultSpawnParent;
        }

        if (_floatingTextPrefab == null || spawnParent == null) return;

        GameObject spawned = Instantiate(_floatingTextPrefab, spawnParent, false);
        if (spawned.TryGetComponent<FloatingText>(out var floatingText))
        {
            floatingText.Setup(message, color, icon);
        }
        else if (spawned.TryGetComponent<FloatingTextController>(out var floatingTextController))
        {
            floatingTextController.Setup(message, color, icon);
        }
        else
        {
            Debug.LogWarning("[CombatFloatingTextManager] FloatingText prefab does not contain a FloatingText or FloatingTextController component.", spawned);
        }

        if (ShouldUseScreenSpace(spawnParent) && worldVisual != null)
        {
            PositionInScreenSpace(spawned.GetComponent<RectTransform>(), worldVisual.transform.position);
        }
    }

    private Transform GetSpawnParentFor(MonsterWorldVisuals worldVisual)
    {
        if (worldVisual == null)
        {
            return _defaultSpawnParent;
        }

        Canvas worldCanvas = worldVisual.GetComponentInParent<Canvas>();
        if (worldCanvas != null && worldCanvas.renderMode == RenderMode.WorldSpace)
        {
            return worldVisual.HudAnchor;
        }

        return _defaultSpawnParent;
    }

    private bool ShouldUseScreenSpace(Transform parent)
    {
        if (parent == null) return false;
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        return canvas != null && canvas.renderMode != RenderMode.WorldSpace;
    }

    private void PositionInScreenSpace(RectTransform rectTransform, Vector3 worldPosition)
    {
        if (rectTransform == null) return;

        Canvas canvas = _defaultSpawnParent.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPosition);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, _uiCamera, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }
}

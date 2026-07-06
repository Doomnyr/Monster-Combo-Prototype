using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))] // Required to capture mouse hovering in World Space!
public class MonsterWorldVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sizing and Auto-Scaling")]
    [Tooltip("Force the sprite to scale uniformly to fit within this bounding box in Unity World Space")]
    [SerializeField] private Vector2 _targetBoxSize = new Vector2(2f, 2f);
    [SerializeField] private bool _enableAutoScaling = true;
    [SerializeField] private Transform _HUDAnchor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    private MonsterInstance _trackedMonster;

    public Transform HudAnchor => _HUDAnchor;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    public void Setup(MonsterInstance monster)
    {
        _trackedMonster = monster;
        _spriteRenderer.sprite = _trackedMonster.MonsterDef.MonsterSprite; 
        
        // Get the current size of the sprite in units
        Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;

        // Calculate the scale needed to fit the largest dimension into the target size
        float scaleFactor = 2 / Mathf.Max(spriteSize.x, spriteSize.y);

        // Apply the uniform scale
        _spriteRenderer.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

        MonsterCombatVisuals combatVisuals = GetComponent<MonsterCombatVisuals>();
        if (combatVisuals != null)
        {
            combatVisuals.SetupVisuals(monster);
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_trackedMonster != null && CombatTooltipManager.Instance != null)
        {
            //CombatTooltipManager.Instance.ShowTooltip(_trackedMonster);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CombatTooltipManager.Instance != null)
        {
            CombatTooltipManager.Instance.HideTooltip();
        }
    }
}
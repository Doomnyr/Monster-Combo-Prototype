using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))] // Required to capture mouse hovering in World Space!
public class MonsterWorldVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sizing and Auto-Scaling")]
    [Tooltip("Force the sprite to scale uniformly to fit within this bounding box in Unity World Space")]
    [SerializeField] private Vector2 _targetBoxSize = new Vector2(2f, 2f);
    [SerializeField] private bool _enableAutoScaling = true;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    private MonsterInstance _trackedMonster;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    public void Setup(MonsterInstance monster)
    {
        _trackedMonster = monster;

        if (_trackedMonster != null && _trackedMonster.MonsterDef != null)
        {
             _spriteRenderer.sprite = _trackedMonster.MonsterDef.MonsterSprite; 
            
            if (_spriteRenderer.sprite != null)
            {
                if (_enableAutoScaling)
                {
                    // Get raw size of the sprite asset in world units
                    float rawWidth = _spriteRenderer.sprite.bounds.size.x;
                    float rawHeight = _spriteRenderer.sprite.bounds.size.y;

                    if (rawWidth > 0 && rawHeight > 0)
                    {
                        // Calculate scaling factors for both width and height
                        float scaleX = _targetBoxSize.x / rawWidth;
                        float scaleY = _targetBoxSize.y / rawHeight;
                                            
                        // Choose the smaller scale factor to ensure it fits completely within the target box
                        // while maintaining the correct aspect ratio (avoiding squishing or stretching)
                        float uniformScale = Mathf.Min(scaleX, scaleY);
                                            
                        transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
                    }
                }

                 _collider.size = _spriteRenderer.sprite.bounds.size;
            }
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
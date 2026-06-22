using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MonsterInstance _trackedMonster;

    /// <summary>
    /// Call this whenever your UI populates/updates this slot with a monster runtime instance.
    /// </summary>
    public void SetupSlot(MonsterInstance monster)
    {
        _trackedMonster = monster;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (_trackedMonster != null)
        {
            Debug.Log("Mouse OVER!");
            CombatTooltipManager.Instance.ShowTooltip(_trackedMonster);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatTooltipManager.Instance.HideTooltip();
    }

    private void OnDisable()
    {
        // Safety check to prevent stuck tooltips if a slot is disabled while hovered
        if (CombatTooltipManager.Instance != null)
        {
            CombatTooltipManager.Instance.HideTooltip();
        }
    }
}
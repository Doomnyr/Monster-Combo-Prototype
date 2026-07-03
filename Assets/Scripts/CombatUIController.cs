using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Loading;

public class CombatUIController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CombatManager combatManager;

    [Header("Grid Visual Layout Setup")]
    [Tooltip("Place your UI Grid Slot components here, matching your column/row layout.")]
    [SerializeField] private List<GridSlotUI> playerUiSlots;
    [SerializeField] private List<GridSlotUI> enemyUiSlots;

    private void OnEnable()
    {
        combatManager.OnCombatDataReady += HandleCombatDataReady;
        combatManager.OnTurnStarted += HandleHighlightTransition;
    }

    private void OnDisable()
    {
        combatManager.OnCombatDataReady -= HandleCombatDataReady;
        combatManager.OnTurnStarted -= HandleHighlightTransition;
    }

    private void HandleCombatDataReady()
    {
        // Match player data instances to player visual slots
        MatchTeamToUi(combatManager.PlayerTeam, playerUiSlots);
        
        // Match enemy data instances to enemy visual slots
        MatchTeamToUi(combatManager.EnemyTeam, enemyUiSlots);
    }

    private void MatchTeamToUi(List<MonsterInstance> teamData, List<GridSlotUI> uiSlots)
    {
        foreach (var monster in teamData)
        {
            int targetUiIndex = (monster.GridPosition.Column * 3) + monster.GridPosition.Row;

            if (targetUiIndex >= 0 && targetUiIndex < uiSlots.Count)
            {
                // We pass the same monster instance as both the health and mana observable target
                uiSlots[targetUiIndex].Bind(
                    monster.MonsterDef.MonsterName,
                    monster.MonsterDef.MonsterSprite, 
                    monster, // Implicitly cast to IHealthObservable
                    monster,  // Implicitly cast to IManaObservable
                    monster
                );

                uiSlots[targetUiIndex].TryGetComponent<TooltipTrigger>(out var trigger);
                     
                if (uiSlots[targetUiIndex].TryGetComponent<MonsterCombatVisuals>(out var visuals)) 
                {
                    Debug.Log("LoadingStatus visual combattext");
                    visuals.SetupVisuals(monster);
                }
                else
                {
                    Debug.Log("Error finding visuals");
                }

                trigger.SetupSlot(monster);
            }
        }
    }

    private void HandleHighlightTransition(MonsterInstance currentActiveMonster)
    {
        if (currentActiveMonster == null) return;

        Debug.Log("Found target to highlight");
        foreach (GridSlotUI slot in playerUiSlots)
        {
            if (slot == null) continue;
            // If this slot is representing the active monster, light up the borders!
            bool isActiveTurnOwner = (slot.BoundMonster == currentActiveMonster);
            slot.SetTurnHighlight(isActiveTurnOwner);
        }

        foreach (GridSlotUI slot in enemyUiSlots)
        {
            if (slot == null) continue;
            // If this slot is representing the active monster, light up the borders!
            bool isActiveTurnOwner = (slot.BoundMonster == currentActiveMonster);
            slot.SetTurnHighlight(isActiveTurnOwner);
        }
    }

}
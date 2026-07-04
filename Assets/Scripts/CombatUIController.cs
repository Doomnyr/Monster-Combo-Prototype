using UnityEngine;
using System.Collections.Generic;

public class CombatUIController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private Canvas _floatingUiCanvas; // Drag your screen-space overlay Canvas here

    [Header("Prefabs to Spawn")]
    [SerializeField] private GameObject _monsterWorldPrefab; // Prefab carrying MonsterWorldVisuals
    [SerializeField] private GameObject _floatingHudPrefab;  // Prefab carrying GridSlotUI

    [Header("World Space Grid Layout Offsets")]
    [SerializeField] private Vector3 _playerBasePosition = new Vector3(-5f, -1f, 0f);
    [SerializeField] private Vector3 _enemyBasePosition = new Vector3(5f, -1f, 0f);
    [SerializeField] private float _rowSpacing = 1.5f;
    [SerializeField] private float _colSpacing = 2.0f;

    // Trackers mapping runtime monster data directly to their spawned world and UI objects
    private readonly Dictionary<MonsterInstance, MonsterWorldVisuals> _worldVisualsMap = new Dictionary<MonsterInstance, MonsterWorldVisuals>();
    private readonly Dictionary<MonsterInstance, GridSlotUI> _monsterToSlotMap = new Dictionary<MonsterInstance, GridSlotUI>();
    
    // Caches the currently highlighted slot so we can unhighlight it instantly without search loops
    private GridSlotUI _currentlyActiveSlot;

    private void OnEnable()
    {
        if (_combatManager != null)
        {
            _combatManager.OnTurnStarted += HandleHighlightTransition;
        }
    }

    private void OnDisable()
    {
        if (_combatManager != null)
        {
            _combatManager.OnTurnStarted -= HandleHighlightTransition;
        }
    }

    /// <summary>
    /// Spawns the physical world monster representations and instantiates/binds their floating UI trackers.
    /// Call this from your Setup/Initialization phase!
    /// </summary>
    public void PopulateBattlefield(List<MonsterInstance> playerTeam, List<MonsterInstance> enemyTeam)
    {
        ClearExistingBattlefield();

        // 1. Spawn Player Team
        SpawnTeamVisuals(playerTeam, _playerBasePosition, isPlayerTeam: true);

        // 2. Spawn Enemy Team
        SpawnTeamVisuals(enemyTeam, _enemyBasePosition, isPlayerTeam: false);

        Debug.Log($"[CombatUIController] Dynamic spawning complete. Spawner paired {_monsterToSlotMap.Count} active units to their screen trackers.", this);
    }

    private void SpawnTeamVisuals(List<MonsterInstance> team, Vector3 basePos, bool isPlayerTeam)
    {
        for (int i = 0; i < team.Count; i++)
        {
            MonsterInstance monster = team[i];
            if (monster == null) continue;

            //float finalColSpacing = _colSpacing <= 0 ? 2.0f : _colSpacing;
            //float finalRowSpacing = _rowSpacing <= 0 ? 1.5f : _rowSpacing;

            // Simple row/col placement calculation from the team list index (e.g. 3x2 grid)
            int row = i / 2;
            int col = i % 2;
            
            // Adjust offsets so backlines sit further away
            float xOffset = isPlayerTeam ? -col * _colSpacing : col * _colSpacing;
            float yOffset = -row * _rowSpacing;
            Vector3 worldSpawnPosition = basePos + new Vector3(xOffset, yOffset, 0f);

            // A. Spawn the physical world monster
            GameObject worldGo = Instantiate(_monsterWorldPrefab, worldSpawnPosition, Quaternion.identity);
            worldGo.name = $"World_{monster.MonsterDef.MonsterName}_{monster.InstanceId.Substring(0, 4)}";
            
            MonsterWorldVisuals worldVisuals = worldGo.GetComponent<MonsterWorldVisuals>();
            worldVisuals.Setup(monster);
            _worldVisualsMap[monster] = worldVisuals;

            // B. Spawn the floating health bar (as a child of the Canvas)
            if (_floatingUiCanvas != null && _floatingHudPrefab != null)
            {
                GameObject hudGo = Instantiate(_floatingHudPrefab, _floatingUiCanvas.transform);
                hudGo.name = $"HUD_{monster.MonsterDef.MonsterName}";

                GridSlotUI hudSlot = hudGo.GetComponent<GridSlotUI>();
                
                // Bind everything together! The HUD now tracks the world transform we just created.
                hudSlot.Bind(
                    monster.MonsterDef.MonsterName, 
                    worldGo.transform, 
                    monster, 
                    monster, 
                    monster
                );

                _monsterToSlotMap[monster] = hudSlot;
                
                // C. Attach the floating number visuals to trigger damage/heal popups in world space
                if (worldGo.TryGetComponent<MonsterCombatVisuals>(out var damageTextController))
                {
                    damageTextController.SetupVisuals(monster);
                }
            }
        }
    }

    private void HandleHighlightTransition(MonsterInstance currentActiveMonster)
    {
        // 1. Turn off the old highlight immediately using our cached active reference
        if (_currentlyActiveSlot != null)
        {
            _currentlyActiveSlot.SetTurnHighlight(false);
            _currentlyActiveSlot = null;
        }

        if (currentActiveMonster == null) return;

        // 2. Perform a fast O(1) dictionary lookup to locate and highlight the new active slot
        if (_monsterToSlotMap.TryGetValue(currentActiveMonster, out GridSlotUI targetSlot))
        {
            targetSlot.SetTurnHighlight(true);
            _currentlyActiveSlot = targetSlot; // Cache it for next turn's cleanup!
        }
    }

    private void ClearExistingBattlefield()
    {
        // Clean up visual GameObjects
        foreach (var slot in _monsterToSlotMap.Values)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        foreach (var visuals in _worldVisualsMap.Values)
        {
            if (visuals != null) Destroy(visuals.gameObject);
        }

        _monsterToSlotMap.Clear();
        _worldVisualsMap.Clear();
        _currentlyActiveSlot = null;
    }

    private void OnDestroy() => ClearExistingBattlefield();
}

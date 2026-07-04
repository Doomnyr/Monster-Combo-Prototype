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

        [Header("Hierarchy Containers")]
    [Tooltip("Optional: Assign separate folder-like transforms to organize spawned monsters. If left empty, these will be generated dynamically.")]
    [SerializeField] private Transform _playerTeamParent;
    [SerializeField] private Transform _enemyTeamParent;

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

    private void SpawnTeamVisuals2(List<MonsterInstance> team, Vector3 basePos, bool isPlayerTeam)
    {
        // 1. Resolve or create clean parent folders in your Hierarchy to avoid scene clutter
        Transform targetParent = isPlayerTeam ? _playerTeamParent : _enemyTeamParent;
        if (targetParent == null)
        {
            GameObject container = new GameObject(isPlayerTeam ? "[Player Team]" : "[Enemy Team]");
            container.transform.SetParent(this.transform); // Nest them cleanly under this UI Controller transform
            targetParent = container.transform;
            
            // Cache the reference so we don't recreate it on subsequent calls
            if (isPlayerTeam) _playerTeamParent = targetParent;
            else _enemyTeamParent = targetParent;
        }

        // 2. Clear out any legacy spawned objects under these parents first to prevent visual stacking
        foreach (Transform child in targetParent)
        {
            Destroy(child.gameObject);
        }

        // 3. Spawn each monster with explicit, rich debug names
        for (int i = 0; i < team.Count; i++)
        {
            MonsterInstance monster = team[i];
            if (monster == null) continue;

            // Determine local offsets based on the monster's GridPosition
            int row = monster.GridPosition.Row;
            int col = monster.GridPosition.Column;

            float finalColSpacing = _colSpacing <= 0 ? 2.0f : _colSpacing;
            float finalRowSpacing = _rowSpacing <= 0 ? 1.5f : _rowSpacing;

            float xOffset = isPlayerTeam ? -col * finalColSpacing : col * finalColSpacing;
            float yOffset = -row * finalRowSpacing;
            Vector3 spawnPosition = basePos + new Vector3(xOffset, yOffset, 0f);

            // Instantiate the prefab directly under the parent container
             GameObject spawnedGo = Instantiate(_monsterWorldPrefab, spawnPosition, Quaternion.identity, targetParent);

            // 2. Safely grab the MonsterWorldVisuals component attached to that prefab
            MonsterWorldVisuals worldVisual = spawnedGo.GetComponent<MonsterWorldVisuals>();
            
            // Give it a beautifully descriptive name for runtime debugging
            string teamPrefix = isPlayerTeam ? "Ally" : "Enemy";
            string monsterName = monster.MonsterDef != null ? monster.MonsterDef.MonsterName : "Unknown";
            worldVisual.gameObject.name = $"[{teamPrefix}] [R:{row}, C:{col}] - {monsterName}";

            // Setup the visuals (autoscale, renderer, collider, etc.)
            worldVisual.Setup(monster);

            // Track this spawned world instance
            _worldVisualsMap[monster] = worldVisual;
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

    #if UNITY_EDITOR
    /// <summary>
    /// Draws interactive visual guides directly in the Unity Scene View 
    /// so you can adjust your layout coordinates in real-time without playing the game!
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw Player Grid slots (Cyan/Blue guide)
        DrawTeamGizmos(_playerBasePosition, true, new Color(0.1f, 0.6f, 1f, 0.8f));

        // Draw Enemy Grid slots (Red/Orange guide)
        DrawTeamGizmos(_enemyBasePosition, false, new Color(1f, 0.35f, 0.1f, 0.8f));
    }

    private void DrawTeamGizmos(Vector3 basePos, bool isPlayerTeam, Color color)
    {
        Gizmos.color = color;
        
        // 1. Draw a solid sphere at the Base Anchor Point
        Gizmos.DrawSphere(basePos, 0.25f);
        
        float finalColSpacing = _colSpacing <= 0 ? 2.0f : _colSpacing;
        float finalRowSpacing = _rowSpacing <= 0 ? 1.5f : _rowSpacing;

        // 2. Visualize a 3x2 grid layout (6 mock slot bounding boxes)
        for (int i = 0; i < 6; i++)
        {
            int row = i / 2;
            int col = i % 2;

            float xOffset = isPlayerTeam ? -col * finalColSpacing : col * finalColSpacing;
            float yOffset = -row * finalRowSpacing;
            Vector3 mockPosition = basePos + new Vector3(xOffset, yOffset, 0f);

            // Draw a 2x2 wireframe box representing the target bounding boundaries of your monsters
            Gizmos.DrawWireCube(mockPosition, new Vector3(2f, 2f, 0.1f));

            // Draw a small crosshair at the center spawn coordinate
            Gizmos.DrawLine(mockPosition - Vector3.left * 0.15f, mockPosition + Vector3.left * 0.15f);
            Gizmos.DrawLine(mockPosition - Vector3.up * 0.15f, mockPosition + Vector3.up * 0.15f);

            // 3. Render dynamic text tags in the editor viewport using Unity's Handles API
            string labelText = $"{(isPlayerTeam ? "Ally" : "Enemy")} Slot {i + 1}\n[Row {row}, Col {col}]";
            
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = color;
            labelStyle.fontSize = 10;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.UpperCenter;

            // Draw the slot designation label slightly above each slot box
            UnityEditor.Handles.Label(mockPosition + Vector3.up * 1.3f, labelText, labelStyle);
        }
    }
    #endif
}

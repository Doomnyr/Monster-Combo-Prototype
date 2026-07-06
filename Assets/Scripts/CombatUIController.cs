using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIController : MonoBehaviour
{
    [Header("Grid Dependency")]
    [Tooltip("Drag your scene's GridManager component here.")]
    [SerializeField] private GridManager _gridManager;

    [Header("Prefabs")]
    [Tooltip("Drag your World-Space physical Monster Visuals prefab here (must have MonsterWorldVisuals attached).")]
    [SerializeField] private GameObject _monsterWorldPrefab;

    [Header("Hierarchy Containers")]
    [Tooltip("Optional: Assign separate folder-like transforms to organize spawned monsters.")]
    [SerializeField] private Transform _playerTeamParent;
    [SerializeField] private Transform _enemyTeamParent;

    [Header("Floating HUD Elements")]
    [Tooltip("Drag your Screen-Space floating GridSlotUI prefab here.")]
    [SerializeField] private GameObject _healthBarPrefab;
    [Tooltip("Drag the parent Canvas (or panel inside it) where floating health bars should render.")]
    [SerializeField] private Transform _hudParent;

    private readonly Dictionary<MonsterInstance, MonsterWorldVisuals> _worldVisualsMap = new Dictionary<MonsterInstance, MonsterWorldVisuals>();

    public void PopulateBattlefield(List<MonsterInstance> playerTeam, List<MonsterInstance> enemyTeam)
    {
        _worldVisualsMap.Clear();

        if (_hudParent != null)
        {
            foreach (Transform child in _hudParent)
            {
                if (child != null) Destroy(child.gameObject);
            }
        }

        SpawnTeamVisuals(playerTeam, true);
        SpawnTeamVisuals(enemyTeam, false);
    }

    private void SpawnTeamVisuals(List<MonsterInstance> team, bool isPlayerTeam)
    {
        if (team == null) return;

        // 1. Resolve GridManager reference
        if (_gridManager == null)
        {
            _gridManager = FindFirstObjectByType<GridManager>();
            if (_gridManager == null)
            {
                Debug.LogError("<color=red><b>[CRITICAL]</b></color> GridManager is missing from the Scene! Please create a GridManager GameObject.", this);
                return;
            }
        }

        // 2. Resolve or create parenting structures
        Transform targetParent = isPlayerTeam ? _playerTeamParent : _enemyTeamParent;
        if (targetParent == null)
        {
            GameObject container = new GameObject(isPlayerTeam ? "[Player Team]" : "[Enemy Team]");
            container.transform.SetParent(this.transform);
            targetParent = container.transform;

            if (isPlayerTeam) _playerTeamParent = targetParent;
            else _enemyTeamParent = targetParent;
        }

        foreach (Transform child in targetParent)
        {
            if (child != null) Destroy(child.gameObject);
        }

        if (_monsterWorldPrefab == null)
        {
            Debug.LogError("[CombatUIController] World Monster Prefab is unassigned!", this);
            return;
        }

        // 3. Spawn loop
        for (int i = 0; i < team.Count; i++)
        {
            MonsterInstance monster = team[i];
            if (monster == null) continue;

            try
            {
                // QUERY THE GRID MANAGER FOR THE SLOT COORDINATE
                Vector3 spawnPosition = _gridManager.GetSlotWorldPosition(
                    isPlayerTeam, 
                    monster.GridPosition.Row, 
                    monster.GridPosition.Column
                );

                GameObject spawnedGo = Instantiate(_monsterWorldPrefab, spawnPosition, Quaternion.identity, targetParent);
                MonsterWorldVisuals worldVisual = spawnedGo.GetComponent<MonsterWorldVisuals>();

                if (worldVisual == null)
                {
                    Debug.LogError($"[CRITICAL] Prefab '{_monsterWorldPrefab.name}' is missing 'MonsterWorldVisuals'!", spawnedGo);
                    Destroy(spawnedGo);
                    continue; 
                }

                string teamPrefix = isPlayerTeam ? "Ally" : "Enemy";
                string monsterName = (monster.MonsterDef != null) ? monster.MonsterDef.MonsterName : "Unknown";
                worldVisual.gameObject.name = $"[{teamPrefix}] [R:{monster.GridPosition.Row}, C:{monster.GridPosition.Column}] - {monsterName}";

                // Setup visuals and scaling
                worldVisual.Setup(monster);
                _worldVisualsMap[monster] = worldVisual;

                GridSlotUI gridSlot = spawnedGo.GetComponentInChildren<GridSlotUI>();
                
                if (_healthBarPrefab != null && _hudParent != null)
                {
                                    
                    if (gridSlot != null)
                    {
                        // Bind HUD to the monster's HUDAnchor transform
                        gridSlot.Bind(
                            monsterName, 
                            worldVisual.HudAnchor, 
                            monster, 
                            monster, 
                            monster
                        );
                    }
                    else
                    {
                        Debug.LogError($"[CombatUIController] HUD Prefab is missing 'GridSlotUI'!", _healthBarPrefab);
                        Destroy(this);
                    }
                } 

                
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CombatUIController] Spawning failed at index {i}: {ex.Message}", this);
            }
        }
    }
}
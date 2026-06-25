using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The "Traffic Controller" from our Architecture Plan.
/// Responsible strictly for a continuous rolling queue of combatants.
/// </summary>
public class TurnManager
{
    private Queue<MonsterInstance> _turnQueue = new Queue<MonsterInstance>();

    /// <summary>
    /// Feed the manager the active teams at the start of combat.
    /// Sets up the initial turn order based on Speed.
    /// </summary>
    public void Initialize(List<MonsterInstance> playerTeam, List<MonsterInstance> enemyTeam)
    {
        _turnQueue.Clear();

        List<MonsterInstance> allCombatants = new List<MonsterInstance>();
        allCombatants.AddRange(playerTeam);
        allCombatants.AddRange(enemyTeam);

        // Sort all living monsters by Speed for the very first setup (highest goes first!)
        var sortedCombatants = allCombatants
            .Where(m => m.IsAlive)
            .OrderByDescending(m => m.Speed)
            .ToList();

        foreach (var monster in sortedCombatants)
        {
            _turnQueue.Enqueue(monster);
        }

        Debug.Log($"[TurnManager] Continuous rolling queue initialized with {_turnQueue.Count} combatants.");
    }

    /// <summary>
    /// Pops the next valid monster from the queue.
    /// </summary>
    public MonsterInstance GetNextTurn()
    {
        // Clear out any monsters that died while waiting in line
        while (_turnQueue.Count > 0 && _turnQueue.Peek().IsDefeated)
        {
            _turnQueue.Dequeue();
        }

        // If the queue is completely empty, combat is mathematically over (everyone is dead)
        if (_turnQueue.Count == 0) return null;

        return _turnQueue.Dequeue();
    }

    /// <summary>
    /// Places a monster at the back of the queue. Call this after their turn ends!
    /// </summary>
    public void RequeueCombatant(MonsterInstance monster)
    {
        if (monster != null && monster.IsAlive)
        {
            _turnQueue.Enqueue(monster);
        }
    }

    /// <summary>
    /// Allows the UI to peek at who is coming up next in the continuous queue.
    /// </summary>
    public List<MonsterInstance> GetUpcomingTurns()
    {
        return _turnQueue.Where(m => m.IsAlive).ToList();
    }
}
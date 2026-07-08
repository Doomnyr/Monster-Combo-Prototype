using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public List<MonsterInstance> PlayerTeam { get; private set; } = new List<MonsterInstance>();
    public List<MonsterInstance> EnemyTeam { get; private set; } = new List<MonsterInstance>();

    private readonly TurnManager _turnManager = new TurnManager();
    private readonly CombatEventDispatcher _eventDispatcher = new CombatEventDispatcher();
    private bool _isTurnRunning = false;
    public event Action OnCombatDataReady;
    public event Action<MonsterInstance> OnTurnStarted;

    public CombatEventDispatcher EventDispatcher => _eventDispatcher;

    public void PrepareMatch(List<MonsterInstance> readyPlayerTeam, List<MonsterInstance> readyEnemyTeam)
    {
        PlayerTeam = readyPlayerTeam;
        EnemyTeam = readyEnemyTeam;

        InitializeMonsterEventSubscriptions(PlayerTeam);
        InitializeMonsterEventSubscriptions(EnemyTeam);

        Debug.Log("CombatManager: Match started!");
        
        _eventDispatcher.PublishCombatStarted(PlayerTeam, EnemyTeam);
        OrderMonsterInTurnQueue(PlayerTeam, EnemyTeam);
        OnCombatDataReady?.Invoke();
    }

    private void InitializeMonsterEventSubscriptions(List<MonsterInstance> monsters)
    {
        foreach (var monster in monsters)
        {
            monster.RegisterCombatEvents(_eventDispatcher);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceToNextTurn();
        }
    }

    private void OrderMonsterInTurnQueue(List<MonsterInstance> playerTeam, List<MonsterInstance> enemyTeam)
    {
        _turnManager.Initialize(playerTeam, enemyTeam);
    }

    public void AdvanceToNextTurn()
    {
        if (!_isTurnRunning)
        {
            StartCoroutine(ExecuteNextTurn());
        }
    }

    private IEnumerator ExecuteNextTurn()
    {
        _isTurnRunning = true;
        MonsterInstance activeMonster = _turnManager.GetNextTurn();

        if (activeMonster == null || !activeMonster.IsAlive)
        {
            _isTurnRunning = false;
            yield break;
        }

        OnTurnStarted?.Invoke(activeMonster);
        var battlefield = BuildBattlefield();
        _eventDispatcher.PublishTurnStarted(activeMonster, battlefield);
        yield return new WaitForSeconds(GameManager.TURN_DELAY);

        if (activeMonster.MonsterDef.CommandPriorityList.Count > 0)
        {
            ExecuteSkill(activeMonster.MonsterDef.CommandPriorityList[0], activeMonster, battlefield);
        }

        _eventDispatcher.PublishTurnEnded(activeMonster, battlefield);
        _turnManager.RequeueCombatant(activeMonster);

        _isTurnRunning = false;
    }

    public void ExecuteSkill(SkillDefinitionSO skill, MonsterInstance caster, List<MonsterInstance> battlefield)
    {
        List<MonsterInstance> lastSuccessfulTargets = new List<MonsterInstance>();

        foreach (SkillAction action in skill.Actions)
        {
            List<MonsterInstance> currentActionTargets = action.targetFinder.FindTargets(
                action, caster, battlefield, lastSuccessfulTargets
            );

            foreach (MonsterInstance target in currentActionTargets)
            {
                foreach (SkillEffectSO effect in action.executionEffect)
                {
                    if (target != null && target.IsAlive)
                    {
                        effect.Apply(action, caster, target);
                    }
                }
            }

            if (currentActionTargets.Count > 0)
            {
                lastSuccessfulTargets = currentActionTargets;
            }
        }

        if (lastSuccessfulTargets.Count > 0)
        {
            _eventDispatcher.PublishSkillExecuted(skill, caster, lastSuccessfulTargets);
        }
    }

    private List<MonsterInstance> BuildBattlefield()
    {
        List<MonsterInstance> battlefield = new List<MonsterInstance>();
        battlefield.AddRange(PlayerTeam);
        battlefield.AddRange(EnemyTeam);
        return battlefield;
    }
}
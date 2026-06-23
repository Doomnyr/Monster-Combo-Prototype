using UnityEngine;

public class MonsterCombatVisuals : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Transform textSpawnPoint; // Where text pops up (usually slightly above the slot)

    [Header("Color Schemes")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color buffColor = Color.cyan;
    [SerializeField] private Color debuffColor = new Color(0.7f, 0.2f, 1f); // Purple

    private MonsterInstance _trackedMonster;

    public void SetupVisuals(MonsterInstance monster)
    {
        // Unsubscribe from previous monster events to prevent memory leaks if slots are reused
        Cleanup();

        _trackedMonster = monster;

        if (_trackedMonster != null)
        {
            // Subscribe to the new events we made!
            _trackedMonster.OnDamageTaken += SpawnDamageText;
            _trackedMonster.OnHealed += SpawnHealText;
            _trackedMonster.OnBuffApplied += SpawnBuffAppliedText;
            _trackedMonster.OnBuffRemoved += SpawnBuffRemovedText;
        }
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (_trackedMonster != null)
        {
            _trackedMonster.OnDamageTaken -= SpawnDamageText;
            _trackedMonster.OnHealed -= SpawnHealText;
            _trackedMonster.OnBuffApplied -= SpawnBuffAppliedText;
            _trackedMonster.OnBuffRemoved -= SpawnBuffRemovedText;
        }
    }

    private void SpawnDamageText(int amount)
    {
        CreateFloatingText($"-{amount}", damageColor);
    }

    private void SpawnHealText(int amount)
    {
        CreateFloatingText($"+{amount}", healColor);
    }

    private void SpawnBuffAppliedText(BuffDefinitionSO buff, int stacks)
    {
        Color textCol = buff.isDebuff ? debuffColor : buffColor;
        string prefix = buff.isDebuff ? "▼" : "▲";
        CreateFloatingText($"{prefix} {buff.buffName} +{stacks}", textCol);
    }

    private void SpawnBuffRemovedText(BuffDefinitionSO buff)
    {
        CreateFloatingText($"{buff.buffName} Expired", Color.gray);
    }

    private void CreateFloatingText(string message, Color color)
    {
        if (floatingTextPrefab == null) return;

        // Spawn the text inside the slot's Canvas space
        Transform parentTransform = textSpawnPoint != null ? textSpawnPoint : transform;
        GameObject textInstance = Instantiate(floatingTextPrefab, parentTransform.position, Quaternion.identity, transform.root);

        if (textInstance.TryGetComponent<FloatingText>(out var flText))
        {
            flText.Setup(message, color);
        }
    }
}
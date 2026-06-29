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
            _trackedMonster.Buffs.OnBuffApplied += SpawnBuffAppliedText;
            _trackedMonster.Buffs.OnBuffRemoved += SpawnBuffRemovedText;
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
            _trackedMonster.Buffs.OnBuffApplied -= SpawnBuffAppliedText;
            _trackedMonster.Buffs.OnBuffRemoved -= SpawnBuffRemovedText;
        }
    }

    private void SpawnDamageText(int amount)
    {
        CreateFloatingText($"-{amount}", damageColor, null);
    }

    private void SpawnHealText(int amount)
    {
        CreateFloatingText($"+{amount}", healColor, null);
    }

    private void SpawnBuffAppliedText(BuffDefinitionSO buff, int stacks)
    {    
        Color textCol = buff.IsDebuff ? debuffColor : buffColor;
        string prefix = buff.IsDebuff ? "▼" : "▲";
        CreateFloatingText($"{prefix} {buff.name} +{stacks}", textCol, buff.Icon);
    }

    private void SpawnBuffRemovedText(BuffDefinitionSO buff)
    {
        CreateFloatingText($"{buff.name} Expired", Color.gray, buff.Icon);
    }

    private void CreateFloatingText(string message, Color color, Sprite buffIcon)
    {
        if (floatingTextPrefab == null) return;

        // Spawn the text inside the slot's Canvas space
        Transform parentTransform = textSpawnPoint != null ? textSpawnPoint : transform;
        GameObject textInstance = Instantiate(floatingTextPrefab, parentTransform.position, Quaternion.identity, transform.root);

        if (textInstance.TryGetComponent<FloatingText>(out var flText))
        {
            flText.Setup(message, color, buffIcon);
        }
    }
}
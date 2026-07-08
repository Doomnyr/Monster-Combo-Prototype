using UnityEngine;

public class MonsterCombatVisuals : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Drag your scale-safe UI FloatingText prefab here.")]
    [SerializeField] private GameObject floatingTextPrefab;
    
    [Tooltip("Drag the child 'FloatingCombatText' GameObject from under your nested canvas here.")]
    [SerializeField] private Transform textSpawnPoint;

    [Header("Color Schemes")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color buffColor = Color.cyan;
    [SerializeField] private Color debuffColor = new Color(0.7f, 0.2f, 1f);

    private MonsterInstance _trackedMonster;

    public void SetupVisuals(MonsterInstance monster)
    {
        Cleanup();
        _trackedMonster = monster;

        if (_trackedMonster != null)
        {
            _trackedMonster.OnDamageTaken += SpawnDamageText;
            _trackedMonster.OnHealed += SpawnHealText;
            _trackedMonster.OnBuffApplied += SpawnBuffAppliedText;
            _trackedMonster.OnBuffRemoved += SpawnBuffRemovedText;
        }
    }

    private void OnDestroy() => Cleanup();

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

    private void SpawnDamageText(int amount) => CreateFloatingText($"-{amount}", damageColor, null);
    private void SpawnHealText(int amount) => CreateFloatingText($"+{amount}", healColor, null);

    private void SpawnBuffAppliedText(BuffDefinitionSO buff, int stacks)
    {    
        Color textCol = buff.isDebuff ? debuffColor : buffColor;
        string prefix = buff.isDebuff ? "▼" : "▲";
        CreateFloatingText($"{prefix} {buff.buffName} +{stacks}", textCol, buff.buffIcon);
    }

    private void SpawnBuffRemovedText(BuffDefinitionSO buff)
    {
        CreateFloatingText($"{buff.buffName} Expired", Color.gray, buff.buffIcon);
    }

    private void CreateFloatingText(string message, Color color, Sprite buffIcon)
    {
        if (floatingTextPrefab == null) return;
        
        // Fallback: use current transform if textSpawnPoint was not specified
        Transform parentTransform = textSpawnPoint != null ? textSpawnPoint : transform;

        // CRITICAL FIX: Pass 'false' as the third parameter so the text perfectly
        // inherits the local Canvas layout bounds and stays proportional!
        GameObject textObj = Instantiate(floatingTextPrefab, parentTransform, false);
        
        if (textObj.TryGetComponent<FloatingText>(out var fText))
        {
            fText.Setup(message, color, buffIcon);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRegistry", menuName = "Data/Game Registry")]
public class GameRegistry : ScriptableObject, ISerializationCallbackReceiver
{
    [System.Serializable]
    public struct AssetEntry { public string id; public ScriptableObject dataAsset; }
    [System.Serializable]
    public struct IntEntry { public string id; public int number; }
    [System.Serializable]
    public struct FloatEntry { public string id; public float decimalValue; }

    [Header("Inspector Lists")]
    [SerializeField] private List<AssetEntry> inspectorAssets;
    [SerializeField] private List<IntEntry> inspectorInts;
    [SerializeField] private List<FloatEntry> inspectorFloats;

    public Dictionary<string, ScriptableObject> AssetDict { get; private set; } = new Dictionary<string, ScriptableObject>();
    public Dictionary<string, int> IntDict { get; private set; } = new Dictionary<string, int>();
    public Dictionary<string, float> FloatDict { get; private set; } = new Dictionary<string, float>();

    public void OnAfterDeserialize()
    {
        // 1. Build Asset Dictionary
        AssetDict.Clear();
        foreach (var entry in inspectorAssets)
            if (!string.IsNullOrEmpty(entry.id) && !AssetDict.ContainsKey(entry.id)) AssetDict.Add(entry.id, entry.dataAsset);

        // 2. Build Int Dictionary
        IntDict.Clear();
        foreach (var entry in inspectorInts)
            if (!string.IsNullOrEmpty(entry.id) && !IntDict.ContainsKey(entry.id)) IntDict.Add(entry.id, entry.number);

        // 3. Build Float Dictionary
        FloatDict.Clear();
        foreach (var entry in inspectorFloats)
            if (!string.IsNullOrEmpty(entry.id) && !FloatDict.ContainsKey(entry.id)) FloatDict.Add(entry.id, entry.decimalValue);
    }

    public void OnBeforeSerialize() { }
}
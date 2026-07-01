using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public static class MonsterDataImporter
{
    [MenuItem("Tools/Import Game Data/Monsters")]
    public static void ImportMonsters()
    {
        string jsonPath = Path.Combine(Application.dataPath, "Data/monsters.json");
        string exportPath = "Assets/GameAssets/Monsters/";

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"Cannot find JSON file at: {jsonPath}");
            return;
        }

    string jsonContent = File.ReadAllText(jsonPath);
    Debug.Log($"File content length: {jsonContent.Length}"); // 2. Is it empty?

        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        MonsterDatabase db = JsonConvert.DeserializeObject<MonsterDatabase>(jsonContent);

if (db == null) {
        Debug.LogError("JSON Deserialization failed! db is null.");
        return;
    }
    
    if (db.monsters == null) {
        Debug.LogError("db.monsters array is null! Check your JSON keys.");
        return;
    }

    Debug.Log($"Found {db.monsters.Length} monsters to import."); // 4. Does this trigger?

        foreach (var record in db.monsters)
        {
            if (string.IsNullOrEmpty(record.monsterID)) continue;

            string assetPath = $"{exportPath}{record.monsterID}.asset";
            Debug.Log($"Checking for asset at: {assetPath}"); // <--- ADD THIS
            
            // 1. Try to load existing
            MonsterDefinitionSO asset = AssetDatabase.LoadAssetAtPath<MonsterDefinitionSO>(assetPath);

            // 2. If it doesn't exist, CREATE IT
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<MonsterDefinitionSO>();
            AssetDatabase.CreateAsset(asset, assetPath);
            
            // ADD THIS:
            if (System.IO.File.Exists(assetPath)) {
                Debug.Log($"SUCCESS: File confirmed on disk at {assetPath}");
            } else {
                Debug.LogError($"CRITICAL: File was NOT created on disk at {assetPath}");
            }
}

            // 3. Apply the data (this works whether it's new or old)
            ApplyDataToAsset(asset, record);
            
            // 4. Save changes
            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Successfully imported {db.monsters.Length} monsters!");
}

    private static void ApplyDataToAsset(MonsterDefinitionSO asset, MonsterRecord record)
    {
        string spritePath = $"Assets/Art/Monsters/{record.monsterSprite}.png";
        Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        asset.Editor_ImportData(
            record.monsterID,
            record.monsterName,
            loadedSprite,
            record.race,
            record.element,
            record.baseStats,
            null,
            null
        );
    }
}

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
        string exportPath = "Assets/Prefabs/Monsters/";

        if (!File.Exists(jsonPath)) { Debug.LogError("JSON not found!"); return; }
        if (!Directory.Exists(exportPath)) { Directory.CreateDirectory(exportPath); }

        string jsonContent = File.ReadAllText(jsonPath);
        MonsterDatabase db = JsonConvert.DeserializeObject<MonsterDatabase>(jsonContent);

        foreach (var record in db.monsters)
        {
            // IMPORTANT: Ensure this matches the key "id" in your JSON
            if (string.IsNullOrEmpty(record.monsterID)) { Debug.LogWarning("Skipping monster: ID is null/empty"); continue; }

            string assetPath = $"{exportPath}{record.monsterID}.asset";
            MonsterDefinitionSO asset = AssetDatabase.LoadAssetAtPath<MonsterDefinitionSO>(assetPath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<MonsterDefinitionSO>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            ApplyDataToAsset(asset, record);
            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Import Complete.");
    }

    private static void ApplyDataToAsset(MonsterDefinitionSO asset, MonsterRecord record)
    {
        string spritePath = $"Assets/Art/Monsters/{record.monsterSprite}.png";
        Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        List<SkillDefinitionSO> loadedSkills = LoadSkills(record.commandList);

        if (loadedSprite == null)
        {
            Debug.LogWarning($"Importer could not find a Sprite at {spritePath}. Check if the file is marked as 'Sprite (2D and UI)' in Import Settings.");
        }

        asset.Editor_ImportData(record.monsterID, 
                                record.monsterName, 
                                loadedSprite, 
                                record.race, 
                                record.element, 
                                record.baseStats, 
                                null, 
                                loadedSkills);
        Debug.Log($"Successfully applied data to: {record.monsterID}");
    }

    private static List<SkillDefinitionSO> LoadSkills(string[] skillNames)
    {
        List<SkillDefinitionSO> skills = new List<SkillDefinitionSO>();
        
        if (skillNames == null) return skills;

        foreach (string skillName in skillNames)
        {
            // ASSUMPTION: Your SkillDefinitionSO assets are located in Assets/GameAssets/Skills/
            string path = $"Assets/Prefabs/SkillData/{skillName}.asset";
            SkillDefinitionSO skill = AssetDatabase.LoadAssetAtPath<SkillDefinitionSO>(path);
            
            if (skill != null)
            {
                skills.Add(skill);
            }
            else
            {
                Debug.LogWarning($"Could not find skill asset at: {path}");
            }
        }
        return skills;
    }
}

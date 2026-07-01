using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class MonsterDataImporter
{
    [MenuItem("Tools/Import Game Data/Monsters")]
    public static void ImportMonsters()
    {
        // 1. Build lookup dictionaries for assets
        var skillDict = BuildAssetDictionary<SkillDefinitionSO>();
        // var traitDict = BuildAssetDictionary<TraitDefinitionSO>(); // Uncomment if needed

        string jsonPath = Path.Combine(Application.dataPath, "Data/monsters.json");
        string jsonContent = File.ReadAllText(jsonPath);

        var root = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
        var monsterList = root["monsters"];

        foreach (var entry in monsterList)
        {
            // Extract raw data
            string id = entry["monsterId"]?.ToString();
            string name = entry["monsterName"]?.ToString();
            string spriteName = entry["monsterSprite"]?.ToString();

            if (string.IsNullOrEmpty(id)) 
            {
                Debug.LogWarning("Found a monster entry in JSON with a missing or null 'id'. Skipping.");
                continue; 
            }
            
            // Handle Enums (Make sure these match your C# Enum names exactly)
            MonsterRace race = (MonsterRace)System.Enum.Parse(typeof(MonsterRace), entry["race"]?.ToString() ?? "Unknown");
            ElementType element = (ElementType)System.Enum.Parse(typeof(ElementType), entry["element"]?.ToString() ?? "None");

            // Handle Stats
            StatBlock stats = entry["baseStats"]?.ToObject<StatBlock>() ?? new StatBlock();

            // Handle Skill Mapping
            List<SkillDefinitionSO> skills = new List<SkillDefinitionSO>();
            var skillNames = entry["commandList"]?.ToObject<string[]>();
            if (skillNames != null)
            {
                foreach (string sName in skillNames)
                {
                    if (skillDict.TryGetValue(sName, out var skill)) skills.Add(skill);
                    else Debug.LogWarning($"Skill '{sName}' not found for {id}");
                }
            }

            // Load/Create Asset
            string assetPath = $"Assets/SO/Monsters/{id}.asset";
            MonsterDefinitionSO asset = AssetDatabase.LoadAssetAtPath<MonsterDefinitionSO>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<MonsterDefinitionSO>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            // Load Sprite (Dynamic path lookup)
            string spritePath = $"Assets/Art/Monsters/{spriteName}.png";
            Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            // Populate the asset
            asset.Editor_ImportData(id, 
                                    name, 
                                    loadedSprite, 
                                    race, 
                                    element, 
                                    stats, 
                                    null, // traits
                                    skills);
            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Monster Import Complete!");
    }

    private static Dictionary<string, T> BuildAssetDictionary<T>() where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        var dict = new Dictionary<string, T>();
        foreach (string guid in guids)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
            if (asset != null && !dict.ContainsKey(asset.name)) dict.Add(asset.name, asset);
        }
        return dict;
    }
}
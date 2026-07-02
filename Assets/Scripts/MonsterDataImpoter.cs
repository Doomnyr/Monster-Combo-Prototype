using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class MonsterCSVImporter
{
    
    [MenuItem("Tools/Import Game Data/Monsters from local ")]
    public static void ImportMonstersCSV()
    {
        var skillDict = BuildAssetDictionary<SkillDefinitionSO>();
        string csvPath = Path.Combine(Application.dataPath, "Data/Monster Combo - Monster List - Monsters.csv");
        
        // Read all lines, skip the header (line 0)
        string[] lines = File.ReadAllLines(csvPath).Skip(1).ToArray();


        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            // CSV columns: 0:id, 1:name, 2:spriteName, 3:race, 4:element, 5:maxHP, 6:strength, 7:skillNames
            
            string monsterId = data[0];
            string monsterName = data[1];
            string monsterSprite = data[2];
            string race = data[3];
            string element = data[4];

            string spritePath = $"Assets/Art/Monsters/{monsterSprite}.png";
            Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            
            // Stats parsing
            StatBlock stats = new StatBlock {
                maxHP = SafeParseFloat(data[6]),
                maxMana = SafeParseFloat(data[7]),
                strength = SafeParseFloat(data[8]),
                defense = SafeParseFloat(data[9]),
                intelligence = SafeParseFloat(data[10]),
                speed = SafeParseFloat(data[11]),
                critChance = SafeParseFloat(data[12]),
                critDamageMult = SafeParseFloat(data[13]),                
                dodgeChance = SafeParseFloat(data[14]),
            };

            // Skill parsing (splitting the '|' delimited string)
            List<SkillDefinitionSO> skills = new List<SkillDefinitionSO>();
            string[] skillNames = data[15].Split('|');
            foreach (string sName in skillNames)
            {
                if (skillDict.TryGetValue(sName, out var skill)) skills.Add(skill);
            }

            // Create/Update Asset
            string assetPath = $"Assets/SO/Monsters/{monsterId}.asset";
            MonsterDefinitionSO asset = AssetDatabase.LoadAssetAtPath<MonsterDefinitionSO>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<MonsterDefinitionSO>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            asset.Editor_ImportData(monsterId, 
                                    monsterName, 
                                    loadedSprite, 
                                    (MonsterRace)System.Enum.Parse(typeof(MonsterRace), race), 
                                    (ElementType)System.Enum.Parse(typeof(ElementType), element),
                                    stats, 
                                    null, //traits
                                    skills);
            
            EditorUtility.SetDirty(asset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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

        private static float SafeParseFloat(string input)
    {
        // Trim whitespace and try to parse
        if (float.TryParse(input.Trim(), out float result))
        {
            return result;
        }
        Debug.LogWarning($"Could not parse '{input}' as a float. Defaulting to 0.");
        return 0f;
    }
}
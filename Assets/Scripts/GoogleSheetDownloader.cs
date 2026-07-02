using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor; // <--- Required for Editor Coroutines

public static class GoogleSheetDownloader
{
    // This is what you click in the menu
    [MenuItem("Tools/Import Game Data/Import All From Google")]
    public static void StartImportProcess()
    {
        // Start the Coroutine in the Editor environment
        EditorCoroutineUtility.StartCoroutine(DownloadAllData(), typeof(GoogleSheetDownloader));
    }

    private static IEnumerator DownloadAllData()
    {
        string spreadsheetId = "10a2gxVXdlrloibwPqM_F_txR5OEFbuJWkibMdRx8eyg";
        var sheetsToDownload = new Dictionary<string, string>
        {
            { "Monsters", "0" },
            { "SkillsActions", "674001118" },
            { "Buffs", "1510479709" }
        };

        Dictionary<string, string> downloadedSheets = new Dictionary<string, string>();

        foreach (var sheet in sheetsToDownload)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=csv&gid={sheet.Value}";
            
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    downloadedSheets[sheet.Key] = www.downloadHandler.text;
                    Debug.Log($"Successfully downloaded {sheet.Key}");
                }
                else
                {
                    Debug.LogError($"Failed to download {sheet.Key}: {www.error}");
                    yield break; // Stop the process if a download fails
                }
            }
        }

        // Now process the data
        if (downloadedSheets.ContainsKey("Monsters"))
        {
            MonsterCSVImporter.ImportMonstersCSV();
        }
        
        Debug.Log("Full Import Complete!");
    }
}
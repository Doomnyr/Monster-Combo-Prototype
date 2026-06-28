using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.UIElements;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject settings;

    public void LoadGame()
    {
        StartCoroutine(WaitAndLoad("CombatScene", 1f));
    }

        IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Assign it from the new scene (adjust to your setup)
        if (settings == null)
            settings = GameObject.Find("SettingsPanel"); // or use a tag / cached type
        if (settings != null)
            settings.SetActive(false);
    }   

    public void OpenSettingPanel()
    {
        if (settings != null) settings.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        if (settings != null) settings.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}

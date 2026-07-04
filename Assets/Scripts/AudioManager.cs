using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

       void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(true);
    }



    public void PlaySound2D(string soundName)
    {
        Debug.Log($"AudioManager instance: {this.name} active={gameObject.activeInHierarchy}, " +
              $"sfx2DSource obj active={sfx2DSource?.gameObject.activeInHierarchy}, " +
              $"sfx2DSource enabled={sfx2DSource?.enabled}");

        if (sfx2DSource == null)
        {
            Debug.LogError("sfx2DSource not assigned.");
            return;
        }

        if (!sfx2DSource.enabled)
        {
            Debug.LogError("sfx2DSource component is disabled.");
            return;
        }

        if (!sfx2DSource.gameObject.activeInHierarchy)
        {
            Debug.LogError("sfx2DSource GameObject is not active in hierarchy.");
            return;
        }

        var clip = sfxLibrary.GetClipFromName(soundName);
        if (clip == null)
        {
            Debug.LogError($"No clip found for soundName: {soundName}");
            return;
        }

        AudioManager.Instance.sfx2DSource.PlayOneShot(clip, 1f);
    }
}

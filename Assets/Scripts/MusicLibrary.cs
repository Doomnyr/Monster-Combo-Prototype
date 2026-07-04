using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip clip;
}
public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] musicTrack;

    public AudioClip GetClipFromName(string name)
    {
        foreach (var track in musicTrack)
        {
            if (track.trackName == name)
            {
                return track.clip;
            }
        }

        Debug.Log("Track not found.");
        return null;

    }


}

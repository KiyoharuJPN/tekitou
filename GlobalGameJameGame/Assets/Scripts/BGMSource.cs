using UnityEngine;

public class BGMSource : MonoBehaviour
{
    public static BGMSource instance;
    AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
        audioSource = GetComponent<AudioSource>();
    }

    public void Volume(float v) { audioSource.volume = v; }
    public float GetVolume() { return audioSource.volume; }

    public void ChangeMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}

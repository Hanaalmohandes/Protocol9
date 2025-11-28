using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSFX(AudioClip clip1, AudioClip clip2)
    {
        AudioClip chosen = (Random.Range(0, 2) == 0) ? clip1 : clip2;
        sfxSource.PlayOneShot(chosen);
    }
}
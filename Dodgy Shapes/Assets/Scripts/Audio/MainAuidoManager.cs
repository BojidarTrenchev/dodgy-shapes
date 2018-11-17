using UnityEngine;
using UnityEngine.SceneManagement;
public class MainAuidoManager : MonoBehaviour
{
    public static MainAuidoManager mainAudio;

    public AudioClip clickSound;

    public static bool isPlayingSound;
    public static bool isPlayingMusic;

    public bool IsPlayingSound
    {
        get
        {
            return sfxSource.isPlaying;
        }
    }

    public AudioSource sfxSource;
    public AudioSource musicSource;


    public void Awake()
    {
        if (mainAudio == null)
        {
            mainAudio = this;
        }
        else if (mainAudio != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayClickSound()
    {
        if (isPlayingSound)
        {
            sfxSource.PlayOneShot(clickSound,0.3f);
        }
    }

    public void PlaySound(AudioClip sound, float volume = 1)
    {
        if (isPlayingSound)
        {
            sfxSource.PlayOneShot(sound, volume);
        }
    }

    public void StartStopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            isPlayingMusic = false;
        }
        else
        {
            musicSource.Play();
            isPlayingMusic = true;
        }
    }
}

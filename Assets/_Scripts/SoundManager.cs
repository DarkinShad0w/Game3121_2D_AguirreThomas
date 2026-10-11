using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip wallHit;
    public AudioClip obstacleHit;
    public AudioClip death;
    public AudioClip beep;
    public AudioClip backgroundMusic;

    [Header("Music")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    private AudioSource audioSource;
    private AudioSource beepSource;
    private AudioSource musicSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        beepSource = gameObject.AddComponent<AudioSource>();
        beepSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        musicSource.Play();
    }

    public void PlayWallHit(float volume)
    {
        audioSource.PlayOneShot(wallHit, volume);
    }

    public void PlayObstacleHit()
    {
        audioSource.PlayOneShot(obstacleHit);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(death);
    }

    public void PlayBeep(float pitch)
    {
        beepSource.pitch = pitch;
        beepSource.PlayOneShot(beep);
    }
}

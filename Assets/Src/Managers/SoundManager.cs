using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip battleMusic;

    [Header("UI")]
    [SerializeField] private AudioClip softClick;
    [SerializeField] private AudioClip normalClick;
    [SerializeField] private AudioClip hardClick;

    [Header("Fight")]
    [SerializeField] private AudioClip blankAttack;

    [Header("Game")]
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayOneShot(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBlankAttack() => PlayOneShot(blankAttack);
    public void PlaySoftClick() => PlayOneShot(softClick);
    public void PlayNormalClick() => PlayOneShot(normalClick);
    public void PlayHardClick() => PlayOneShot(hardClick);
    public void PlayVictory() => PlayOneShot(victory);
    public void PlayGameOver() => PlayOneShot(gameOver);

    public void PlayMenuMusic() => PlayMusic(menuMusic, true);
    public void PlayBattleMusic() => PlayMusic(battleMusic, true, 0.5f);
}
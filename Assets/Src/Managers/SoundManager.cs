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
    [SerializeField] private AudioClip[] bossBattleThemes;

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

    public void ApplySavedVolumes()
    {
        SetMusicVolume(GameSave.MusicVolume);
        SetSfxVolume(GameSave.SfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume * GameSave.MusicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, GameSave.SfxVolume);
    }

    public void PlayBlankAttack() => PlayOneShot(blankAttack);
    public void PlaySoftClick() => PlayOneShot(softClick);
    public void PlayNormalClick() => PlayOneShot(normalClick);
    public void PlayHardClick() => PlayOneShot(hardClick);
    public void PlayVictory() => PlayOneShot(victory);
    public void PlayGameOver() => PlayOneShot(gameOver);

    public void PlayMenuMusic() => PlayMusic(menuMusic, true, 1f);

    public void PlayBattleMusic() => PlayBattleMusicForLevel(0);

    public void PlayBattleMusicForLevel(int level)
    {
        AudioClip clip = battleMusic;
        if (bossBattleThemes != null && bossBattleThemes.Length > 0)
        {
            int index = Mathf.Clamp(level, 0, bossBattleThemes.Length - 1);
            if (bossBattleThemes[index] != null)
            {
                clip = bossBattleThemes[index];
            }
        }

        PlayMusic(clip, true, 0.5f);
    }
}

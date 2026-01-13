using UnityEngine;

public class AudioHospitalManager : MonoBehaviour
{
    public static AudioHospitalManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip sceneMusic; // Nhạc nền cho scene hiện tại

    [Header("SFX Clips")]
    public AudioClip buttonClickSFX;
    public AudioClip healSFX;      // Âm thanh hồi máu
    public AudioClip reviveSFX;    // Âm thanh hồi sinh
    public AudioClip loseSFX;      // Âm thanh lỗi/không đủ tiền

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    void Awake()
    {
        // Singleton cho scene hiện tại
        Instance = this;
        InitializeAudioSources();
    }

    void Start()
    {
        // Tự động phát nhạc nền khi scene load
        if (sceneMusic != null)
        {
            PlayMusic(sceneMusic);
        }
    }

    void InitializeAudioSources()
    {
        // Tạo AudioSource nếu chưa có
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    /* =========================
       MUSIC CONTROLS
       ========================= */

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    /* =========================
       SFX CONTROLS
       ========================= */

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayHeal()
    {
        PlaySFX(healSFX);
    }

    public void PlayRevive()
    {
        PlaySFX(reviveSFX);
    }

    public void PlayLose()
    {
        PlaySFX(loseSFX);
    }

    /* =========================
       VOLUME CONTROLS (Optional)
       ========================= */

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
}
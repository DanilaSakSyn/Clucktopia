using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Projects.Games.Garden
{
    public class GardenGameAudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip victoryMusic;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip buttonSpawnSound;
        [SerializeField] private AudioClip buttonExpireSound;
        [SerializeField] private AudioClip gameStartSound;
        [SerializeField] private AudioClip countdownSound;
        
        [Header("Audio Settings")]
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private bool enableSounds = true;
        
        private bool isGameActive = false;
        
        private void Start()
        {
            SetupAudioSources();
            PlayBackgroundMusic();
        }
        
        private void OnEnable()
        {
            GardenGameController.OnGameEnded += OnGameEnded;
            GardenGameController.OnScoreChanged += OnScoreChanged;
        }
        
        private void OnDisable()
        {
            GardenGameController.OnGameEnded -= OnGameEnded;
            GardenGameController.OnScoreChanged -= OnScoreChanged;
        }
        
        private void SetupAudioSources()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
                musicSource.loop = true;
            }
            
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
                sfxSource.loop = false;
            }
        }
        
        public void PlayBackgroundMusic()
        {
            if (!enableSounds || musicSource == null || backgroundMusic == null) return;
            
            if (musicSource.clip != backgroundMusic)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        
        public void PlayVictoryMusic()
        {
            if (!enableSounds || musicSource == null || victoryMusic == null) return;
            
            musicSource.clip = victoryMusic;
            musicSource.Play();
        }
        
        public void PlayButtonClickSound()
        {
            PlaySFX(buttonClickSound);
        }
        
        public void PlayButtonSpawnSound()
        {
            PlaySFX(buttonSpawnSound);
        }
        
        public void PlayButtonExpireSound()
        {
            PlaySFX(buttonExpireSound);
        }
        
        public void PlayGameStartSound()
        {
            PlaySFX(gameStartSound);
        }
        
        public void PlayCountdownSound()
        {
            PlaySFX(countdownSound);
        }
        
        private void PlaySFX(AudioClip clip)
        {
            if (!enableSounds || sfxSource == null || clip == null) return;
            
            sfxSource.PlayOneShot(clip);
        }
        
        private void OnGameEnded(int score, int money)
        {
            isGameActive = false;
            StartCoroutine(PlayEndGameSequence());
        }
        
        private void OnScoreChanged(int newScore)
        {
            PlayButtonClickSound();
        }
        
        private IEnumerator PlayEndGameSequence()
        {
            yield return new WaitForSeconds(0.5f);
            PlayVictoryMusic();
        }
        
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
        
        public void ToggleSounds(bool enabled)
        {
            enableSounds = enabled;
            
            if (!enabled)
            {
                if (musicSource != null) musicSource.Stop();
                if (sfxSource != null) sfxSource.Stop();
            }
            else
            {
                PlayBackgroundMusic();
            }
        }
    }
}

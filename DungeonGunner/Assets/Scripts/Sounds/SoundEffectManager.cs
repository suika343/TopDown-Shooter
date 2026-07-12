using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume")){
            soundsVolume = PlayerPrefs.GetInt("soundsVolume", soundsVolume);
        }

        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume++;

        SetSoundsVolume(soundsVolume);
    }

    public void DecreaseSoundsVolume()
    {
        if (soundsVolume <= 0) return;

        soundsVolume--;

        SetSoundsVolume(soundsVolume);
    }

    public void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        this.soundsVolume = soundsVolume;

        if (soundsVolume <= 0)
        {
            GameResources.Instance.soundsMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibel(soundsVolume));
        }
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, transform.position, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    private IEnumerator DisableSound(SoundEffect sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        sound.gameObject.SetActive(false);
    }
}

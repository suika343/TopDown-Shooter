using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        SetSoundsVolume(soundsVolume);
    }

    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;
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

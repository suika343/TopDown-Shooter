using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicLevelText;
    [SerializeField] private TextMeshProUGUI sfxLevelText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator InitializeUI()
    {
        //wait for one frame to ensure that the SoundEffectManager and MusicManager instances are initialized
        yield return null;

        sfxLevelText.text = SoundEffectManager.Instance.soundsVolume.ToString();
        musicLevelText.text = MusicManager.Instance.musicVolume.ToString();

        musicSlider.value = MusicManager.Instance.musicVolume;
        sfxSlider.value = SoundEffectManager.Instance.soundsVolume;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void MusicSliderValue()
    {
        MusicManager.Instance.SetMusicVolume((int)musicSlider.value);
        musicLevelText.text = MusicManager.Instance.musicVolume.ToString();
    }

    public void SFXSliderValue()
    {
        SoundEffectManager.Instance.SetSoundsVolume((int)sfxSlider.value);
        sfxLevelText.text = SoundEffectManager.Instance.soundsVolume.ToString();
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.text = MusicManager.Instance.musicVolume.ToString();
    }

    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.text = MusicManager.Instance.musicVolume.ToString();
    }

    public void IncreaseSFXVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        sfxLevelText.text = SoundEffectManager.Instance.soundsVolume.ToString();
    }

    public void DecreaseSFXVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        sfxLevelText.text = SoundEffectManager.Instance.soundsVolume.ToString();
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(sfxLevelText), sfxLevelText);
    }
#endif
    #endregion
}

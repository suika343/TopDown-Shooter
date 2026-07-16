using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region HEADER GAMEOBECT REFERENCES
    [Space(10)]
    [Header("GameObect References")]
    #endregion
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScoreButton;
    [SerializeField] private GameObject returnToMainMenuButton;

    private bool isHighScoresSceneLoaded = false;
    void Start()
    {
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
        //play music
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        returnToMainMenuButton.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadHighScores()
    {
        playButton.SetActive(false);
        highScoreButton.SetActive(false);

        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");
        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector() {

        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;

        }

        highScoreButton.SetActive(true);
        playButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoreButton), highScoreButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
    #endregion
}

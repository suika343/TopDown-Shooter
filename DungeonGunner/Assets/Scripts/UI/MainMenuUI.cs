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
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject instructionsButton;

    private bool isHighScoresSceneLoaded = false;
    private bool isInstructionsSceneLoaded = false;
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
        quitButton.SetActive(false);
        instructionsButton.SetActive(false);
        highScoreButton.SetActive(false);

        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");
        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        instructionsButton.SetActive(false);
        highScoreButton.SetActive(false);

        isInstructionsSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");
        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector() {

        returnToMainMenuButton.SetActive(false);
        
        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionsSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionsSceneLoaded = false;
        }

        highScoreButton.SetActive(true);
        playButton.SetActive(true);
        instructionsButton.SetActive(true);
        quitButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoreButton), highScoreButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
    }
#endif
    #endregion
}

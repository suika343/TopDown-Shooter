using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
        //play music
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

}

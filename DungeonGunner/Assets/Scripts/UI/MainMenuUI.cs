using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    void Start()
    {
        //play music
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

}

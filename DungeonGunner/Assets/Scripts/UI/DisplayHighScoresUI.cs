using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DisplayHighScoresUI : MonoBehaviour
{
    #region HEADER GAMEOBECT REFERENCES
    [Space(10)]
    [Header("GameObect References")]
    #endregion
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        HighScores highScores = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameObect;

        int rank = 0;
        foreach(Score score in highScores.scoreList)
        {
            rank++;

            //instantiate score prefab
            scoreGameObect = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);

            ScorePrefab scorePrefab = scoreGameObect.GetComponent<ScorePrefab>();
            scorePrefab.rankText.text = rank.ToString();
            scorePrefab.nameText.text = score.playerName;
            scorePrefab.levelText.text = score.levelDescription;
            scorePrefab.scoreText.text = score.playerScore.ToString("###,###");

        }

        //Add blank line
        scoreGameObect = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
        ScorePrefab scorePrefabBlank = scoreGameObect.GetComponent<ScorePrefab>();
        scorePrefabBlank.rankText.text = "";
        scorePrefabBlank.nameText.text = "";
        scorePrefabBlank.levelText.text = "";
        scorePrefabBlank.scoreText.text = "";
    }
}

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores highScores = new HighScores();

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    //Load Scores from List
    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if(File.Exists(Application.persistentDataPath + "/DungeonGunnerHighScores.dat"))
        {
            ClearScoresList();

            FileStream fs = File.OpenRead(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

            highScores = (HighScores)bf.Deserialize(fs);

            fs.Close();
        }
    }

    private void ClearScoresList()
    {
        highScores.scoreList.Clear();
    }

    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        if (highScores.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            highScores.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = File.Create(Application.persistentDataPath + "/DungeonGunnerHighScores.dat");

        bf.Serialize(fs, highScores);

        fs.Close();
    }

    public HighScores GetHighScores()
    {
        return highScores;
    }

    public int GetRank(long playerScore)
    {
        //if there are no scores in the list - then this score is ranked 1 and then return immediately
        if(highScores.scoreList.Count == 0)
        {
            return 1;
        }

        int index = 0;

        //loop through scores list to find the rank of this score
        for(int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if(playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }
        }

        if(highScores.scoreList.Count < Settings.numberOfHighScoresToSave)
        {
            return (index + 1);
        }

        //score isnt high enough to save
        return 0;
    }
}

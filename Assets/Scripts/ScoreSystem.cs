using DG.Tweening;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ScoreData
{
    public int score;

    public ScoreData()
    {
        score = 0;
    }
}

public class ScoreSystem : Singleton<ScoreSystem>
{
    public ScoreData scoreData;
    public TMP_Text scoreText;
    public GameObject starPrefab;
    public GameObject mainCanvas;
    public GameObject starImage;

    private void Start()
    {
        //LoadScoreData();
        SetScoreText();
    }

    public void AddScore(int points)
    {
        scoreData.score += points;
        SetScoreText();
        SaveScoreData();
        LevelManager.Instance.CheckWin();
    }

    public void ResetScore()
    {
        scoreData.score = 0;
        SetScoreText();
        SaveScoreData();
    }

    public int GetScore()
    {
        return scoreData.score;
    }

    public void SetScoreText()
    {
        scoreText.text = scoreData.score.ToString();
    }

    public void SendStar(Vector3 pos)
    {
        GameObject star = Instantiate(starPrefab, Camera.main.WorldToScreenPoint(pos), Quaternion.identity,
            mainCanvas.transform);

        star.transform.DORotate(starImage.transform.eulerAngles, 0.75f);
        star.transform.DOMove(starImage.transform.position, 0.75f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(star);
            AddScore(1);
        });
    }


    public void SaveScoreData()
    {
        string json = JsonUtility.ToJson(scoreData);
        PlayerPrefs.SetString("ScoreData", json);
        PlayerPrefs.Save();
    }

    public void LoadScoreData()
    {
        if (PlayerPrefs.HasKey("ScoreData"))
        {
            string json = PlayerPrefs.GetString("ScoreData");
            scoreData = JsonUtility.FromJson<ScoreData>(json);
        }
        else
        {
            scoreData = new ScoreData();
        }
    }
}
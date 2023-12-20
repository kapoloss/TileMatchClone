
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : Singleton<LevelManager>
{
    public List<GameObject> levePrefabList;

    public int currentLevel;
    public GameObject currentLevelPrefab;
    public LevelDataHolder currentLevelDataHolder;
    public LevelDataSO currentLevelDataSo;
    public GridSystem currentGridSystem;
    public int startPieceCount;

    [Header("WinPanel")]
    public GameObject winPanel;
    public TMP_Text winLevelText;
    public TMP_Text earnedStarCountText;
    
    [Header("LosePanel")]
    public GameObject losePanel;
    public TMP_Text loseLevelText;


    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetLevel();
    }

    public void OpenWinPanel()
    {
        winPanel.transform.localScale = Vector3.zero;
        winPanel.SetActive(true);
        
        winLevelText.text = "LEVEL" + (currentLevel + 1).ToString();
        earnedStarCountText.text = ScoreSystem.Instance.GetScore().ToString();
        winPanel.transform.DOScale(Vector3.one, 0.5f);
        
        PlayfabManager.Instance.SendLeaderBoard(1);
    }

    public void CloseWinPanel()
    {
       
        winPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            winPanel.SetActive(false);
        });
    }
    
    public void OpenLosePanel()
    {
        losePanel.transform.localScale = Vector3.zero;
        losePanel.SetActive(true);
        loseLevelText.text = "LEVEL" + (currentLevel + 1).ToString();

        losePanel.transform.DOScale(Vector3.one, 0.5f);
    }

    public void CloseLosePanel()
    {
        losePanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            losePanel.SetActive(false);
        });;
    }
    
    public void Restart()
    {
        SetLevel();
        CloseLosePanel();
        
    }
    
    public void NextLevel()
    {
        currentLevel++;
        SetLevel();
        SaveLevel();
        CloseWinPanel();
    }

    public void SetLevel()
    {
        if (currentLevelPrefab != null)
        {
            Destroy(currentLevelPrefab);
        }
        ScoreSystem.Instance.ResetScore();
        GameControl.Instance.takenPieces.Clear();
        
        currentLevelPrefab = Instantiate(levePrefabList[currentLevel % levePrefabList.Count], Vector3.zero, Quaternion.identity, null);
        currentLevelDataHolder = currentLevelPrefab.GetComponent<LevelDataHolder>();
        currentLevelDataSo = currentLevelDataHolder.levelDataSo;
        //startPieceCount = currentLevelDataHolder.gridSystem.startPieceCount;
    }

    public void SaveLevel()
    {
        PlayerPrefs.SetInt("currentLevel",currentLevel);
    }
    
    public void CheckWin()
    {
        if (ScoreSystem.Instance.GetScore() == startPieceCount)
        {
            OpenWinPanel();
        }
    }
    
}

// [System.Serializable]
// public struct WinPanelSerialize()
// {
//     public 
// }
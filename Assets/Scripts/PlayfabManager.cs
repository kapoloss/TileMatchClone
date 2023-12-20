using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayfabManager : Singleton<PlayfabManager>
{
    public Animator animator;
    public Transform infoParent;
    
    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,OnSuccess, OnError);
        
    }

    private void OnError(PlayFabError obj)
    { 
        Debug.Log("Login Fail"); 
    }

    private void OnSuccess(LoginResult obj)
    { 
        Debug.Log("Login Successful");
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request,OnLeaderBoardUpdate,OnError);
    }

    private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult obj)
    {
        Debug.Log("Successful update");
    }

    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(request,OnLeaderBoardGet,OnError);
    }

    private void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach (Transform child in infoParent)
        {
            child.gameObject.SetActive(false);
        }

        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            GameObject info = infoParent.transform.GetChild(i).gameObject;
            info.SetActive(true);
            info.transform.SetSiblingIndex(result.Leaderboard[i].Position);
            info.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text =
                (result.Leaderboard[i].Position + 1).ToString();
            info.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text =
                result.Leaderboard[i].PlayFabId.Substring(0,4);
            info.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text =
                result.Leaderboard[i].StatValue.ToString();
            
        }
        // foreach (var item in result.Leaderboard)
        // {
        //     Debug.Log(item.StatValue + "" + item.PlayFabId + "" + item.Position); 
        // }
    }
}

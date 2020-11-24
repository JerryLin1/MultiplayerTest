using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar.Collections;
using MLAPI.Spawning;

public class UIManager : NetworkedBehaviour
{
    // public NetworkedDictionary<string, int> scores = new NetworkedDictionary<string, int>();
    private Transform scoreboard;
    private void Start()
    {
        // scoreboard = transform.Find("Scoreboard");
        // scoreboard.gameObject.SetActive(false);
    }
    public void StartHost()
    {
        NetworkingManager.Singleton.StartHost();
        StartGame();
    }
    public void StartClient()
    {
        NetworkingManager.Singleton.StartClient();
        StartGame();
    }
    void StartGame()
    {
        GetName();
        DisableButtons();
        ActivateCrosshair();
        // scoreboard.gameObject.SetActive(true);
        // UpdateScoreboard();
    }
    void DisableButtons()
    {
        gameObject.transform.Find("Buttons").gameObject.SetActive(false);
    }
    void ActivateCrosshair()
    {
        GameObject.Find("Crosshair").GetComponent<Crosshair>().SetEnable(true);
    }

    void GetName()
    {
        string username = gameObject.transform.Find("Buttons").Find("Name").GetComponent<InputField>().text;
        if (username == "") username = "Player " + NetworkingManager.Singleton.LocalClientId.ToString();
    }

    // [ServerRPC]
    // public void AddPoint(string name)
    // {
    //     if (!IsServer) return;
    //     int outvalue;
    //     if (scores.TryGetValue(name, out outvalue))
    //     {
    //         scores[name] = outvalue + 1;
    //     }
    //     UpdateScoreboard();
    // }
    // void UpdateScoreboard()
    // {
    //     string scoreText = "";
    //     foreach (KeyValuePair<string, int> entry in scores)
    //     {
    //         scoreText = entry.Key + ": " + entry.Value + "\n";
    //     }
    //     scoreboard.GetComponentInChildren<Text>().text = scoreText;
    // }
}

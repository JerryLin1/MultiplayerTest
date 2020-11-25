using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar.Collections;
using MLAPI.Spawning;
using MLAPI.Connection;

public class UIManager : NetworkedBehaviour
{
    private Transform scoreboard;
    private void Start()
    {
        scoreboard = transform.Find("Scoreboard");
        scoreboard.gameObject.SetActive(false);
    }
    public void StartHost()
    {
        NetworkingManager.Singleton.StartHost();
        StartGame();
    }
    public void StartClient()
    {
        NetworkingManager.Singleton.StartClient();
        // NetworkingManager.Singleton.ConnectionApprovalCallback+=( connectionData, clientId, connApprovalDel )=>StartGame();
        StartGame();
    }
    void StartGame()
    {
        GetName();
        DisableButtons();
        ActivateCrosshair();
        scoreboard.gameObject.SetActive(true);
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
        // SpawnManager.GetLocalPlayerObject().name = username;
		// SpawnManager.GetLocalPlayerObject().GetComponent<Player>().username = username;
    }
}

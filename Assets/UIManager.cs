﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.NetworkedVar.Collections;
using MLAPI.Spawning;
using MLAPI.Connection;

public class UIManager : NetworkedBehaviour
{
    private static NetworkedVarSettings nvsOwner = new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly };
    private static NetworkedVarSettings nvsEveryone = new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone };
    private Transform scoreboard;
    private NetworkedDictionary<string, int> ScoreTracker = new NetworkedDictionary<string, int>(nvsEveryone);
    private List<ulong> clientIdList = new List<ulong>();
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
        StartGame();
    }
    void StartGame()
    {
        if (IsHost)
            InitiatePlayer(NetworkingManager.Singleton.LocalClientId);
        else if (IsClient)
            NetworkingManager.Singleton.OnClientConnectedCallback += InitiatePlayer;
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
    void InitiatePlayer(ulong clientId)
    {
        string username = gameObject.transform.Find("Buttons").Find("Name").GetComponent<InputField>().text;
        if (username == "") username = "Player " + NetworkingManager.Singleton.LocalClientId.ToString();
        ScoreTracker.Add(username, 0);
        SpawnManager.GetPlayerObject(clientId).name = username;
        SpawnManager.GetPlayerObject(clientId).GetComponent<Player>().username.Value = username;
        InvokeServerRpc(UpdateScoreboard);
    }
    [ServerRPC(RequireOwnership = false)]
    public void AddPoint(string name)
    {
        Debug.Log("poo");
        int points;
        if (ScoreTracker.TryGetValue(name, out points))
        {
            ScoreTracker[name] = points + 1;
        }
        InvokeServerRpc(UpdateScoreboard);
    }
    [ServerRPC(RequireOwnership = false)]
    public void UpdateScoreboard()
    {
        string sct = "";
        foreach (KeyValuePair<string, int> entry in ScoreTracker)
        {
            sct += entry.Key + ": " + entry.Value + "\n";
        }
        scoreboard.GetComponentInChildren<Text>().text = sct;
        InvokeClientRpcOnEveryone(UpdateScoreboardClient, sct);
    }
    //cringe way to do it but i lazy
    [ClientRPC]
    void UpdateScoreboardClient(string scoreboardText)
    {
        scoreboard.GetComponentInChildren<Text>().text = scoreboardText;
    }
}

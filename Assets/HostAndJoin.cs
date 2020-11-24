using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;

public class HostAndJoin : NetworkedBehaviour
{
    public GameObject playerPrefab;
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
        DisableButtons();
        ActivateCrosshair();
    }
    void DisableButtons()
    {
        gameObject.transform.Find("Buttons").gameObject.SetActive(false);
    }
    void ActivateCrosshair()
    {
        GameObject.Find("Crosshair").GetComponent<Crosshair>().SetEnable(true);
    }
    void GetName() {
        // TODO: do this
    }
}

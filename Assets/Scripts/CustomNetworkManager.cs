using UnityEngine;
using Mirror;
using System.Collections.Generic;
using Steamworks;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] PlayerObjectController gamePlayerPrefab;

    public List<PlayerObjectController> gamePlayer = new List<PlayerObjectController>();

    bool gameStarted;

    public Transform seekerSpawn;



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerObjectController gamePlayerInstance = conn.identity.gameObject.GetComponent<PlayerObjectController>();

        gamePlayerInstance.connectionID = conn.connectionId;
        gamePlayerInstance.playerID = gamePlayer.Count + 1;
        gamePlayerInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, gamePlayer.Count);

        if (conn.authenticationData is bool authData)
        {
            gamePlayerInstance.isSeeker = authData;
        }
    }

    [Server]
    public void StartGame(string sceneName) 
    {
        if (gameStarted) { return; }

        gameStarted = true;

        if (LobbyController.instance != null)
        {
            Destroy(LobbyController.instance);
        }
        
        ServerChangeScene(sceneName);
    }

    [Server]
    public void SetSeeker()
    {
        int seekerIndex = Random.Range(0, gamePlayer.Count);

        for (int i = 0; i < gamePlayer.Count; i++)
        {
            if (i == seekerIndex)
            {
                gamePlayer[i].RPCSetSeeker();
            }
            else
            {
                gamePlayer[i].RPCSetHider();
            }
        }
    }
    [Server]
    public void AnnounceRole()
    {
        for (int i = 0; i < gamePlayer.Count; i++)
        {
            if (gamePlayer[i].isSeeker)
            {
                gamePlayer[i].RPCAnnounce("You are a Seeke");
            }
            else
            {
                gamePlayer[i].RPCAnnounce("You are a Hider");
            }
        }
    }

}

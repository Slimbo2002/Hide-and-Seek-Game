using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;
public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    public TextMeshProUGUI lobbyNameText;

    public GameObject playerListViewContents;
    public GameObject PlayerListItemPrefab;
    public GameObject localPlayerObject;

    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController localPlayerController;

    int privacyIndex = 0;
    public TextMeshProUGUI privacyText, playerCount;

    CustomNetworkManager networkManager;
    CustomNetworkManager NetworkManager
    {
        get
        {
            if (networkManager != null)
            {
                return networkManager;
            }
            return networkManager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        privacyText.text = "Public"; 
        
    }
    private void Update()
    {
        MaxPlayerCount(); 
        
    }

    public void UpdateLobbyName()
    {
        currentLobbyID = NetworkManager.GetComponent<SteamLobby>().currentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated)
        {
            CreateHostPlayerItem();
        }
        if(playerListItems.Count < NetworkManager.gamePlayer.Count)
        {
            CreateClientPlayerItem();
        }
        if(playerListItems.Count > NetworkManager.gamePlayer.Count)
        {
            RemovePlayerItem();
        }
        if(playerListItems.Count == NetworkManager.gamePlayer.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }
    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in NetworkManager.gamePlayer)
        {
            GameObject newPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.connectionID = player.connectionID;
            newPlayerItemScript.playerSteamID = player.playerSteamID;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(playerListViewContents.transform, false);
            newPlayerItem.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerItemScript);
        }
        playerItemCreated = true;
    }
    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.gamePlayer)
        {
            if (player == null || player.gameObject == null)
                continue; // Skip destroyed players

            if (!playerListItems.Any(b => b.connectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(PlayerListItemPrefab);
                if (newPlayerItem == null) continue;

                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
                if (newPlayerItemScript == null) continue;

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamID;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(playerListViewContents.transform, false);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }
        }
    }


    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in NetworkManager.gamePlayer)
        {
            foreach(PlayerListItem playerListItemScript in playerListItems)
            {
                if(playerListItemScript.connectionID == player.connectionID)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.SetPlayerValues();
                }
            }
        }
    }
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem playerListItem in playerListItems)
        {
            if(!networkManager.gamePlayer.Any(b=> b.connectionID == playerListItem.connectionID))
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }
        if(playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem PlayerListItemToRemove in playerListItemToRemove)
            {
                GameObject objectToRemove = PlayerListItemToRemove.gameObject;
                playerListItems.Remove(PlayerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
    public void RotateLobbyPrivacy()
    {
        privacyIndex += 1;
        
        if(privacyIndex > 2)
        {
            privacyIndex = 0;
        }

        switch (privacyIndex)
        {
            case 0:
                ChangeLobbyPrivacy(ELobbyType.k_ELobbyTypePrivate);
                privacyText.text = "Private";
                break;
            case 1:
                ChangeLobbyPrivacy(ELobbyType.k_ELobbyTypeFriendsOnly);
                privacyText.text = "Friends";
                break;
            case 2:
                ChangeLobbyPrivacy(ELobbyType.k_ELobbyTypePublic);
                privacyText.text = "Public";
                break;
        
        }
    }

    public void ChangeLobbyPrivacy(ELobbyType newType)
    {
        if (currentLobbyID == 0)
        {
            Debug.LogError("No active lobby to modify.");
            return;
        }

        CSteamID lobbyID = new CSteamID(currentLobbyID);
        SteamMatchmaking.SetLobbyType(lobbyID, newType);
        Debug.Log("Lobby privacy changed to: " + newType);
    }

    public void StartGame(string sceneName)
    {
        StartCoroutine(GameStart(sceneName));
    }
    void ChangeScene(string sceneName)
    {
        localPlayerController.StartGame(sceneName);
    }
    IEnumerator GameStart(string sceneName)
    {
        NetworkManager.SetSeeker();
        NetworkManager.AnnounceRole();

        yield return new WaitForSeconds(3f);

        ChangeScene(sceneName);
    }
    void MaxPlayerCount()
    {
        playerCount.text = $"{NetworkManager.gamePlayer.Count}/{NetworkManager.maxConnections}";
    }

}

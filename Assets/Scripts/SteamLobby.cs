using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEditor.Analytics;
using UnityEngine.Rendering;
using HeathenEngineering.SteamworksIntegration;

public class SteamLobby : MonoBehaviour
{
    //callbacks
    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEnter;

    public ulong currentLobbyID;
    const string HostAddressKey = "HostAddress";
    CustomNetworkManager manager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        if (!SteamManager.Initialized) return;

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }
    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        Debug.Log("Lobby Created");
        manager.StartHost();

        CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        currentLobbyID = callback.m_ulSteamIDLobby;

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

    }

    void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        // Check if already in a lobby
        if (currentLobbyID != 0)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(currentLobbyID));
            currentLobbyID = 0; // Reset current lobby ID
        }

        // Join the new lobby
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = callback.m_ulSteamIDLobby;

        if (!NetworkServer.active) // If not the host, start the client
        {
            manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            manager.StartClient();
        }
    }
 
    

}


using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.Examples.CouchCoop;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int connectionID;
    [SyncVar] public int playerID;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook =nameof(PlayerNameUpdate))] public string playerName;

    [SyncVar] public bool isSeeker;


    Rigidbody rb;

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
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (SceneManager.GetActiveScene().name == "HouseMap")
        {
            if (isSeeker && GameManager.instance.seekerSpawn != null)
            {

                StartCoroutine(moveSeeker());
            }
        }
    }

    public override void OnStartAuthority()
    {
        cmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";

        if (LobbyController.instance != null)
        {
            LobbyController.instance.FindLocalPlayer();
            LobbyController.instance.UpdateLobbyName();
        }
        

        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }

    }
    public override void OnStartClient()
    {

        NetworkManager.gamePlayer.Add(this);
        rb = GetComponent<Rigidbody>();

        if (LobbyController.instance != null)
        {
            LobbyController.instance.UpdateLobbyName();
            LobbyController.instance.UpdatePlayerList();
        }
        

        
    }

    public override void OnStopClient()
    {
        NetworkManager.gamePlayer.Remove(this);

        if(LobbyController.instance != null)
        {
            LobbyController.instance.UpdatePlayerList();
        }
        
    }
    [Command]
    void cmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.playerName = newValue;
        }
        if (isClient)
        {
            if (LobbyController.instance != null)
            {
                LobbyController.instance.UpdatePlayerList();
            }
            
        }
    }
    public void RPCSetSeeker()
    {
        Debug.Log("RPCSetSeeker");
        isSeeker = true;

        NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();
        NetworkConnectionToClient conn = networkIdentity.connectionToClient;

        conn.authenticationData = isSeeker;
    }
    public void RPCSetHider()
    {
        Debug.Log("RPCSet");
        isSeeker =false;

        NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();
        NetworkConnectionToClient conn = networkIdentity.connectionToClient;

        conn.authenticationData = isSeeker;
    }
    [TargetRpc]
    public void RPCAnnounce(string message)
    {
        UIManager.Instance.ShowRole(message);
    }

    public void StartGame(string sceneName)
    {
        if (isServer)
        {
            cmdStartGame(sceneName);
        }
    }
    [Command]
    public void cmdStartGame(string sceneName)
    {
        NetworkManager.StartGame(sceneName);
    }
    public IEnumerator moveSeeker()
    {
        yield return new WaitForSeconds(.2f);


        rb.MovePosition(GameManager.instance.seekerSpawn.transform.position);
    }
}

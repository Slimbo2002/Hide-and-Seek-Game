using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.Examples.CouchCoop;
using UnityEngine.SceneManagement;

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
        
    }

    public override void OnStartAuthority()
    {
        cmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();

        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }
        
    }
    public override void OnStartClient()
    {
        NetworkManager.gamePlayer.Add(this);
        rb = GetComponent<Rigidbody>();
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();

        if (SceneManager.GetActiveScene().name == "HouseMap")
        {
            if (isSeeker)
            {
                rb.linearVelocity = Vector3.zero; // Reset any movement forces
                rb.angularVelocity = Vector3.zero; // Reset rotation forces
                rb.isKinematic = true; // Fully disable physics


                transform.position = GameManager.instance.seekerSpawn.transform.position; // Manually set position
                rb.isKinematic = false; // Re-enable physics

            }
        }
    }

    public override void OnStopClient()
    {
        NetworkManager.gamePlayer.Remove(this);
        LobbyController.instance.UpdatePlayerList();
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
            LobbyController.instance.UpdatePlayerList();
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
}

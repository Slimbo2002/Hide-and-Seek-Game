using UnityEngine;
using Mirror;
using System.Collections;
public class GameManager : NetworkBehaviour
{
    public Transform hiderSpawn, seekerSpawn;
    bool isSpawned;

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

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkManager.seekerSpawn = seekerSpawn;
    }
    public IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(.1f);
        InitPlayerSpawns();
    }
    [Server]
    void InitPlayerSpawns()
    {
        foreach (var player in NetworkManager.gamePlayer)
        {
            var spawn = player.isSeeker ? seekerSpawn : NetworkManager.GetStartPosition();

            if (player.connectionToClient != null)
            {
                player.GetComponent<PlayerSpawn>().TargetSetSpawn(player.connectionToClient, spawn);
            }
        }
    }

}

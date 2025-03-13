using UnityEngine;
using Mirror;
using System.Collections;
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkManager.seekerSpawn = seekerSpawn;
    }
    

}

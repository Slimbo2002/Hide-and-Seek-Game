using UnityEngine;
using Mirror;
using System.Collections;
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Transform hiderSpawn, seekerSpawn, caughtPos;

    public float gameTime;
    public float seekerTime;
    bool seekerRelease = false;

    float timer = 0f;
    bool timerOn = false;

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

        GameStart();
    }
    private void Update()
    {
        if (timerOn)
        {
            timer += Time.deltaTime;
        }

        if (timer >= seekerTime && !seekerRelease)
        {
            ReleaseSeeker();
            seekerRelease = true;
        }

        if(timer >= gameTime)
        {
            NetworkManager.EndGame();
            NetworkManager.ServerChangeScene("OutdoorsScene");
        }

        if(timer >= gameTime - 60f)
        {
            for (int i = 0; i < NetworkManager.gamePlayer.Count; i++)
            {
                networkManager.gamePlayer[i].RPCAnnounce("1 Minute Remaining");
            }
        }
    }

    void GameStart() 
    {
        StartCoroutine(SetPos());

        timerOn = true;

        

        
    }

    IEnumerator SetPos()
    {
        yield return new WaitForSeconds(.1f);

        for (int i = 0; i < NetworkManager.gamePlayer.Count; i++)
        {
            if (NetworkManager.gamePlayer[i].isSeeker)
            {
                Rigidbody rb = NetworkManager.gamePlayer[i].GetComponent<Rigidbody>();
                rb.MovePosition(seekerSpawn.position);
            }
        }
    }
    void ReleaseSeeker()
    {
        for (int i = 0; i < NetworkManager.gamePlayer.Count; i++)
        {
            if (NetworkManager.gamePlayer[i].isSeeker)
            {
                NetworkManager.gamePlayer[i].GetComponent<Rigidbody>().isKinematic = false;
                Rigidbody rb = NetworkManager.gamePlayer[i].GetComponent<Rigidbody>();
                rb.MovePosition(NetworkManager.GetStartPosition().position);
                
            }
            networkManager.gamePlayer[i].GetComponent<SeekerBehaviour>().enabled = true;
            networkManager.gamePlayer[i].RPCAnnounce("Seeker is Coming!");
        }
    }
    

}

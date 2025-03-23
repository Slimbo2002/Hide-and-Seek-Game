using Mirror;
using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{
    public LobbyController controller;

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
    public void StartInteract()
    {
        UIManager.Instance.ShowInteract(true);
    }
    public void StopInteract()
    {
        UIManager.Instance.ShowInteract(false);
    }
    public void Interact()
    {
        if (NetworkManager.gamePlayer.Count > 1)
        {
            controller.StartGame("HouseMap");
        }
        else
        {
            NetworkManager.gamePlayer[0].RPCAnnounce("Not enough players");

        }
    }
}

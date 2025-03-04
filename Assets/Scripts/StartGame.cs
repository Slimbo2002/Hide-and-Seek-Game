using Mirror;
using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{
    public LobbyController controller;
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
        controller.StartGame("HouseMap");
    }
}

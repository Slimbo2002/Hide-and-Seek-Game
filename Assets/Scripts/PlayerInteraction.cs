using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerInteraction : NetworkBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 2f;

    private IInteractable currentInteractable;
    IInteractable lastInteractable;
    void Update()
    {
        if (!isOwned) return; // Ensure only the local player runs this


        
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (lastInteractable != interactable) // If we are looking at a new interactable
                {
                    lastInteractable?.StopInteract(); // Stop interacting with the previous one
                    lastInteractable = interactable; // Update lastInteractable
                    interactable.StartInteract(); // Start interaction with the new object
                }

                if (UserInputs.inputREF.interactInput)
                {
                    interactable.Interact();
                }
            }

            // 🌟 Interact with UI Button
            if (UserInputs.inputREF.interactInput)
            {
                if (hit.collider.TryGetComponent<Button>(out Button button))
                {
                    button.onClick.Invoke(); // Simulate button click
                    return; // Stop further interaction if UI was clicked
                }
            }
        }
        else
        {
            if (lastInteractable != null)
            {
                lastInteractable.StopInteract(); // Stop interacting if we're not looking at anything interactable
                lastInteractable = null; // Reset lastInteractable
            }
        }
    }

}


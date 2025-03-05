using UnityEngine;
using Mirror;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform orientation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation; // Handles looking up and down
    float yRotation; // Handles looking left and right

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable or disable the camera for the local player
        if (isOwned)
        {
            cam.gameObject.SetActive(true);
        }
        else
        {
            cam.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Ensure only the local player can control their view
        if (!isOwned) return;

        // Get mouse input
        mouseX = GetLookInput().x;
        mouseY = GetLookInput().y;

        // Adjust rotation values
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        // Clamp xRotation to prevent over-rotation (looking beyond straight up or down)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0); // Rotate the camera vertically
        if (NetworkClient.ready)
        {
            CMDLook(yRotation);
        }
        
    }
    [Command]
    void CMDLook(float newYRotation)
    {
        // Update the player's rotation on the server
        yRotation = newYRotation;

        // Tell all clients to update this player's rotation
        RPCLook(yRotation);
    }

    [ClientRpc]
    void RPCLook(float newYRotation)
    {
        // Apply rotation on all clients
        orientation.rotation = Quaternion.Euler(0, newYRotation, 0);
    }

    // Get mouse input from your input system
    Vector2 GetLookInput()
    {
        return UserInputs.inputREF.lookInput;
    }
}

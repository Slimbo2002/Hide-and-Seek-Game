using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseMenu;

    PlayerLook looking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        looking = GetComponent<PlayerLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInputs.inputREF.pauseInput)
        {
            isPaused = !isPaused;  
            looking.enabled = !isPaused;
        }

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        pauseMenu.SetActive(isPaused);
    }
}

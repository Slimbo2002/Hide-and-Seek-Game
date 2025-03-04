using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public TextMeshProUGUI roleText;
    public TextMeshProUGUI interactPrompt;

    public void ShowRole(string message)
    {
        roleText.text = message;
        roleText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideAnnouncement));
        Invoke(nameof(HideAnnouncement), 3f);
    }
    void HideAnnouncement()
    {
        roleText.gameObject.SetActive(false);
    }

    public void ShowInteract(bool interact)
    {
        interactPrompt.gameObject.SetActive(interact);
    }
}

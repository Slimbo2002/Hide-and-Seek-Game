using UnityEngine;
using TMPro;
using Steamworks;
using UnityEngine.UI;
public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;
    bool avatarReceived;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;

    protected Callback<AvatarImageLoaded_t> imageLoaded;
    private void Start()
    {
        imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded); 
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if(ImageID == -1)
        {
            return;
        }

        playerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    public void SetPlayerValues()
    {
        playerNameText.text = playerName;
        if (!avatarReceived)
        {
            GetPlayerIcon();
        }
    }
        

    void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerSteamID)
        {
            playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarReceived = true;
        return texture;
    }
}

using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Mirror;

public class VivoxPlayer : MonoBehaviour
{
    public static VivoxPlayer Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(Instance);
    }
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await VivoxService.Instance.InitializeAsync();

        await VivoxService.Instance.LoginAsync();
    }
    public async void JoinChannel(string channelName)
    {
        await VivoxService.Instance.JoinEchoChannelAsync(channelName, ChatCapability.TextAndAudio);

    }
    public async void LeaveChannel(string channelName)
    {
        await VivoxService.Instance.LeaveChannelAsync(channelName);
    }
    async void OnApplicationQuit()
    {
        await VivoxService.Instance.LogoutAsync();
    }
}

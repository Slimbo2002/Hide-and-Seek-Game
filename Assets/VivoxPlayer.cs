using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Mirror;
using System.Collections.Generic;
using UnityEngine.Android;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using HeathenEngineering;

public class VivoxPlayer : MonoBehaviour
{
    int PermissionCount = 0;
    public string vcName = "GameVC";

    float _nextUpdate = 0;
    private bool connected = false;
    Transform camera;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;

        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            camera = GameObject.Find("ExampleCamera").transform;
        }
        
    }
    async void InitializeAsync()
    {
        Debug.Log("Vivox InitializeAsync called");
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();
    }

    // ===============================================
    // Android approval helpers to use the microphone
    // ===============================================
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        bool IsAndroid12AndUp()
        {
            // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
            const int android12VersionCode = 31;
            AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
            int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

            return buildSdkVersion >= android12VersionCode;
        }

        string GetBluetoothConnectPermissionCode()
        {
            if (IsAndroid12AndUp())
            {
                // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
                AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
                string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

                return permissionCode;
            }

            return "";
        }
#endif


    bool IsMicPermissionGranted()
    {
        bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
            if (IsAndroid12AndUp())
            {
                // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
                isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
            }
#endif
        return isGranted;
    }

    void AskForPermissions()
    {
        string permissionCode = Permission.Microphone;

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
            if (m_PermissionAskedCount == 1 && IsAndroid12AndUp())
            {
                permissionCode = GetBluetoothConnectPermissionCode();
            }
#endif
        PermissionCount++;
        Permission.RequestUserPermission(permissionCode);
    }

    bool IsPermissionsDenied()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
            if (IsAndroid12AndUp())
            {
                return PermissionCount == 2;
            }
#endif
        return PermissionCount == 1;
    }
    public void SignIntoVivox(string name)     //in the new API this is now an Async function
    {
        //Actual code runs from here
        if (IsMicPermissionGranted())
            LoginToVivox(name);

        else
        {
            if (IsPermissionsDenied())
            {
                PermissionCount = 0;
                LoginToVivox(name);
            }
            else
            {
                AskForPermissions();
                LoginToVivox(name);
            }
        }
    }


    async void LoginToVivox(string name)
    {
        await VivoxVoiceManager.Instance.InitializeAsync(transform.name.ToString());
        var loginOptions = new LoginOptions()
        {
            ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond,
            DisplayName = name,
            EnableTTS = false
        };
        await VivoxService.Instance.LoginAsync(loginOptions);
        //await VivoxService.Instance.JoinEchoChannelAsync(vcName, ChatCapability.AudioOnly);
        await VivoxService.Instance.JoinGroupChannelAsync(vcName, ChatCapability.AudioOnly);
        //await VivoxService.Instance.JoinPositionalChannelAsync(vcName, ChatCapability.AudioOnly,
        //        new Channel3DProperties(5, 1, 1f, AudioFadeModel.LinearByDistance));
    }
    void OnUserLoggedIn()
    {
        Debug.Log("Logged into Vivox");
        connected = true;
    }

    void OnUserLoggedOut()
    {
        Debug.Log("Logged out from Vivox");
        connected = false;
    }
    void Update()
    {
        if (!connected) return;

        if (Time.time > _nextUpdate)
        {
            //VivoxService.Instance.Set3DPosition(camera.gameObject, vcName); //new in API>1.6.0
            _nextUpdate += 0.5f;
        }
    }
}

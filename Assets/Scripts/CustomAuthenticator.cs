using Mirror;
using UnityEngine;

public struct PlayerData
{
    public bool isSeeker;
}
public class CustomAuthenticator : NetworkAuthenticator
{
    public struct AuthRequestMessage : NetworkMessage
    {
        public bool isSeeker;
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    public override void OnClientAuthenticate()
    {
        // Send authentication request with seeker role
        AuthRequestMessage authMessage = new AuthRequestMessage
        {
            isSeeker = false // Example: Default to not a seeker
        };

        NetworkClient.Send(authMessage);
    }

    private void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        Debug.Log($"[Server] Auth request received. Is Seeker: {msg.isSeeker}");

        // Store in authenticationData
        conn.authenticationData = new PlayerData
        {
            isSeeker = msg.isSeeker
        };

        ServerAccept(conn);
    }
}

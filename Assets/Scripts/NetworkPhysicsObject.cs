using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbodyReliable))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(NetworkIdentity))]
public class NetworkPhysicsObject : NetworkBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Command(requiresAuthority = false)]  // Allows any client to call this
    public void CMDApplyForce(Vector3 force)
    {
        if (!isServer) return; // Ensure only the server applies the force

        rb.linearVelocity = -force / rb.mass;
        Debug.Log("Force applied: " + force);
    }
}

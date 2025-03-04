using UnityEngine;
using Mirror;

public class PlayerSpawn : NetworkBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    [TargetRpc]
    public void TargetSetSpawn(NetworkConnection conn, Transform spawnPosition)
    {

        rb.linearVelocity = Vector3.zero; // Reset any movement forces
        rb.angularVelocity = Vector3.zero; // Reset rotation forces
        rb.isKinematic = true; // Fully disable physics
        

        transform.position = spawnPosition.position; // Manually set position
        rb.isKinematic = false; // Re-enable physics
    }


}

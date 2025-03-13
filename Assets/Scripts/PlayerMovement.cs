using Mirror;
using Mirror.BouncyCastle.Asn1.Cmp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody rb;
    public float speed = 5f;
    public float jumpForce = 5f;
    int maxJumps = 1;
    int jumps = 0;

    [SerializeField] private Transform cameraTransform; // Reference to the camera's transform


    // Ground check
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    private bool groundedPlayer;
    private Vector3 moveDirection;

    public GameObject cube;

    bool spawned;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isOwned || gameObject == null) return;

        Move();
    }

    void Move()
    {
        // Ground check
        groundedPlayer = Physics.CheckSphere(transform.position, groundDistance, groundMask);

        // Get movement input
        float x = GetWASDInput().x;
        float y = GetWASDInput().y;

        // Calculate movement direction relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore vertical direction for movement
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * y + right * x;

        // Jump input
        if (UserInputs.inputREF.jumpInput && NetworkClient.ready)
        {
            if(jumps < maxJumps)
            {
                CMDJump();
                jumps++;
            }
            
        }
        if (groundedPlayer)
        {
            jumps = 0;
        }

    }

    void FixedUpdate()
    {
        if (!isOwned) return;

        if (NetworkClient.ready)
        {
            CmdMove(moveDirection);
        }
        
    }

    [Command]
    void CMDJump()
    {
        RpcJump(); 
    }
    [ClientRpc]
    void RpcJump()
    {
        rb.AddForce(Vector3.up * 9f, ForceMode.Impulse); // Apply force on all clients
    }
    [Command]
    void CmdMove(Vector3 moveDirection)
    {
        Vector3 moveVelocity = moveDirection * speed;
        Vector3 currentVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
        RPCMove(currentVelocity);
    }
    [ClientRpc]
    void RPCMove(Vector3 currentVelocity)
    {
        rb.linearVelocity = currentVelocity;
    }
    Vector2 GetWASDInput()
    {
        return UserInputs.inputREF.moveInput;
    }
}

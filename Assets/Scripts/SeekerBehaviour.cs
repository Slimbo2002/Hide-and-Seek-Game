using UnityEngine;

public class SeekerBehaviour : MonoBehaviour
{
    public Camera cam;

    // Update is called once per frame
    private void Start()
    {
        GetComponent<PlayerMovement>().speed = 10f;
    }
    void Update()
    {
        if (UserInputs.inputREF.attackInput)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (hit.collider.tag == "Player")
                {
                    hit.collider.gameObject.GetComponent<Rigidbody>().MovePosition(GameManager.instance.caughtPos.position);
                }
            }
        }
    }
}

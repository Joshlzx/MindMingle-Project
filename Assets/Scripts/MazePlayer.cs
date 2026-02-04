using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))  // left
            move += Vector3.left;
        if (Input.GetKey(KeyCode.X))  // up
            move += Vector3.forward;
        if (Input.GetKey(KeyCode.C))  // down
            move += Vector3.back;
        if (Input.GetKey(KeyCode.V))  // right
            move += Vector3.right;

        move = move.normalized;  // ensures diagonal movement isn't faster
        Vector3 newPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
}
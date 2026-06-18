using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D body;
    public float moveSpeed;

    public InputActionReference move;
    private float moveDirection;
    
    void Update()
    {
        moveDirection = move.action.ReadValue<float>();
    }

    void FixedUpdate()
    {
        body.linearVelocity = Vector3.right * moveDirection * moveSpeed;
    }
}

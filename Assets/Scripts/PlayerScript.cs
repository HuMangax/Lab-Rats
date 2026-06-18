using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D body;
    public float moveSpeed;
    public float jumpHeight;

    public InputActionReference move;
    private float moveDirection;

    public InputActionReference jump;
    private bool jumpReady = false;
    
    void Update()
    {
        moveDirection = move.action.ReadValue<float>();
    }

    void FixedUpdate()
    {
        body.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            body.linearVelocity.y
        );

        if (jump.action.IsPressed() && jumpReady)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpHeight
            );

            jumpReady = false;
        } 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.9f) jumpReady = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        jumpReady = false;
    }
}

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
    private bool grounded = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 startPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    // Send the player back to where it started (used by spikes / hazards).
    public void Respawn()
    {
        transform.position = startPosition;
        body.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        moveDirection = move.action.ReadValue<float>();

        // The rat is drawn facing right; flip it to face the way it travels.
        if (moveDirection > 0.01f) spriteRenderer.flipX = false;
        else if (moveDirection < -0.01f) spriteRenderer.flipX = true;

        // Drive the animation state machine (Idle / Walk / Jump).
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveDirection));
            animator.SetBool("Grounded", grounded);
        }
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
            if (contact.normal.y > 0.9f)
            {
                jumpReady = true;
                grounded = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        jumpReady = false;
        grounded = false;
    }
}

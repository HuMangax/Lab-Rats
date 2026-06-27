using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    public float bounceHeight;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D body = other.attachedRigidbody;

        if (body)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                bounceHeight
            );
        }
    }
}

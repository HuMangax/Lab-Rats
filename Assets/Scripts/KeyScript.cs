using UnityEngine;

public class KeyScript : Activator
{
    public Rigidbody2D body;
    private Rigidbody2D player;
    private bool obtained = false;

    void FixedUpdate()
    {
        if (player)
        {
            body.transform.position = new Vector2(
                player.transform.position.x,
                player.transform.position.y + 1.5f
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            player = other.attachedRigidbody;
            return;
        }

        Unlockable unlockable = other.GetComponent<Unlockable>();

        if (unlockable != null && unlockable.activators.Contains(this))
        {
            SetActive(true);
            Destroy(gameObject);
        }
    }
}

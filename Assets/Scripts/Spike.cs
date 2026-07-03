using UnityEngine;

// Hazard: sends the player back to its start position on contact.
public class Spike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerScript>();
        if (player != null) player.Respawn();
    }
}

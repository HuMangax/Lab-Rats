using UnityEngine;

public class ButtonScript : Activator
{
    public Rigidbody2D body;
    private float activationThreshold;

    void Start()
    {
        activationThreshold = body.transform.position.y - GetComponent<Renderer>().bounds.size.y;
    }

    void Update()
    {
        bool pressed = body.transform.position.y <= activationThreshold * 0.9;
        SetActive(pressed);
    }
}

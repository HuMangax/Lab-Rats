using UnityEngine;

public class ButtonScript : Activator
{
    public Rigidbody2D body;
    private float originalY;
    private float buttonSize;

    void Start()
    {
        originalY = body.transform.position.y;
        buttonSize = GetComponent<Renderer>().bounds.size.y;
    }

    void Update()
    {
        bool pressed = body.transform.position.y <= originalY - buttonSize * 0.9f;
        SetActive(pressed);
    }
}

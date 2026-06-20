using UnityEngine;

public class DoorScript : Unlockable
{
    public Rigidbody2D body;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Vector2 targetPosition;

    public float moveSpeed;

    protected override void Unlock()
    {
        targetPosition = openPosition;
    }

    protected override void Lock()
    {
        targetPosition = closedPosition;
    }

    private void Start()
    {
        Vector2 startPosition = body.transform.position;
        float doorHeight = GetComponent<Renderer>().bounds.size.y;

        closedPosition = new Vector2(startPosition.x, startPosition.y);
        openPosition = new Vector2(startPosition.x, startPosition.y + doorHeight);
        targetPosition = closedPosition;
    }

    private void FixedUpdate()
    {
        body.MovePosition(Vector2.Lerp(
            body.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        ));
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    
    // We use an Enum to track the 4 possible directions
    private enum Direction { None, Up, Down, Left, Right }
    private Direction lastDirection = Direction.None;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Get Raw Input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // 2. Update the 'Last Direction' based on the newest button press
        // Keyboard Support
        if (Input.GetKeyDown(KeyCode.W)) lastDirection = Direction.Up;
        if (Input.GetKeyDown(KeyCode.S)) lastDirection = Direction.Down;
        if (Input.GetKeyDown(KeyCode.A)) lastDirection = Direction.Left;
        if (Input.GetKeyDown(KeyCode.D)) lastDirection = Direction.Right;

        // 3. Controller 'Closest Input' logic (Snap to strongest axis)
        // If the stick is moved significantly, we treat it as a new direction press
        if (Mathf.Abs(inputX) > 0.8f && Mathf.Abs(inputX) > Mathf.Abs(inputY))
            lastDirection = inputX > 0 ? Direction.Right : Direction.Left;
        else if (Mathf.Abs(inputY) > 0.8f && Mathf.Abs(inputY) > Mathf.Abs(inputX))
            lastDirection = inputY > 0 ? Direction.Up : Direction.Down;

        // 4. Verification: If the 'Last Direction' key is no longer held, 
        // fallback to whatever else is currently being pressed.
        Vector2 moveVec = GetDirectionVector(lastDirection);
        
        // If the last pressed key isn't actually being held anymore, find a new one
        if (!IsDirectionHeld(lastDirection, inputX, inputY))
        {
            if (inputY > 0) lastDirection = Direction.Up;
            else if (inputY < 0) lastDirection = Direction.Down;
            else if (inputX > 0) lastDirection = Direction.Right;
            else if (inputX < 0) lastDirection = Direction.Left;
            else lastDirection = Direction.None;
        }

        rb.linearVelocity = GetDirectionVector(lastDirection) * moveSpeed;
    }

    // Helper: Converts our Direction choice into a Vector2
    Vector2 GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Vector2.up;
            case Direction.Down: return Vector2.down;
            case Direction.Left: return Vector2.left;
            case Direction.Right: return Vector2.right;
            default: return Vector2.zero;
        }
    }

    // Helper: Checks if the specific direction is still being "pushed"
    bool IsDirectionHeld(Direction dir, float x, float y)
    {
        switch (dir)
        {
            case Direction.Up: return Input.GetKey(KeyCode.W) || y > 0.1f;
            case Direction.Down: return Input.GetKey(KeyCode.S) || y < -0.1f;
            case Direction.Left: return Input.GetKey(KeyCode.A) || x < -0.1f;
            case Direction.Right: return Input.GetKey(KeyCode.D) || x > 0.1f;
            default: return false;
        }
    }
}
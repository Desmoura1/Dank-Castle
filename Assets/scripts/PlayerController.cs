using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    
    // We track the last pressed direction for each axis separately
    private float lastXDir = 0;
    private float lastYDir = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Detect NEW key presses for Horizontal
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastXDir = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastXDir = 1;

        // 2. Detect NEW key presses for Vertical
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastYDir = 1;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastYDir = -1;

        // 3. Controller / Joystick Logic
        // If the stick is pushed far enough, it overrides the 'lastDir'
        float stickX = Input.GetAxisRaw("Horizontal");
        float stickY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(stickX) > 0.1f) lastXDir = stickX > 0 ? 1 : -1;
        if (Mathf.Abs(stickY) > 0.1f) lastYDir = stickY > 0 ? 1 : -1;

        // 4. Verification: If a key is released, and the OTHER key on that axis is still held, 
        // switch back to it. If nothing is held, set that axis to 0.
        float finalX = GetActiveAxisInput(lastXDir, "Horizontal", KeyCode.D, KeyCode.A);
        float finalY = GetActiveAxisInput(lastYDir, "Vertical", KeyCode.W, KeyCode.S);

        // 5. Combine for Diagonal Movement
        Vector2 moveDirection = new Vector2(finalX, finalY);

        // Normalize prevents "Diagonal Speed Boost" (going faster when moving diagonally)
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // Helper: Checks if the 'last' pressed direction is still valid, or falls back to the opposite
    float GetActiveAxisInput(float lastDir, string axisName, KeyCode positiveKey, KeyCode negativeKey)
    {
        float raw = Input.GetAxisRaw(axisName);

        // If we are holding the 'last' direction, keep it
        if ((lastDir > 0 && Input.GetKey(positiveKey)) || (lastDir < 0 && Input.GetKey(negativeKey)))
        {
            return lastDir;
        }
        // If we released the last direction but are holding the opposite one
        if (raw != 0) return raw;

        return 0;
    }
}
using UnityEngine;
using System.Collections;

public class SolidSimpleDash : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashDuration = 0.15f;
    public float cooldown = 0.5f;
    public LayerMask wallLayer; // MUST BE SET IN INSPECTOR
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isDashing = false;
    private float nextDashTime;
    private Vector2 lastMoveDir = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        
        // Settings for smooth collision
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        if (isDashing) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(x, y).normalized;
        
        if (input != Vector2.zero) lastMoveDir = input;

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton0)) && Time.time > nextDashTime)
        {
            StartCoroutine(PerformDash(input == Vector2.zero ? lastMoveDir : input));
        }

        rb.linearVelocity = input * moveSpeed;
    }

    IEnumerator PerformDash(Vector2 dir)
    {
        isDashing = true;
        nextDashTime = Time.time + cooldown;
        
        Vector2 startPos = rb.position;

        // 1. BOXCAST: Checks the player's full body size along the dash path
        // We use the size of your actual BoxCollider2D
        RaycastHit2D hit = Physics2D.BoxCast(startPos, boxCol.size, 0f, dir, dashDistance, wallLayer);
        
        Vector2 targetPos;
        if (hit.collider != null)
        {
            // If we hit a wall, stop exactly at the hit point minus a tiny buffer
            targetPos = hit.centroid; 
        }
        else
        {
            targetPos = startPos + (dir * dashDistance);
        }

        float elapsed = 0;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / dashDuration;
            float curvePercent = easeCurve.Evaluate(percent);
            
            // 2. MovePosition: Respects physics and prevents phasing
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, curvePercent));
            
            yield return null;
        }

        rb.MovePosition(targetPos);
        isDashing = false;
    }
}
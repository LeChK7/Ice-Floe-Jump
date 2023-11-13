using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 20.0f;
    public Transform[] groundPoints;  // Points from which to raycast for ground check
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;  // Radius for overlap circle to check for ground
    public float attackDuration = 0.2f;  // Duration for the attack animation

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float attackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Lock the rotation on the Z-axis
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        isGrounded = IsGrounded();
        HandleMovement();
        HandleJumping();
        HandleAttacking();

        // Update the Animator states
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    void FixedUpdate()
    {
        // Fixed update for physics-based checks
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
            animator.SetBool("isAttacking", attackTimer > 0);
        }
    }

    void HandleMovement()
    {
    float move = Input.GetAxis("Horizontal");
    rb.velocity = new Vector2(move * speed, rb.velocity.y);

    // Check if movement is to the left (negative) or to the right (positive)
    if (move > 0.01f)
    {
        // Moving right - ensure the local scale is positive
        transform.localScale = new Vector3(1, 1, 1);
    }
    else if (move < -0.01f)
    {
        // Moving left - flip the sprite by making the local scale negative
        transform.localScale = new Vector3(-1, 1, 1);
    }
    
    // Update the animator with whether the player is walking
    animator.SetBool("isWalking", Mathf.Abs(move) > 0.01f);
    }


    void HandleJumping()
    {
        // Only allow jumping if grounded and the jump button is pressed
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Add a vertical force to the player.
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // Immediately set isGrounded to false to prevent multiple jumps
            isGrounded = false;
        }
    }

    void HandleAttacking()
    {
        if (Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            attackTimer = attackDuration;
            // The attack animation is handled by the Animator based on the isAttacking parameter
        }
    }

    private bool IsGrounded()
    {
        foreach (Transform point in groundPoints)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundCheckRadius, groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)  // Make sure we're not detecting the player's own collider
                {
                    return true;  // Ground is found
                }
            }
        }
        return false;  // No ground detected
    }
}

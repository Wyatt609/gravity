using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;

    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float gravityStrength = 25f;   // MUCH stronger gravity for fast fall
    public float fallMultiplier = 2.2f;   // extra gravity when falling
    public float groundCheckRadius = 0.3f;

    [Header("Grounding")]
    public LayerMask groundMask;
    public Transform groundCheck;

    private Vector3 velocity;
    private bool isGrounded;
    private bool jumpPressed;
    private bool wasGroundedLastFrame;

    [Header("Gravity Reorientation")]
    public float reorientSpeed = 10f;

    void Update()
    {
        Vector3 gravityDir = GravityManager.Instance ? GravityManager.Instance.GravityDir : Vector3.down;
        Vector3 gravity = gravityDir * gravityStrength;

        // Ground check using Physics.CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // Reset downward velocity on landing
        if (isGrounded && Vector3.Dot(velocity, gravityDir) > 0f)
            velocity -= gravityDir * Vector3.Dot(velocity, gravityDir);

        // --- Movement ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z);
        controller.Move(move * speed * Time.deltaTime);

        // --- Jump ---
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        if (jumpPressed && isGrounded)
        {
            float jumpSpeed = Mathf.Sqrt(2f * gravityStrength * jumpHeight);
            velocity = -gravityDir * jumpSpeed;
            jumpPressed = false;
            isGrounded = false;
        }

        // --- Gravity ---
        float alignment = Vector3.Dot(velocity.normalized, gravityDir);
        bool falling = alignment > 0.3f;
        velocity += gravity * (falling ? fallMultiplier : 1f) * Time.deltaTime;

        // --- Move ---
        controller.Move(velocity * Time.deltaTime);

        // --- Rotate player with gravity ---
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, -gravityDir) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, reorientSpeed * Time.deltaTime);

        wasGroundedLastFrame = isGrounded;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

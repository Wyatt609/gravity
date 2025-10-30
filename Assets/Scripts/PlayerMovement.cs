using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float jumpHeight = 3f;
    public float gravityStrength = 25f;
    public float fallMultiplier = 2.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float reorientSpeed = 10f;
    public float inputDeadzone = 0.05f;
    public float platformProbeDistance = 3.5f;
    public float carryCoyoteTime = 0.2f;

    Vector3 velocity;
    bool isGrounded;
    bool jumpPressed;

    Transform currentPlatform;
    Vector3 lastPlatformPos;
    float carryTimer;

    void OnEnable()
    {
        if (GravityManager.Instance != null) GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    void OnDisable()
    {
        if (GravityManager.Instance != null) GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
    }

    void OnGravityChanged(Vector3 newDown)
    {
        Vector3 g = newDown.normalized;
        velocity = Vector3.Project(velocity, g);
        currentPlatform = null;
        carryTimer = 0f;
        AntiStuckNudge(g, 0.35f);
        SnapToSurface(g, 0.6f);
    }

    void Update()
    {
        Vector3 gravityDir = GravityManager.Instance ? GravityManager.Instance.GravityDir : Vector3.down;
        Vector3 gravity = gravityDir * gravityStrength;

        if (currentPlatform != null)
        {
            Vector3 delta = currentPlatform.position - lastPlatformPos;
            if (delta.sqrMagnitude > 0f) controller.Move(delta);
            lastPlatformPos = currentPlatform.position;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask) || controller.isGrounded;

        if (isGrounded && Vector3.Dot(velocity, gravityDir) > 0f)
            velocity -= gravityDir * Vector3.Dot(velocity, gravityDir);

        float xRaw = Input.GetAxisRaw("Horizontal");
        float zRaw = Input.GetAxisRaw("Vertical");
        Vector2 raw = new Vector2(xRaw, zRaw);
        if (raw.magnitude < inputDeadzone) raw = Vector2.zero;

        if (raw != Vector2.zero)
        {
            Vector3 move = transform.right * raw.x + transform.forward * raw.y;
            move = Vector3.ProjectOnPlane(move, -gravityDir).normalized;
            controller.Move(move * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump")) jumpPressed = true;

        if (jumpPressed && isGrounded)
        {
            float jumpSpeed = Mathf.Sqrt(2f * gravityStrength * jumpHeight);
            velocity = -gravityDir * jumpSpeed;
            jumpPressed = false;
            isGrounded = false;
            currentPlatform = null;
            carryTimer = 0f;
        }

        float alignment = velocity.sqrMagnitude > 0f ? Vector3.Dot(velocity.normalized, gravityDir) : 0f;
        bool falling = alignment > 0.3f;
        velocity += gravity * (falling ? fallMultiplier : 1f) * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded) SnapToSurface(gravityDir, 0.15f);

        Quaternion targetRot = Quaternion.FromToRotation(transform.up, -gravityDir) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, reorientSpeed * Time.deltaTime);

        ProbePlatform();
    }

    void ProbePlatform()
    {
        Vector3 down = -transform.up;
        float radius = Mathf.Max(0.1f, controller.radius * 0.95f);
        Vector3 origin = groundCheck != null ? groundCheck.position : transform.position;

        bool latched = false;
        if (Physics.SphereCast(origin, radius, down, out RaycastHit hit, platformProbeDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            var mp = hit.collider.GetComponentInParent<MovingPlatform>();
            if (mp != null)
            {
                if (currentPlatform != mp.transform)
                {
                    currentPlatform = mp.transform;
                    lastPlatformPos = currentPlatform.position;
                }
                latched = true;
            }
        }

        if (latched)
        {
            carryTimer = carryCoyoteTime;
        }
        else
        {
            if (carryTimer > 0f) carryTimer -= Time.deltaTime;
            else currentPlatform = null;
        }
    }

    void AntiStuckNudge(Vector3 gravityDir, float dist)
    {
        controller.enabled = false;
        transform.position += gravityDir * dist;
        controller.enabled = true;
    }

    void SnapToSurface(Vector3 gravityDir, float maxDistance)
    {
        Vector3 up = -gravityDir;
        float skin = 0.02f;
        float footOffset = controller.height * 0.5f - controller.radius + skin;
        Vector3 origin = transform.position + up * footOffset;
        if (Physics.SphereCast(origin, controller.radius * 0.95f, gravityDir, out RaycastHit hit, maxDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 targetCenter = hit.point - up * footOffset;
            controller.enabled = false;
            transform.position = targetCenter;
            controller.enabled = true;
        }
        else
        {
            controller.Move(-gravityDir * 0.005f);
        }
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


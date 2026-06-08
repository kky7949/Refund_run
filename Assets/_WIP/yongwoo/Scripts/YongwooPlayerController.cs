using UnityEngine;
using UnityEngine.InputSystem;

public class YongwooPlayerController : MonoBehaviour
{
    [Header("이동")]
    public float Speed = 5f;
    public float JumpForce = 7f;
    public float lowJumpMultiplier = 0.5f;

    [Header("리스폰")]
    public float respawnTime = 1.5f;
    public float fallDeathY = -5f;

    private Rigidbody body;
    private Animator animator;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isGrounded = true;
    private bool isDead;
    private float deadTimer;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        // 죽은 뒤에는 잠깐 날아가는 모션을 보여주고 시작 위치로 돌린다.
        if (isDead)
        {
            deadTimer += Time.deltaTime;
            if (deadTimer >= respawnTime)
            {
                Respawn();
            }

            return;
        }

        if (transform.position.y < fallDeathY)
        {
            DeathTrigger();
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        float move = 0f;
        if (Keyboard.current.aKey.isPressed)
        {
            move = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            move = 1f;
        }

        Move(move);
        Jump();
    }

    private void Move(float move)
    {
        if (move != 0f)
        {
            float yRotation = move > 0f ? 90f : -90f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            SetAnimatorBool("isMoving", true);
        }
        else
        {
            SetAnimatorBool("isMoving", false);
        }

        SetAnimatorBool("isGrounded", isGrounded);
        transform.Translate(Vector3.forward * Mathf.Abs(move) * Speed * Time.deltaTime);
    }

    private void Jump()
    {
        if (body == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            body.linearVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            body.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && body.linearVelocity.y > 0f)
        {
            body.linearVelocity = new Vector3(body.linearVelocity.x, body.linearVelocity.y * lowJumpMultiplier, body.linearVelocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            DeathTrigger();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DeathTrigger();
        }
    }

    public void DeathTrigger()
    {
        if (isDead || body == null)
        {
            return;
        }

        isDead = true;
        deadTimer = 0f;

        if (animator != null)
        {
            animator.enabled = false;
        }

        body.constraints = RigidbodyConstraints.None;

        // 장애물에 닿으면 뒤로 튕겨 나가는 느낌만 단순하게 준다.
        Vector3 flyDirection = new Vector3(Random.Range(-5f, 5f), Random.Range(10f, 15f), Random.Range(-5f, -10f));
        body.AddForce(flyDirection, ForceMode.Impulse);
        body.AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)), ForceMode.Impulse);
    }

    private void Respawn()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
        }

        isGrounded = true;
        isDead = false;
        ResetTraps();
    }

    private void ResetTraps()
    {
        foreach (var trap in FindObjectsOfType<YongwooMovingTrap>())
        {
            trap.ResetTrap();
        }

        foreach (var trap in FindObjectsOfType<YongwooDepthTrafficObstacle>())
        {
            trap.ResetTrap();
        }
    }

    private void SetAnimatorBool(string key, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(key, value);
        }
    }
}

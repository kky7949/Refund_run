using UnityEngine;
using UnityEngine.InputSystem;


public class NewMoveCS : MonoBehaviour
{
    public float Speed = 8.0f;
    public float JumpForce = 12.0f;  //변수 선언
    public float lowJumpMultiplier = 0.5f;

    [Header("오디오 설정")]
    public AudioSource playerAudio;
    public AudioClip jumpSound;

   [Header("장애물 충돌 소리")]
    public AudioClip[] obstacleCollisionSounds;
    
    private Rigidbody rb;   //물리엔진을 담당하는 컴포넌트
    private Animator anim;   //애니메이션을 담당하는 컴포넌트
    private bool isGrounded = true;    //캐릭터가 땅에 닿아있는지 확인하는 기억변수

    private Vector3 startPosition;
    private bool isDead = false;

    private float deadTimer = 0.0f;
    private float respawnTime = 1.5f;

    public bool isPlayingPuzzle = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        if (playerAudio != null) playerAudio = GetComponent<AudioSource>();

    }


    void Update()
    {
        if (isDead){
            deadTimer += Time.deltaTime;
            if (deadTimer >= respawnTime){
                Respawn();
            }
            return;
        }

        if (transform.position.y < -5f)
        {
            DeathTrigger();
        }

        if (isPlayingPuzzle)
        {
            anim.SetBool("isMoving", false);
            return;
        }

        if (Keyboard.current == null) return;

        float horizontalInput = 0f;
        if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
        if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;

        if (horizontalInput != 0f)
        {
            float yRotation = horizontalInput > 0 ? 90f : -90f;
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;

            if (playerAudio != null && jumpSound != null) playerAudio.PlayOneShot(jumpSound);
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * lowJumpMultiplier, rb.linearVelocity.z);
        }

        anim.SetBool("isGrounded", isGrounded);

        transform.Translate(Vector3.forward * Mathf.Abs(horizontalInput) * Speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)  //충동하는 순간 자동 호출되는 함수
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
        isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle") && !isDead)
        {
            DeathTrigger();
        }
    }

    void DeathTrigger()
    {
        isDead = true;
        deadTimer = 0.0f;

        anim.enabled = false;

        rb.constraints = RigidbodyConstraints.None;

        if (obstacleCollisionSounds != null && obstacleCollisionSounds.Length > 0)
        {
            // 무작위로 하나의 소리 선택
            int randomIndex = Random.Range(0, obstacleCollisionSounds.Length);
            
            // 플레이어 오디오 소스 및 선택된 클립이 존재하는지 확인 후 재생
            if (playerAudio != null && obstacleCollisionSounds[randomIndex] != null)
            {
                playerAudio.PlayOneShot(obstacleCollisionSounds[randomIndex]);
            }
        }

        Vector3 flyDirection = new Vector3(
            Random.Range(-5f, 5f),  // 좌우로 랜덤하게
            Random.Range(10f, 15f), // 위로 뽝!
            Random.Range(-5f, -10f) // 카메라 쪽(뒤)으로 튕겨 나옴!
        );

        rb.AddForce(flyDirection, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)), ForceMode.Impulse);

    }

    void Respawn()
    {
        transform.position = startPosition;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        transform.rotation = Quaternion.Euler(0, 90f, 0);

        anim.enabled = true;
        anim.Rebind();
        isDead = false;

        MovingTrap[] movingTraps = FindObjectsOfType<MovingTrap>();
        foreach (MovingTrap trap in movingTraps)
        {
            trap.ResetTrap();
        }

        FallingTrap[] fallingTraps = FindObjectsOfType<FallingTrap>();
        foreach (FallingTrap trap in fallingTraps)
        {
            trap.ResetTrap();
        }
    }
}

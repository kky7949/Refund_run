using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class NewMoveCS : MonoBehaviour
{
    public float Speed = 50.0f;
    public float RotateSpeed = 100.0f;
    public float JumpForce = 5.0f;  //변수 선언
    
    private Rigidbody rb;   //물리엔진을 담당하는 컴포넌트
    private Animator anim;   //애니메이션을 담당하는 컴포넌트
    private bool isGrounded = true;    //캐릭터가 땅에 닿아있는지 확인하는 기억변수

    private Vector3 startPosition;
    private bool isDead = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (isDead) return;

        Vector2 input = Vector2.zero;
        if (Keyboard.current != null) {
            if (Keyboard.current.aKey.isPressed) input.x = -1;
            if (Keyboard.current.dKey.isPressed) input.x = 1;
            if (Keyboard.current.wKey.isPressed) input.y = 1;
            if (Keyboard.current.sKey.isPressed) input.y = -1;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded) { // 스페이스바를 누른 순간에만 반응 || 스페이스바를 눌렀더라도 땅에 있을 때만 점프
                rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
        // 키보드 입력 스크립트
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        if (input.x !=0 || input.y !=0) {
            anim.SetBool("isMoving", true);
        }
        else {
            anim.SetBool("isMoving", false);
        }

        anim.SetBool("isGrounded", isGrounded);

        // 이동 거리 보정 스크립트
        float h = input.x * RotateSpeed * Time.deltaTime;
        float v = input.y * Speed * Time.deltaTime;

        // 실제 이동 스크립트
        transform.Rotate(Vector3.up * h);
        transform.Translate(Vector3.forward * v);
    }

    void OnCollisionEnter(Collision collision)  //충동하는 순간 자동 호출되는 함수
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
        isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle") && !isDead)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    IEnumerator DieAndRespawn()
    {
        isDead = true;

        anim.SetTrigger("die");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(1.5f);

        transform.position = startPosition;
        anim.Play("Idle", 0, 0f);
        isDead = false;
    }
}

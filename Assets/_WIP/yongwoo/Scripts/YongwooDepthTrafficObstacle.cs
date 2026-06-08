using UnityEngine;

public class YongwooDepthTrafficObstacle : MonoBehaviour
{
    [Header("움직임")]
    public Transform player;
    public float triggerDistance = 16f;
    public float moveSpeed = 7f;
    public Vector3 moveDirection = Vector3.back;
    public float travelDistance = 11f;
    public float startDelay = 0f;
    public float loopDelay = 1.2f;

    private Rigidbody body;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isStarted;
    private bool isMoving;
    private float waitTimer;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        startPosition = transform.position;
        targetPosition = startPosition + moveDirection.normalized * travelDistance;
        waitTimer = startDelay;
    }

    private void Update()
    {
        // 플레이어가 가까이 오기 전까지 차는 대기한다.
        if (!isStarted)
        {
            if (player == null || Vector3.Distance(transform.position, player.position) > triggerDistance)
            {
                return;
            }

            isStarted = true;
            isMoving = false;
            waitTimer = startDelay;
        }

        // 출발 딜레이가 있으면 잠깐 기다린다.
        if (!isMoving)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer > 0f)
            {
                return;
            }

            isMoving = true;
        }

        // 목표 지점까지 이동한다.
        MoveTo(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));

        // 끝까지 가면 처음 위치로 돌아가고 다시 반복한다.
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            MoveTo(startPosition);
            isMoving = false;
            waitTimer = loopDelay;
        }
    }

    public void ResetTrap()
    {
        MoveTo(startPosition);
        isStarted = false;
        isMoving = false;
        waitTimer = startDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 쪽 죽음 함수 이름만 맞춰두고, 장애물은 메시지만 보낸다.
        other.SendMessageUpwards("DeathTrigger", SendMessageOptions.DontRequireReceiver);
    }

    private void MoveTo(Vector3 position)
    {
        if (body != null && body.isKinematic)
        {
            body.MovePosition(position);
            return;
        }

        transform.position = position;
    }
}

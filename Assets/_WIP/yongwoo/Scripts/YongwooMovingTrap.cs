using UnityEngine;

public class YongwooMovingTrap : MonoBehaviour
{
    [Header("함정 설정")]
    public Transform player;
    public float triggerDistance = 15f;
    public float moveSpeed = 15f;
    public Vector3 moveDirection = new Vector3(-1f, 0f, 0f);

    private bool isTriggered;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // 플레이어가 가까이 오면 그때부터 한 방향으로 움직인다.
        if (!isTriggered && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= triggerDistance)
            {
                isTriggered = true;
            }
        }

        if (isTriggered)
        {
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void ResetTrap()
    {
        transform.position = startPosition;
        isTriggered = false;
    }
}

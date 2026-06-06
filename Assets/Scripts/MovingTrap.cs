using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("함정 설정")]
    public Transform player;
    public float triggerDistance = 15f;
    public float moveSpeed = 15f;
    public Vector3 moveDirection = new Vector3(-1, 0, 0);

    private bool isTriggered = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }


    void Update()
    {
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
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void ResetTrap()
    {
        transform.position = startPosition;
        isTriggered = false;
    }
}

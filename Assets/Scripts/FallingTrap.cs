using System.Collections;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    [Header("낙하 함정 설정")]
    public Transform player;
    public float triggerDistanceX = 5f;
    public float resetDelay = 2.0f;
    public float returnSpeed = 3.0f;
    public float extraFallForce = 50f;

    private Rigidbody rb;
    private bool isTriggered = false;
    private bool isReturning = false;
    private Vector3 startPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
        }
        startPosition = transform.position;
    }

    void Update()
    {
        if (isReturning)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * returnSpeed);

            if (transform.position == startPosition)
            {
                isReturning = false;
                isTriggered = false;
            }
            return;
        }

        if (!isTriggered && player != null && rb != null)
        {
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);
            
            if (distanceX <= triggerDistanceX)
            {
                isTriggered = true;
                rb.useGravity = true;

                StartCoroutine(AutoResetRoutine());
            }
        }
    }
    
    void FixedUpdate()
    {
        if (isTriggered && !isReturning && rb != null && rb.useGravity)
        {
            rb.AddForce(Vector3.down * extraFallForce, ForceMode.Acceleration);
        }
    }

    IEnumerator AutoResetRoutine()
    {
        yield return new WaitForSeconds(resetDelay);

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
        isReturning = true;
    }


    public void ResetTrap()
    {
        StopAllCoroutines();

        transform.position = startPosition;
        isTriggered = false;
        isReturning = false;
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
    }


}

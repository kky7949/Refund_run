using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("함정 설정")]
    public Transform player;
    public float triggerDistance = 15f;
    public float moveSpeed = 15f;
    public Vector3 moveDirection = new Vector3(-1, 0, 0);

    [Header("오디오 설정")]
    public AudioSource trapAudio;
    public AudioClip flySound;

    private bool isTriggered = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        if (trapAudio == null) trapAudio = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (!isTriggered && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            
            if (distance <= triggerDistance)
            {
                isTriggered = true;

                if (trapAudio != null && flySound != null)
                {
                    trapAudio.PlayOneShot(flySound);
                }   
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

        if (trapAudio != null && trapAudio.isPlaying)
        {
            trapAudio.Stop();
        }
    }
}

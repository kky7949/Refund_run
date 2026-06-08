using UnityEngine;

public class OscillatingPlatform : MonoBehaviour
{
    public Vector3 localOffset = new Vector3(0f, 1.4f, 0f);
    public float cycleSeconds = 2.2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float cycle = Mathf.Max(0.1f, cycleSeconds);
        float t = (Mathf.Sin(Time.time * Mathf.PI * 2f / cycle) + 1f) * 0.5f;
        transform.position = startPosition + localOffset * t;
    }
}

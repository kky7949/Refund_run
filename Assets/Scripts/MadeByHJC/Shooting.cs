using JetBrains.Annotations;
//using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject obstaclePef;
    public float waitTime = 3.0f;
    private float currentWaitTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWaitTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaitTime > waitTime)
        {
            GameObject obstacle = Instantiate(obstaclePef);
            obstacle.transform.position = transform.position - transform.forward;
            obstacle.GetComponent<Rigidbody>().AddForce(-transform.forward * 75.0f, ForceMode.Impulse);
            Destroy(obstacle, 3.0f);
            currentWaitTime = 0.0f;
        }
        currentWaitTime += Time.deltaTime;
    }
}

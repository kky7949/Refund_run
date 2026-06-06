using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("2.5D 카메라 세팅")]
    public Transform target; // 따라갈 캐릭터의 Transform
    public float offsetY = 3f;  // 캐릭터보다 얼마나 높이 있을지
    public float fixedZ = -12f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            // 캐릭터의 X 위치만
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + offsetY, fixedZ);
            
            // 카메라의 위치를 업데이트
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }
}

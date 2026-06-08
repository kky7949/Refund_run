using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 캐릭터의 Transform
    public float offsetZ = -10f; // 캐릭터와의 거리
    public float offsetY = 2f;  // 캐릭터보다 얼마나 높이 있을지

    void LateUpdate()
    {
        if (target != null)
        {
            // 캐릭터의 X 위치만
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + offsetY, target.position.z + offsetZ);
            
            // 카메라의 위치를 업데이트
            transform.position = targetPosition;
        }
    }
}

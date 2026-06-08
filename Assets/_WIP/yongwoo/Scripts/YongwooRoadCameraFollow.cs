using UnityEngine;

public class YongwooRoadCameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("카메라 위치")]
    public float offsetY = 3.75f;
    public float fixedZ = -14f;
    public float followSpeed = 6f;
    public float xAngle = 15f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 플레이어의 X/Y만 따라가고, Z는 고정해서 2.5D처럼 보이게 한다.
        // X축 15도 기준으로 offsetY 3.75면 플레이어가 화면 중앙 근처에 온다.
        Vector3 nextPosition = new Vector3(target.position.x, target.position.y + offsetY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * followSpeed);

        // X축으로 살짝 눕혀서 Z축 도로에서 차가 오는 방향을 더 잘 보이게 한다.
        transform.rotation = Quaternion.Euler(xAngle, 0f, 0f);
    }
}

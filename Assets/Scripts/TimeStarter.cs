using UnityEngine;

public class TimeStarter : MonoBehaviour
{
    void Start()
    {
        // 이 스크립트가 켜지는 순간 Timer 인스턴스를 찾아 OnTimer()를 실행합니다.
        if (Timer.instance != null)
        {
            Timer.instance.OnTimer();
        }
        else
        {
            Debug.LogWarning("타이머 인스턴스를 찾을 수 없습니다! Start 씬에서 타이머가 생성되었는지 확인하세요.");
        }
    }
}

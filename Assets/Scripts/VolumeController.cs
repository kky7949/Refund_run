using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            // 슬라이더를 움직일 때마다 SetVolume 함수가 실행되도록 연결
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // 슬라이더의 값(0.0 ~ 1.0)을 받아서 전체 볼륨에 적용
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
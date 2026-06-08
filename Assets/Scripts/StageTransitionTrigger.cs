using UnityEngine;
using UnityEngine.SceneManagement; 

public class StageTransitionTrigger : MonoBehaviour
{
    [Header("이동할 씬 이름 설정")]
    public string nextSceneName; // 인스펙터에서 직접 씬 이름을 타이핑할 수 있게 함

    [Header("효과음 설정 (선택사항)")]
    public AudioSource triggerAudio;
    public AudioClip clearSound;

    private bool isTransitioning = false; 

    void Start()
    {
        if (triggerAudio == null) triggerAudio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 닿은 물체의 태그가 "Player"이고, 아직 씬 이동 중이 아닐 때만 실행
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true; // 중복 실행 방지
            
            // 클리어 소리가 있다면 재생
            if (triggerAudio != null && clearSound != null)
            {
                triggerAudio.PlayOneShot(clearSound);
            }

            Debug.Log(nextSceneName + " 씬으로 이동합니다!");
            
            
            Invoke("LoadNextScene", 5.0f); 
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
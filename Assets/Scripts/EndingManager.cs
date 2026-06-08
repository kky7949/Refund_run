using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class EndingManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image cinematicImage; // 엔딩 이미지가 보여질 곳
    public GameObject buttonPanel; // ★ 메인/종료 버튼을 묶어둔 패널

    [Header("시네마틱 이미지 리스트")]
    public Sprite[] endingSprites; // 엔딩 컷 이미지들

    [Header("전환할 메인 씬 이름")]
    public string mainMenuSceneName = "main"; // 돌아갈 시작 화면 씬 이름

    [Header("페이드 효과 설정")]
    public float fadeDuration = 0.5f; 

    private int currentIdx = 0;
    private bool isFading = false;
    private bool isStoryFinished = false; // ★ 스토리가 끝났는지 체크

    void Start()
    {
        // 게임 시작 시 버튼은 안 보이게 숨겨둡니다.
        if (buttonPanel != null) buttonPanel.SetActive(false);

        if (endingSprites != null && endingSprites.Length > 0)
        {
            cinematicImage.sprite = endingSprites[currentIdx];
            StartCoroutine(FadeIn());
        }
        else
        {
            ShowButtons();
        }
    }

    void Update()
    {
        // 페이드 중이거나, 이미 스토리가 다 끝나서 버튼이 나온 상태라면 클릭 무시
        if (isFading || isStoryFinished) return; 

        bool isMouseClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool isSpacePress = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (isMouseClick || isSpacePress)
        {
            NextCut();
        }
    }

    void NextCut()
    {
        currentIdx++;
        
        // 남은 이미지가 있다면 다음 이미지로 페이드
        if (currentIdx < endingSprites.Length)
        {
            StartCoroutine(FadeToNextImage()); 
        }
        // 모든 이미지를 다 보았다면 버튼 패널 띄우기
        else
        {
            ShowButtons();
        }
    }

    IEnumerator FadeIn()
    {
        isFading = true;
        Color c = cinematicImage.color;
        c.a = 0f; 
        cinematicImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); 
            cinematicImage.color = c;
            yield return null;
        }
        c.a = 1f;
        cinematicImage.color = c;
        isFading = false;
    }

    IEnumerator FadeToNextImage()
    {
        isFading = true;
        Color c = cinematicImage.color;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            cinematicImage.color = c;
            yield return null;
        }

        cinematicImage.sprite = endingSprites[currentIdx];

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            cinematicImage.color = c;
            yield return null;
        }

        c.a = 1f;
        cinematicImage.color = c;
        isFading = false;
    }

    // ★ 마지막 컷에서 버튼 패널을 켜주는 함수
    void ShowButtons()
    {
        isStoryFinished = true; // 이제 마우스 클릭으로 안 넘어감
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
        }
    }

    // ==========================================
    // 아래는 버튼이 클릭되었을 때 실행될 함수들입니다.
    // ==========================================

    public void GoToMainMenu()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        // 유니티 에디터와 실제 빌드된 게임 모두에서 종료되게 하는 코드
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif   
    }
}
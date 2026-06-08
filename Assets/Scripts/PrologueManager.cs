using System.Collections; // ★ 코루틴(페이드 효과)을 사용하기 위해 필수
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class PrologueManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image cinematicImage; // 스토리가 보여질 UI Image 컴포넌트

    [Header("시네마틱 이미지 리스트")]
    public Sprite[] prologueSprites; // 보여줄 스토리 컷 이미지들 (배열)

    [Header("전환할 다음 씬 이름")]
    public string nextSceneName = "stage1";

    [Header("페이드 효과 설정")]
    public float fadeDuration = 0.5f; // 페이드 아웃/인에 걸리는 시간 (초)

    private int currentIdx = 0;
    private bool isFading = false; // ★ 페이드 중일 때 중복 클릭 방지용

    void Start()
    {
        if (prologueSprites != null && prologueSprites.Length > 0)
        {
            // 게임 시작 시 첫 이미지도 부드럽게 페이드 인으로 시작
            cinematicImage.sprite = prologueSprites[currentIdx];
            StartCoroutine(FadeIn());
        }
        else
        {
            LoadNextScene();
        }
    }

    void Update()
    {
        // ★ 화면이 깜빡거리며 넘어가는 중(페이드 중)일 때는 입력을 무시함
        if (isFading) return; 

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
        
        // 아직 보여줄 이미지가 더 남았다면
        if (currentIdx < prologueSprites.Length)
        {
            StartCoroutine(FadeToNextImage()); // 페이드 아웃 -> 교체 -> 페이드 인
        }
        // 모든 이미지를 다 보았으면
        else
        {
            StartCoroutine(FadeToNextScene()); // 페이드 아웃 -> 씬 이동
        }
    }

    // 1. 처음 시작할 때 부드럽게 나타나는 효과
    IEnumerator FadeIn()
    {
        isFading = true;
        Color c = cinematicImage.color;
        c.a = 0f; // 완전 투명하게 시작
        cinematicImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); // 서서히 불투명해짐
            cinematicImage.color = c;
            yield return null;
        }
        c.a = 1f;
        cinematicImage.color = c;
        isFading = false;
    }

    // 2. 이미지 사이를 넘어갈 때의 페이드 효과
    IEnumerator FadeToNextImage()
    {
        isFading = true;
        Color c = cinematicImage.color;

        // [페이드 아웃] 현재 이미지를 서서히 투명하게
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            cinematicImage.color = c;
            yield return null;
        }

        // 투명해진 상태에서 다음 이미지로 몰래 교체
        cinematicImage.sprite = prologueSprites[currentIdx];

        // [페이드 인] 새 이미지를 서서히 불투명하게
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

    // 3. 마지막 컷에서 게임 씬으로 넘어갈 때의 페이드 효과
    IEnumerator FadeToNextScene()
    {
        isFading = true;
        Color c = cinematicImage.color;

        // 마지막 이미지를 서서히 투명하게 (어두워짐)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            cinematicImage.color = c;
            yield return null;
        }

        // 화면이 완전히 사라지면 다음 스테이지 씬 로드
        SceneManager.LoadScene(nextSceneName);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
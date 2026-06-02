using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Button nextButton;
    public string nextSceneName = "Level_02";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (nextButton == null)
        {
            Debug.LogError("Next Button이 연결되지 않았습니다!");
            return;
        }

        nextButton.interactable = false;
        nextButton.onClick.AddListener(OnNextClicked);
    }

    public void ShowClear()
    {
        if (nextButton == null) return;
        nextButton.interactable = true; // 이것만으로 색상 자동 전환!
        Debug.Log("퍼즐 완성! Next 버튼 활성화");
    }

    void OnNextClicked()
    {
        SceneManager.LoadScene(nextSceneName); // "Level_02"
    }
}
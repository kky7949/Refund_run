using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    // 씬 이름을 매개변수로 받아 해당 씬으로 이동하는 하나의 함수로 통합
    public void LoadSceneByName(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    // 환경설정 창 같은 UI 패널을 켜고 끄기 위한 함수 -> (씬 전환 없이 화면에 띄우기)
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            // 패널이 켜져있으면 끄고, 꺼져있으면 켭니다.
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }

    public void Quit() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif   
    }
}

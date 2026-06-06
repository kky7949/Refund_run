using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MemoryPuzzle : MonoBehaviour
{
    [Header("플레이어 연결")]
    public NewMoveCS playerMovement;

    [Header("UI 연결")]
    // 왜 배열을 쓰는가.
    // -> 버튼 9개를 따로 변수로 만들면 코드가 지저분해짐
    // 배열을 쓰면 0번부터 8번까지 버튼을 묶어서 for문 쓰면됨.

    public Image[] buttonImages;
    public TextMeshProUGUI statusText;
    public GameObject introPanel;
 
    [Header("색상 설정")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    [Header("난이도 설정")]
    public int currentRound = 1;
    public int maxRounds = 3;


    // 왜 리스트를 쓰는가.
    // -> 배열은 처음에 크기를 정해야하지만, 리스트는 크기가 자유롭게 늘어남
    // 게임이 진행될수로 기억해야 할 정답 개수가 계속 늘어날 수 있기 때문.

    private List<int> correctSequence = new List<int>();

    // 플레이어가 몇 번째 정답을 누를차례인가.
    private int playerStep = 0;
    // 플레이어가 버튼을 누를 수 있는 상태인지 막아두는 변수 (불빛이 보여줄때 못누르기)
    private bool isPlayerTurn = false;

    void OnEnable()
    {
        if (playerMovement != null)
        {
            playerMovement.isPlayingPuzzle = true;
        }

        introPanel.SetActive(true);
        statusText.text = "";
        isPlayerTurn = false;
    }

    void OnDisable()
    {
        if (playerMovement != null)
        {
            playerMovement.isPlayingPuzzle = false;
        }
    }

    public void OnStartButtonClick()
    {
        introPanel.SetActive(false);
        currentRound = 1;
        StartCoroutine(StartGame());
    }
    // 왜 코루틴을 쓸까.
    // -> 시간의 흐름제어, 일반 함수로는 코드가 너무 길어짐. 이번에만 사용

    IEnumerator StartGame()
    {
        isPlayerTurn = false;
        playerStep = 0; // 클릭순서 초기화
        correctSequence.Clear(); // 이전게임 정답 초기화

        int sequenceLength = currentRound + 2;

    // 랜덤한 순서를 출제
        for(int i = 0; i < sequenceLength; i++) 
        {
            correctSequence.Add(Random.Range(0, 9));
        }

        yield return StartCoroutine(ShowSequence());
    
    }

    IEnumerator ShowSequence()
    {
        statusText.text = currentRound + "라운드 준비... 잘 보세요!";
        yield return new WaitForSeconds(1.5f);

        statusText.text = "순서 기억하기!";
        foreach (int btnIndex in correctSequence)
        {
            buttonImages[btnIndex].color = highlightColor;
            yield return new WaitForSeconds(0.5f);

            buttonImages[btnIndex].color = normalColor;
            yield return new WaitForSeconds(0.2f);
        }
        isPlayerTurn = true;
        statusText.text = "순서대로 누르세요!";
        Debug.Log("플레이어 입력 시작!");
    }

    public void OnButtonClik(int clickIndex)
    {
        if (!isPlayerTurn) return;

        if (clickIndex == correctSequence[playerStep])
        {
            playerStep++;
            Debug.Log("정답! 다음 버튼 누르세요.");

            if (playerStep >= correctSequence.Count)
            {
                // 아직 마지막 라운드가 아니면 난이도를 올리고 다음 라운드로 진행
                if (currentRound < maxRounds)
                {
                    currentRound++;
                    Debug.Log("퍼즐 클리어, 난이도가 증가합니다. (현재 라운드: " + currentRound + ")");
                    StartCoroutine(RoundClearRoutine()); 
                }
                else 
                {
                    statusText.text = "모든 라운드 완벽 클리어!";
                    Debug.Log("모든 라운드 완벽 클리어!");
                    StartCoroutine(ClosePuzzleRoutine());
                }
            }
        }
        else 
        {
            Debug.Log("틀렸습니다! 현재 " + currentRound + "번째 라운드입니다.");
            StartCoroutine(RestartRoundRoutine());
        }
    }
    IEnumerator RoundClearRoutine()
    {
        isPlayerTurn = false; 
        statusText.text = "클리어! 다음 문제가 나옵니다.";
        yield return new WaitForSeconds(2.0f); 
        StartCoroutine(StartGame());
    }

    IEnumerator RestartRoundRoutine()
    {
        isPlayerTurn = false;
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StartGame());
    }

    IEnumerator ClosePuzzleRoutine()
    {
        isPlayerTurn = false;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false); 
    }
}
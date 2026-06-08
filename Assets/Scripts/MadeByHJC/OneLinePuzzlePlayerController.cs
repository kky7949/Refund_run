using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OneLinePuzzlePlayerController : MonoBehaviour
{
    public Transform puzzleCameraPos;   // 카메라 위치
    public Transform startPosition;     // 게임 시작 지즘
    public Transform startPoint;        // 캐릭터 퍼즐 시작 지점
    public Transform goalPoint;         // 클리어 시 도착 지점
    public Transform flyGoal;
    public Material basicBlock;         // 기본 블록 재질
    public Material stepedBlock;        // 밟은 블록 재질
    public GameObject[] bridge = new GameObject[3];           // 퍼즐에 사용될 블록들
    public AnimatorController puzzleAnimController;   // 퍼즐에서 사용할 애니메이션 컨트롤러
    public AnimatorController basicAnimController;    // 기본 애니메이션 컨트롤러
    public GameObject smokePref;
    public GameObject explainPanel;
    public GameObject explainBtn;
    public Toggle explainToggle;


    private GameObject currentBlock;    // 밟은 블록 저장 변수
                                        // 블록을 밟고 다음 블록으로 넘어갈 때
                                        // 지금 밟고 있는 블록을 변경하고 넘어가기 위해
                                        // 블록의 정보를 저장해놓음
    private int stepCount;      // 밟은 블록의 개수
    private quaternion cam;     // 기본 카메라 회전각 저장
    private Animator anim;      // 애니메이션을 담당하는 컴포넌트
    private float deadTimer;
    private float respawnTime = 1.5f;
    private GameObject usedBridge;
    private Rigidbody rb;
    private float smoking;
    private bool isBridge;

    void Awake()
    {
        anim = GetComponent<Animator>();
        deadTimer = 0.0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // 퍼즐 입장 시 시행될 함수
    void OnEnable()
    {
        int bridgeNum = UnityEngine.Random.Range(0, bridge.Length);
        usedBridge = bridge[bridgeNum];
        usedBridge.SetActive(true);
        
        anim.runtimeAnimatorController = puzzleAnimController;  // 애니메이션 컨트롤러 교체
        transform.position = usedBridge.transform.GetChild(0).position + Vector3.up;  // 위치
        GetComponent<NewMoveCS>().enabled = false;              // 기존의 이동 방식 중지
        transform.LookAt(transform.position + Vector3.right);   // 방향
        stepCount = 0;                                          // 밟은 블록 개수 초기화

        // 위에서 카메라가 내려다봄
        cam = Camera.main.transform.rotation;
        Camera.main.GetComponent<CameraFollow>().enabled = false;   // 기존에 있던 카메라가 따라가는 방식을 중지
        Camera.main.transform.position = puzzleCameraPos.position;  // 카메라 위치
        Camera.main.transform.rotation = puzzleCameraPos.rotation;  // 카메라 방향
        Camera.main.orthographic = true;                            // 원근감 제거
        Camera.main.orthographicSize = 7.0f;                        // 카메라 시야 조절

        rb = GetComponent<Rigidbody>();
        smoking = 0;
        isBridge = true;
        if (!explainToggle.isOn)
        {
            explainPanel.SetActive(true);
            explainBtn.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 인풋 받기
        Vector2 input = Vector2.zero;
        if(deadTimer > 0)
        {
            deadTimer -= Time.deltaTime;
            smoking += Time.deltaTime;
            if(deadTimer <= 0)
            {
                transform.position = startPosition.position;
                transform.rotation = startPosition.rotation;
                GetComponent<NewMoveCS>().enabled = true;
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                enabled = false;
            }
            if(smoking > 0.05f)
            {
                Instantiate(smokePref, transform.position - Vector3.up * 0.5f, transform.rotation);
                smoking = 0;
            }
            if(deadTimer >= 1.0)
            {
                Camera.main.transform.position = new Vector3(transform.position.x, puzzleCameraPos.position.y ,puzzleCameraPos.position.z);
            }
            Vector3 direction = (flyGoal.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 10;
        }
        else if (Keyboard.current != null && isBridge) {
            if (Keyboard.current.aKey.wasPressedThisFrame) input.x = 1;
            else if (Keyboard.current.dKey.wasPressedThisFrame) input.x = -1;
            else if (Keyboard.current.wKey.wasPressedThisFrame) input.y = 1;
            else if (Keyboard.current.sKey.wasPressedThisFrame) input.y = -1;
            Move(input); // 실질적으로 옮기는 함수
        }
        
    }

    // 캐릭터를 옮김
    void Move(Vector2 input)
    {
        // 이동 시 블록 내용 변경
        if(currentBlock != null && input != Vector2.zero) {   // 움직일 때만 감지하기 위해 input값이 zero면 안 움직임
            currentBlock.GetComponent<BoxCollider>().isTrigger = true;          // isTrigger 설정으로 지나간 자리에 들어가면 빠짐
            currentBlock.GetComponent<MeshRenderer>().material = stepedBlock;   // 지나간 자리의 재질 변경
            anim.SetTrigger("walk");
            if(GetComponent<CameraFollow>() != null)
            {
                Debug.Log("exist");
            }
            isBridge = false;
        }

        Vector3 direction = new Vector3(input.y, 0.0f, input.x);    // 볼 방향
        transform.LookAt(transform.position + direction);           // 바라봄
        transform.position = transform.position + direction;        // 이동
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!this.enabled) return;
        if (coll.gameObject.CompareTag("Bridge"))
        {
            isBridge = true;
            currentBlock = coll.gameObject;    // 밟은 블록의 정보 저장
            stepCount++;

            // 블록을 전부 밟았을 시 통과
            if(stepCount == usedBridge.transform.childCount)   
            {
                transform.position = goalPoint.position;
                transform.rotation = goalPoint.rotation;
                GetComponent<NewMoveCS>().enabled = true;
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                enabled = false;
            }
        }
        else if (coll.gameObject.CompareTag("Lava"))
        {
            transform.Rotate(Vector3.right * -40);
            rb.AddForce(Vector3.up * 10.0f, ForceMode.Impulse);
            Camera.main.orthographic = false;
            deadTimer = respawnTime;
        }
    }

    void OnDisable()
    {
        anim.runtimeAnimatorController = basicAnimController;
        Camera.main.transform.rotation = cam;
        Camera.main.orthographic = false;

        if(usedBridge == null) return;
        for(int i = 0; i < usedBridge.transform.childCount; i++)
        {
            usedBridge.transform.GetChild(i).GetComponent<MeshRenderer>().material = basicBlock;
            usedBridge.transform.GetChild(i).GetComponent<BoxCollider>().isTrigger = false;
        }
        usedBridge.SetActive(false);
        explainBtn.SetActive(false);
    }
}

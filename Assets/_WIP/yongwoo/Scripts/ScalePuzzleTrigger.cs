using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(Collider))]
public class ScalePuzzleTrigger : MonoBehaviour
{
    [SerializeField] private string puzzleSceneName = "ScalePuzzle";
    [SerializeField] private string puzzleScenePath = "Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity";
    [SerializeField] private YongwooPlayerController playerMovement;
    [SerializeField] private bool disableAfterSolved = true;

    private bool loading;
    private bool closing;
    private bool solved;
    private ScalePuzzleController activePuzzle;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneUnloaded -= HandleSceneUnloaded;

        if (activePuzzle != null)
        {
            activePuzzle.SolvedCorrectly -= HandlePuzzleSolved;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryOpenFor(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryOpenFor(other);
    }

    private void TryOpenFor(Collider other)
    {
        var incomingPlayer = other.GetComponentInParent<YongwooPlayerController>();
        if (incomingPlayer == null)
        {
            return;
        }

        TryOpenForPlayer(incomingPlayer);
    }

    public void TryOpenForPlayer(YongwooPlayerController incomingPlayer)
    {
        if (solved || loading || closing || incomingPlayer == null)
        {
            return;
        }

        if (playerMovement == null)
        {
            playerMovement = incomingPlayer;
        }

        OpenPuzzle();
    }

    private void Update()
    {
        if (!loading && !closing && !solved)
        {
            CheckPlayerOverlap();
        }

        if (!loading || activePuzzle != null)
        {
            return;
        }

        var scene = SceneManager.GetSceneByName(puzzleSceneName);
        if (scene.isLoaded)
        {
            BindPuzzle(scene);
        }

        if (closing && !SceneManager.GetSceneByName(puzzleSceneName).isLoaded)
        {
            CompleteClose();
        }
    }

    private void CheckPlayerOverlap()
    {
        var triggerCollider = GetComponent<Collider>();
        var bounds = triggerCollider.bounds;
        var hits = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, ~0, QueryTriggerInteraction.Collide);

        foreach (var hit in hits)
        {
            if (hit == triggerCollider)
            {
                continue;
            }

            var overlappingPlayer = hit.GetComponentInParent<YongwooPlayerController>();
            if (overlappingPlayer == null)
            {
                continue;
            }

            TryOpenForPlayer(overlappingPlayer);
            return;
        }
    }

    private void OpenPuzzle()
    {
        loading = true;
        playerMovement.enabled = false;

        var body = playerMovement.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        var scene = SceneManager.GetSceneByName(puzzleSceneName);
        if (!scene.isLoaded)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
#if UNITY_EDITOR
            // Build Settings에 씬을 안 넣어도 에디터에서는 yongwoo 폴더의 씬 경로로 연다.
            EditorSceneManager.LoadSceneAsyncInPlayMode(puzzleScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
#else
            SceneManager.LoadSceneAsync(puzzleSceneName, LoadSceneMode.Additive);
#endif
            return;
        }

        BindPuzzle(scene);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != puzzleSceneName)
        {
            return;
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        BindPuzzle(scene);
    }

    private void BindPuzzle(Scene scene)
    {
        activePuzzle = FindPuzzleController(scene);
        if (activePuzzle != null)
        {
            activePuzzle.SolvedCorrectly += HandlePuzzleSolved;
            loading = false;
        }
    }

    private void HandlePuzzleSolved(ScalePuzzleController puzzle)
    {
        if (activePuzzle != null)
        {
            activePuzzle.SolvedCorrectly -= HandlePuzzleSolved;
        }

        solved = true;
        closing = true;
        activePuzzle = null;

        var scene = SceneManager.GetSceneByName(puzzleSceneName);
        if (scene.isLoaded)
        {
            SceneManager.sceneUnloaded -= HandleSceneUnloaded;
            SceneManager.sceneUnloaded += HandleSceneUnloaded;
            SceneManager.UnloadSceneAsync(scene);
            return;
        }

        CompleteClose();
    }

    private void HandleSceneUnloaded(Scene scene)
    {
        if (scene.name != puzzleSceneName)
        {
            return;
        }

        SceneManager.sceneUnloaded -= HandleSceneUnloaded;
        CompleteClose();
    }

    private void CompleteClose()
    {
        closing = false;
        playerMovement.enabled = true;

        if (disableAfterSolved)
        {
            gameObject.SetActive(false);
        }
    }

    private static ScalePuzzleController FindPuzzleController(Scene scene)
    {
        if (!scene.isLoaded)
        {
            return null;
        }

        foreach (var root in scene.GetRootGameObjects())
        {
            var controller = root.GetComponentInChildren<ScalePuzzleController>(true);
            if (controller != null)
            {
                return controller;
            }
        }

        return null;
    }
}

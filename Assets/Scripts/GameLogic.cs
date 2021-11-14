using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _logic;
    public static GameLogic Logic => _logic;

    private Camera _currentCamera;
    private Vector3 _currentRespawnPosition;

    [Header("Components")] 
    private PostProcessManager postProcessManager;
    
    [Header("Properties")] 
    [SerializeField]
    private Vector2 screenSize;
    [SerializeField]
    private bool pause;

    private void Awake()
    {
        if (_logic != null)
        {
            Destroy(gameObject);
            return;
        }

        _logic = this;
    }

    private void Start()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
    }

    public void SetCamera(Camera newCamera)
    {
        _currentCamera = newCamera;
    }

    public void SetPostProcessManager(PostProcessManager manager)
    {
        postProcessManager = manager;
    }

    public void SetRespawnPoint(Vector3 respawnPoint)
    {
        _currentRespawnPosition = respawnPoint;
    }

    public void Pause() => pause = true;
    public bool IsPaused() => pause;

    public Camera CurrentCamera => _currentCamera;
    public PostProcessManager PostProcessManager => postProcessManager;
    public Vector3 CurrentRespawnPosition => _currentRespawnPosition;
}
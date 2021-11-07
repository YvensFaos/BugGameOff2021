using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _logic;
    public static GameLogic Logic => _logic;

    private Camera currentCamera;

    [Header("Components")]
    
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
        currentCamera = newCamera;
    }

    public void Pause() => pause = true;
    public bool IsPaused() => pause;

    public Camera CurrentCamera => currentCamera;
}
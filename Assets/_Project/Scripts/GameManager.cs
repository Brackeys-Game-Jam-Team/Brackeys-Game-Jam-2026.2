using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public InputManager InputManager { get; private set; }

    public StateMachine StateMachine { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StateMachine.Enter();
    }
}
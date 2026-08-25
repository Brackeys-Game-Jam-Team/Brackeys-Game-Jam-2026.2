using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
    [field: SerializeField] public Gameplay Gameplay { get; private set; }

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

        StateMachine = new(this);
        StateMachine.AddState(new MainMenuState(StateMachine));
        StateMachine.AddState(new GameplayState(StateMachine));
        StateMachine.SetDefaultState<MainMenuState>();
    }

    private void Start()
    {
        StateMachine.Enter();
    }
}
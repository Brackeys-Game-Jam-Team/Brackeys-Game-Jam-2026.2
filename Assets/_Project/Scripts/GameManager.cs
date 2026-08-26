using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
    [field: SerializeField] public Gameplay Gameplay { get; private set; }

    public StateMachine StateMachine { get; private set; }
    public string CurrentSceneName { get; private set; }
    public bool IsLoadingScene { get; private set; }
    public bool CanSelectCard { get; set; }
    
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

    public Coroutine LoadScene(string sceneName, bool unloadCurrent = true)
    {
        return StartCoroutine(LoadSceneRoutine(sceneName, unloadCurrent));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool unloadCurrent)
    {
        IsLoadingScene = true;

        // Unload old content scene
        if (unloadCurrent && !string.IsNullOrEmpty(CurrentSceneName))
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(CurrentSceneName);

            if (unload != null)
            {
                while (!unload.isDone)
                    yield return null;
            }
        }

        // Load new content scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        load.allowSceneActivation = false;

        while (load.progress < 0.9f)
            yield return null;

        load.allowSceneActivation = true;

        while (!load.isDone)
            yield return null;

        // Set new scene as active
        Scene loaded = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loaded);

        CurrentSceneName = sceneName;
        IsLoadingScene = false;
    }

}
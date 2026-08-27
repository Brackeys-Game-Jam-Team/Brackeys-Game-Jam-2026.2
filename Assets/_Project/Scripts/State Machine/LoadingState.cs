using System;
using UnityEngine;

public class LoadingState : State<GameManager>
{
    private string targetScene;
    private Type targetState;
    private bool sceneLoaded;

    public LoadingState(StateMachine owner) : base(owner)
    {
    }

    public LoadingState Configure(string sceneName, Type nextState)
    {
        targetScene = sceneName;
        targetState = nextState;
        return this;
    }

    public override void Enter()
    {
        Debug.Log($"[Loading] Loading scene: {targetScene}");
        sceneLoaded = false;
        context.UIManager.ShowScreen<LoadingScreen>();

        context.OnSceneLoaded += HandleSceneLoaded;
        context.LoadScene(targetScene);
    }

    public override void Exit()
    {
        context.UIManager.HideScreen<LoadingScreen>();
        context.OnSceneLoaded -= HandleSceneLoaded;
    }

    public override void Update(float deltaTime)
    {
        if (sceneLoaded)
            owner.ChangeState(targetState);
    }

    private void HandleSceneLoaded(string sceneName)
    {
        if (sceneName == targetScene)
            sceneLoaded = true;
    }
}
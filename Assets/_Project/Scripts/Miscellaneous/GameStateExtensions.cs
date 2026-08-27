/// <summary>
/// Helper to transition through LoadingState with a scene change.
/// </summary>
public static class GameStateExtensions
{
    public static void TransitionToScene<T>(this StateMachine machine, string sceneName) where T : IState
    {
        var loading = machine.GetState<LoadingState>();
        loading.Configure(sceneName, typeof(T));
        machine.ChangeState<LoadingState>();
    }
}
public abstract class State<T> : IState where T : class
{
    protected StateMachine owner;
    protected T context;

    public State(StateMachine owner)
    {
        this.owner = owner;
        context = owner.Context as T;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void FixedUpdate(float deltaTime)
    {
    }

    public virtual void LateUpdate(float deltaTime)
    {
    }
}
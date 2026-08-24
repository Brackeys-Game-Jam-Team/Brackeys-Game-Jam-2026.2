public interface IState
{
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
//抽象类中实现子类的功能
public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyController enemy);
    public abstract void OnUpdate(EnemyController enemy);

}

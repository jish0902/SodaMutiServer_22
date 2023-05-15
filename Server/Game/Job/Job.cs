using System;

namespace Server.Game;

public abstract class IJob
{
    public bool Cancel { get; set; } = false;
    public abstract void Execute();
}

internal class Job : IJob
{
    private readonly Action _action;

    public Job(Action action)
    {
        _action = action;
    }

    public override void Execute()
    {
        if (Cancel == false)
            _action.Invoke();
    }
}

internal class Job<T1> : IJob
{
    private readonly Action<T1> _action;
    private readonly T1 _t1;


    public Job(Action<T1> action, T1 t1)
    {
        _action = action;
        _t1 = t1;
    }

    public override void Execute()
    {
        if (Cancel == false)
            _action.Invoke(_t1);
    }
}

internal class Job<T1, T2> : IJob
{
    private readonly Action<T1, T2> _action;
    private readonly T1 _t1;
    private readonly T2 _t2;


    public Job(Action<T1, T2> action, T1 t1, T2 t2)
    {
        _action = action;
        _t1 = t1;
        _t2 = t2;
    }

    public override void Execute()
    {
        if (Cancel == false)
            _action.Invoke(_t1, _t2);
    }
}

internal class Job<T1, T2, T3> : IJob
{
    private readonly Action<T1, T2, T3> _action;
    private readonly T1 _t1;
    private readonly T2 _t2;
    private readonly T3 _t3;

    public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
    {
        _action = action;
        _t1 = t1;
        _t2 = t2;
        _t3 = t3;
    }

    public override void Execute()
    {
        if (Cancel == false)
            _action.Invoke(_t1, _t2, _t3);
    }
}
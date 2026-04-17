using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public StateMachine stateMachine;
    public State(StateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
    }
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}

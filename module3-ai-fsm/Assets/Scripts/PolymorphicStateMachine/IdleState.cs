using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public IdleState(StateMachine _stateMachine) : base(_stateMachine) { }

    public override void Enter()
    {
        stateMachine.idleTime = Time.time;
    }

    public override void Execute()
    {

        stateMachine.agent.speed = stateMachine.normalSpeed;

        // during idle, can never see player
        stateMachine.canSeePlayer = false;

        float idleTimeElapsed = Time.time - stateMachine.idleTime;
        if (idleTimeElapsed >= stateMachine.idleTimeThreshold)
        {
            stateMachine.ChangeState(new PatrolState(stateMachine));
        }
    }

    public override void Exit()
    {

    }

}

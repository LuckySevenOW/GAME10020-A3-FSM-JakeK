using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChaseState : State
{
    public ChaseState(StateMachine _stateMachine) : base(_stateMachine) { }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        stateMachine.agent.speed = stateMachine.chaseSpeed;
        stateMachine.agent.SetDestination(stateMachine.character.position);

        stateMachine.canSeePlayer = stateMachine.IsInViewCone();
        if (!stateMachine.canSeePlayer)
        {
            stateMachine.ChangeState(new SearchState(stateMachine));
        }
    }

    public override void Exit()
    {
    }

}

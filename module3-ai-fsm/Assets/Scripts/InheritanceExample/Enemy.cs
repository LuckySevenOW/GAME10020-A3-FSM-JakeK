using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GenericCharacter
{
    public float attack = 2f;
    public float defense = 2f;
    public override void Move()
    {
        // use this if you need to call the base class implementation
        base.Move();

        FightMovement();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (damage < 35f)
        {
            TriggerFightPhase2();
        }
    }


    void FightMovement()
    {

    }

    void TriggerFightPhase2()
    {

    }
}

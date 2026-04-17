using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GenericCharacter
{
    public float attack = 2f;
    public float defense = 2f;
    public float fireMagic = 5f;
    public override void Move()
    {
        // use this if you need to call the base class implementation
        base.Move();

        InputMovement();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        UpdateHealthBar();
    }

    public void InputMovement()
    {

    }

    public void UpdateHealthBar()
    {

    }
}

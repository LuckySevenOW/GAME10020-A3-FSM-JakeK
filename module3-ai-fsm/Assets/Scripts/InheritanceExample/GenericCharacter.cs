using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCharacter : MonoBehaviour
{
    public string characterName = "Player Name";
    public float health = 100f;

    public virtual void Move()
    {
        Debug.Log("Player movement");
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"{characterName} takes damage: {damage}");
        health -= damage;
    }
}

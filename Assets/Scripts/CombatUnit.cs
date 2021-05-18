using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health;
    protected bool dead;
    protected float decayTimer = 45f;

    public virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if(health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void TakeMeleeHit(float damage)
    {
        health -= damage;

        print(transform.name+" Took " + damage + " Damage! Health: " + health);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        dead = true;
    }
}

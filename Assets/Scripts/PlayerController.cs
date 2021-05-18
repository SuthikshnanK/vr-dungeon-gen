using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CombatUnit
{
    HealthBar healthbar;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        healthbar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
        healthbar.SetMaxHealth((int)startingHealth);
        FindObjectOfType<GameManager>();
    }

    public override void TakeMeleeHit(float damage)
    {
        health -= damage;

        healthbar.SetHealth((int)health);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();
        FindObjectOfType<GameManager>().EndGame();
        Destroy(gameObject);
    }
}

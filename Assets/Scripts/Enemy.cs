using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : CombatUnit
{

    NavMeshAgent pathfinder;
    Transform target;
    public Animator anim;
    public float lookRadius = 10f;
    public float attackRadius = 12f;
    private ParticleSystem particles;
    public bool hasSeen;

    private float hitCooldown = 3f;
    private float timeUntilNextAttack = 0f;

    public int damage = 10;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        particles = gameObject.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {


        float move = pathfinder.velocity.magnitude;
        if (move > 0.5f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
    public override void Die()
    {
        base.Die();
        anim.SetBool("isDead", true);
        particles.Play();
        GameObject.Destroy(gameObject, anim.GetNextAnimatorStateInfo(0).length+decayTimer);
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (target != null)
        {

            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

            float distance = Vector3.Distance(targetPosition, transform.position);

            if ((distance <= lookRadius || hasSeen) && !dead)
            {
                hasSeen = true;
                pathfinder.SetDestination(target.position);

                if (distance <= attackRadius && Time.time > timeUntilNextAttack)
                {
                    anim.SetTrigger("attack");
                    IDamageable damageableObj = target.GetComponent<IDamageable>();
                    if (damageableObj != null)
                    {
                        damageableObj.TakeMeleeHit(damage);
                        timeUntilNextAttack = Time.time + hitCooldown;
                    }
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public LayerMask collisionMask;
    public LayerMask terrainCollider;
    public float speed = 10;
    public float damage = 1;
    public float maxDist = 200f;
    public float distTravelled = 0f;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaMove = speed * Time.deltaTime;
        CheckCollisions(deltaMove);
        transform.Translate(Vector3.forward * deltaMove);
        if(distTravelled >= maxDist)
        {
            Destroy(gameObject);
        }
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
        else if(Physics.Raycast(ray, out hit, moveDistance, terrainCollider, QueryTriggerInteraction.Collide))
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            distTravelled += moveDistance;
        }

    }

    void OnHitObject(RaycastHit hit)
    {
        print(hit.transform.name+" was hit");
        IDamageable damageableObj = hit.collider.GetComponent<IDamageable>();
        if(damageableObj != null)
        {
            damageableObj.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public Transform muzzle;
    public Projectile projectile;
    public AudioSource audioSource;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    float nextShotTime;

    public void Shoot()
    {
        if( Time.time > nextShotTime)
        {
            audioSource.PlayOneShot(audioSource.clip);
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{

    [SerializeField]
    private AudioClip stepClip;

    [SerializeField]
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        audioSource.PlayOneShot(stepClip);
    }
}

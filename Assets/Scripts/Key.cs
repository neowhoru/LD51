using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private bool isFollowing;
    private bool isMovingToAnotherPosition;
    public float followSpeed = 1;
    public Transform followTarget;
    private Vector2 targetPosition;

    public AudioSource audioSource;
    public AudioClip collectsound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
        }

        if (isMovingToAnotherPosition)
        {
            if (Vector3.Distance(targetPosition, transform.position) < 0.1f)
            {
                isMovingToAnotherPosition = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);    
            }
            
        }
    }

    public void MoveToNewPosition(Vector2 newPosition)
    {
        targetPosition = newPosition;
        isFollowing = false;
        isMovingToAnotherPosition = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (!isFollowing)
            {
                PlayerTopDownController playerMovement = other.GetComponent<PlayerTopDownController>();
                followTarget = playerMovement.keyFollowPoint;
                isFollowing = true;
                playerMovement.followingKey = this;
                audioSource.PlayOneShot(collectsound);
            }
        }
    }
}

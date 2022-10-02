using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerTopDownController : MonoBehaviour
{
    public float speed = 5f;
        private Rigidbody2D myRigidbody;
        private Vector3 change;
        private Animator anim;
        private AudioSource audioSource;
        public AudioSource footStepSoundSource;
        public bool canMove = true;
        
        public Key followingKey;
        public Transform keyFollowPoint;
    
        public AudioClip collectSound;
        public AudioClip fallSound;

        public GameObject deathPrefab;

        public Light2D playerLight;
        
        
        // Start is called before the first frame update
        void Start()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            footStepSoundSource.enabled = false;
        }
    
        // Update is called once per frame
        void Update()
        {
            change = Vector3.zero;
            change.x = Input.GetAxis("Horizontal");
            change.y = Input.GetAxis("Vertical");
      
        }
    
        private void FixedUpdate()
        {
            UpdateAnimationAndMove();
        }
    
        void UpdateAnimationAndMove()
        {
            if (change != Vector3.zero && canMove)
            {
                MoveCharacter();
                anim.SetFloat("moveX", change.x);
                anim.SetFloat("moveY", change.y);
                anim.SetBool("isMoving", true);
                footStepSoundSource.enabled = true;
            }
            else
            {
                footStepSoundSource.enabled = false;
                anim.SetBool("isMoving", false);
            }
        }
    
        void MoveCharacter()
        {
            myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
        }
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Collectable"))
            {
                Destroy(collision.gameObject);
                audioSource.PlayOneShot(collectSound);
            }
    
            if (collision.tag.Equals("Hazard"))
            {
                PlayerDieFall();
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.tag.Equals("Hazard"))
            {
                PlayerDieFall();
            }
        }

        public void PlayerDieFall()
        {
            Debug.Log("Player Die");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
            canMove = false;
            Invoke("PlayerReload", 1);
            
        }
    
        public void PlayerReload()
        {
            FindObjectOfType<GameManager>().PrepareRestartLevel();
        }

        public void DisablePlayerLight()
        {
            playerLight.enabled = false;
        }
}

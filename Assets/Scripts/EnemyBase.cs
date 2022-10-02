using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int scoreValue = 1000;
    public AudioClip sfx;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    private CircleCollider2D _circleCollider2D;
    private Animator _animator;
    public bool isEnabled = false;
    
    public Transform target;
    public float chaseRadius;
    public float attakRadius;
    public Transform homePosition;
    public float moveSpeed = 5f;

    public Vector3 startPosition;
    public GameManager _gameManager;
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _gameManager = FindObjectOfType<GameManager>();
        startPosition = transform.position;
    }

    public void DisableEnemy()
    {

        isEnabled = false;
        _spriteRenderer.enabled = false;
    }
    
    
    void Update()
    {
        if (isEnabled)
        {
            CheckDistance();    
        }
    }

    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attakRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            _spriteRenderer.flipX = target.transform.position.x > transform.position.x;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Bullet"))
        {
            EnemyDeath();
        }

        if (other.tag.Equals("BrickGround"))
        {
            if (other.gameObject.GetComponent<BrickTile>().currentState == BrickTile.BRICK_STATE.OPEN)
            {
                EnemyDeath();
            }
        }
    }

    public void EnemyDeath()
    {
        isEnabled = false;
        _collider2D.enabled = false;
        _animator.SetBool("IsDead",true);
        
        if (_audioSource != null && sfx != null)
        {
            _audioSource.PlayOneShot(sfx);
        }
            
        Invoke(nameof(DisableEnemy), 1f);
    }
    
}

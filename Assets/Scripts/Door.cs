using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite openFinalSprite;
    public Sprite closeSprite;

    private GameManager m_GameManager;
    private LevelSetting _levelSetting;
    public string TargetLevel; 
    public enum DOOR_STATE
    {
        OPEN,
        CLOSE
    }

    public DOOR_STATE currentState = DOOR_STATE.CLOSE;
    public bool isWaitingToOpenDoor;

    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;
    private PlayerTopDownController player;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        player = FindObjectOfType<PlayerTopDownController>();
        m_GameManager = FindObjectOfType<GameManager>();
        _levelSetting = FindObjectOfType<LevelSetting>();

        if (_levelSetting.isLastLevel)
        {
            openSprite = openFinalSprite;
        }

    }

    private void Update()
    {
        if (isWaitingToOpenDoor)
        {
            if (Vector3.Distance(player.followingKey.transform.position, transform.position) < 0.1f)
            {
                isWaitingToOpenDoor = false;
                OpenDoor();
                player.followingKey.gameObject.SetActive(false);
                player.followingKey = null;
            }
        }
        else
        {
            if (player.followingKey != null)
            {
                float distance = Vector3.Distance(player.followingKey.transform.position, transform.position);
                if (distance < 3)
                {
                    TakeControlForKey();
                }
            }
        }
    }

    public void OpenDoor()
    {
        currentState = DOOR_STATE.OPEN;
        _renderer.sprite = openSprite;
        _collider.isTrigger = true;
    }

    public void CloseDoor()
    {
        currentState = DOOR_STATE.CLOSE;
        _renderer.sprite = closeSprite;
        _collider.isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleCollision(other);
    }

    public void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentState == DOOR_STATE.CLOSE && player.followingKey != null)
            {
                TakeControlForKey();
                //SceneManager.LoadSceneAsync(TargetLevel);
            }

            if (currentState == DOOR_STATE.OPEN && TargetLevel != null)
            {
                player.canMove = false;
                m_GameManager.LoadLevel(TargetLevel);
                
            }
        }
    }

    public void TakeControlForKey()
    {
        player.followingKey.followTarget = transform;
        isWaitingToOpenDoor = true;
    }
}
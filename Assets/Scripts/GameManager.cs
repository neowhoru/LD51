using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    public Text eventTextMessage;
    public Text floorText;
    public Animator fadePanelAnimator;

    private bool isLevelActive = true;
    
    public enum GAME_EVENTS
    {
        DESTROY_TILES,
        SPAWN_ENEMY,
        DARKEN_WORLD,
        MOVE_KEY
    }

    public GAME_EVENTS currentEvent = GAME_EVENTS.DESTROY_TILES;

    public int secondsLeft = 10;


    public GameObject globalLight;
    public PlayerTopDownController playerTopDownController;

    public GameObject[] enemyPrefabs;
    private LevelSetting _levelSetting;

    private int spawnCountEnemies = 1;
    private int tileDestroyCount = 3;

    private string targetLevelScene = "Level1";
    
    void Start()
    {
        playerTopDownController = FindObjectOfType<PlayerTopDownController>();
        globalLight = GameObject.FindGameObjectWithTag("GlobalLight");

        if (globalLight.GetComponent<Light2D>().enabled)
        {
            playerTopDownController.DisablePlayerLight();
        }

        timerText.gameObject.SetActive(false);
        _levelSetting = FindObjectOfType<LevelSetting>();
        Invoke(nameof(StartTimer),1 );
        SetFloorText();
    }

    public void SetFloorText()
    {
        floorText.text = "<color=red>Floor:</color>" + _levelSetting.levelName;
    }

    private void Awake()
    {
        if (fadePanelAnimator.gameObject != null)
        {
            fadePanelAnimator.gameObject.SetActive(true);
            fadePanelAnimator.Play("FadeIn");    
        }
    }

    public void StartTimer()
    {
        timerText.gameObject.SetActive(true);
        InvokeRepeating(nameof(UpdateTimer), 0, 1);
    }
    
    public void UpdateTimer()
    {
        int seconds = Mathf.FloorToInt(secondsLeft % 60F);
        timerText.text = seconds + "s";
        secondsLeft--;
        if (secondsLeft < 0)
        {
            secondsLeft = 10;
            Debug.Log("Event Occur");
            HandleGameEvent();
        }
    }

    private void HandleGameEvent()
    {
        currentEvent = GetRandomGameEvent();

        Debug.Log("Event is" + currentEvent);
        if (isLevelActive)
        {
            switch (currentEvent)
                    {
                        case GAME_EVENTS.DESTROY_TILES:
                            DestroyRandomTiles();
                            ShowEventMessage("More Lava!");
                            break;
                        case GAME_EVENTS.SPAWN_ENEMY:
                            ShowEventMessage("New Challengers arrive!");
                            SpawnRandomEnemyInRandomTile();
                            break;
                        case GAME_EVENTS.DARKEN_WORLD:
                            ShowEventMessage("It becomes darker!");
                            DarkenWorld();
                            break;
                        case GAME_EVENTS.MOVE_KEY:
                            ShowEventMessage("Kkthxbyebye Key!");
                            MoveKeyToNewPos();
                            break;
                        default:
                            DestroyRandomTiles();
                            break;
                    }
        }
        
    }

    public void ShowEventMessage(string message)
    {
        eventTextMessage.text = message;
        Invoke(nameof(RemoveEventMessage), 2);
    }

    public void RemoveEventMessage()
    {
        eventTextMessage.text = String.Empty;
    }

    private void MoveKeyToNewPos()
    {
        Vector2 randomPositionForKeyMove = _levelSetting.GetRandomSpawnPoint();
        Key theKey = FindObjectOfType<Key>();
        if (theKey != null)
        {
            theKey.MoveToNewPosition(randomPositionForKeyMove);
        }
    }

    private void DarkenWorld()
    {
        Light2D[] lights = FindObjectsOfType<Light2D>();

        foreach (var light in lights)
        {
            if (!light.gameObject.tag.Contains("Player") && !light.gameObject.tag.Contains("GlobalLight") && light.intensity>0.3f)
            {
                light.intensity -= 0.1f;

            }
        }

    }

    private void SpawnRandomEnemyInRandomTile()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        
        // ToDo: think about how to be sure to spawn at a random position 
        //Collider2D CollisionWithEnemy = Physics2D.OverlapCircle(spawnPoint, enemyRadius, LayerMask.GetMask("EnemyLayer"));
        Vector2 position = _levelSetting.GetRandomSpawnPoint();
        Instantiate(enemyPrefabs[index], position, Quaternion.identity);
    }

    private void DestroyRandomTiles()
    {
        BrickTile[] tiles = FindObjectsOfType<BrickTile>();
        List<BrickTile> tilesThatAreNotDestroyed = new List<BrickTile>();
        if (tiles.Length > 0)
        {
            foreach (BrickTile tile in tiles)
            {
                if (tile.currentState == BrickTile.BRICK_STATE.CLOSED)
                {
                    tilesThatAreNotDestroyed.Add(tile);
                }
            }
        }

        if (tilesThatAreNotDestroyed.Count > 0)
        {
            for (int i = 0; i < tileDestroyCount;i++)
            {
                int index = Random.Range(0, tilesThatAreNotDestroyed.Count);
                tilesThatAreNotDestroyed[index].DestroyTile();    
            }

            tileDestroyCount++;

        }
        
        
    }

    public GAME_EVENTS GetRandomGameEvent()
    {
        int maxIndex = 3;
        Key theKey = FindObjectOfType<Key>();
        if (theKey == null)
        {
            maxIndex = 2;
        }
        return (GAME_EVENTS)Random.Range(0, maxIndex);
    }

    public void PrepareRestartLevel()
    {
        // ToDo: Fancy transition 
        fadePanelAnimator.Play("FadeOut");
        Invoke(nameof(RestartLevel),2);
    }

    public void RestartLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string targetLevel)
    {
        // Cleanup
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        isLevelActive = false;

        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
            Destroy(enemy);
        }
        fadePanelAnimator.Play("FadeOut");
        targetLevelScene = targetLevel;
        Invoke(nameof(LoadTargetLevelScene),2);
    }

    public void LoadTargetLevelScene()
    {
        SceneManager.LoadSceneAsync(targetLevelScene);
    }
}

using UnityEngine;

public class BrickTile : MonoBehaviour
{
    public enum BRICK_STATE { OPEN, CLOSED}
    public BRICK_STATE currentState = BRICK_STATE.CLOSED;
    // Start is called before the first frame update
    private Animator anim;
    private AudioSource audioSource;
    public AudioClip brickDestroySound;
    public SpriteRenderer renderer;
    public Sprite openSprite;
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<SpriteRenderer>();
        if (currentState.Equals(BRICK_STATE.OPEN))
        {
            anim.enabled = false;
            audioSource.enabled = false;
            renderer.sprite = openSprite;
        }
    }
    

    public void DestroyTile()
    {
        currentState = BRICK_STATE.OPEN;
        anim.SetBool("IsDestroyed", true);
        if(audioSource!=null && brickDestroySound != null)
        {
            audioSource.PlayOneShot(brickDestroySound);
        }
        // ToDo: Camshake maybe
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && currentState == BRICK_STATE.OPEN)
        {
            collision.gameObject.GetComponent<PlayerTopDownController>().PlayerDieFall();
        }
    }

}

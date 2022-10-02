
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTile : MonoBehaviour
{
    public Tilemap destructibleTilemap;
    public GameObject explosionFx;
    private AudioSource _audioSource;

    public float radius = 2;

    // Start is called before the first frame update
    void Start()
    {
        destructibleTilemap = GetComponent<Tilemap>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Tilemap Collision : " + other.collider.tag);

        if (other.collider.CompareTag("Fireball") || other.collider.CompareTag("Bullet"))
        {
            Vector3 hitPosition = Vector3.zero;
            foreach (ContactPoint2D hit in other.contacts)
            {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

                DestroyTerrain(hitPosition);
                //destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);
            }
        }
    }
    

    public void DestroyTerrain(Vector3 position)
    {
        for (int x = -(int) radius; x < radius; x++)
        {
            for (int y = -(int) radius; y < radius; y++)
            {
                Vector3Int tilePos = destructibleTilemap.WorldToCell(position + new Vector3(x, y, 0));
                if (destructibleTilemap.GetTile(tilePos) != null)
                {
                    DestroyTile(tilePos);
                }
            }
        }
    }

    public void DestroyTile(Vector3Int pos)
    {
        destructibleTilemap.SetTile(pos, null);
        if (explosionFx != null)
        {
            Instantiate(explosionFx, pos, Quaternion.identity);
            _audioSource.PlayOneShot(_audioSource.clip);
        }
    }
}
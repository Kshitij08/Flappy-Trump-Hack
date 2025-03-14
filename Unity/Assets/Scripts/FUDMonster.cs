using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FUDMonster : MonoBehaviour
{
    public float speed = 2f;

    [Header("Speech Bubble Settings")]
    public GameObject[] speechBubbleArray; // Array of exactly 2 GameObjects.

    [Header("Timing Settings")]
    public float minOffInterval = 1f;
    public float maxOffInterval = 3f;
    public float minOnInterval = 1f;
    public float maxOnInterval = 3f;

    [Header("Collision Settings")]
    public float delay = 0.5f; // Delay before enabling the collider.

    // Cached Components
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    public float time;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public GameObject particleEffectGO;

    private void Start()
    {
        // Cache SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Assign NFT material based on current level
        int nftLevel = GameManager.Instance.web3Manager.fUDNFTCurrentLevel - 1;
        spriteRenderer.material = GameManager.Instance.nftMaterialArrayList[nftLevel];

        // Cache and initialize BoxCollider2D
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
            StartCoroutine(EnableCollider());
        }
        else
        {
            Debug.LogWarning($"BoxCollider2D component not found on {gameObject.name}");
        }

        // Start the speech bubble toggling coroutine
        StartCoroutine(ToggleObjectsRoutine());
    }

    private void Update()
    {
        time += Time.deltaTime;

        // Move towards the right boundary
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(12f, transform.position.y, 0f), speed * Time.deltaTime);

        // Destroy when out of bounds
        if (transform.position.x >= 12f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ToggleObjectsRoutine()
    {
        while (true)
        {
            // Ensure both speech bubbles are turned off
            foreach (GameObject obj in speechBubbleArray)
            {
                obj.SetActive(false);
            }

            // Wait for a random off interval
            yield return new WaitForSeconds(Random.Range(minOffInterval, maxOffInterval));

            // Activate a random speech bubble
            speechBubbleArray[Random.Range(0, speechBubbleArray.Length)].SetActive(true);

            // Wait for a random on interval before turning it off again
            yield return new WaitForSeconds(Random.Range(minOnInterval, maxOnInterval));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check collision with the player
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.TakeDamage(1); // Reduce player's health

            // Play impact sound effect
            audioSource.PlayOneShot(audioClip);

            // Disable visuals and collider
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            particleEffectGO.SetActive(true);

            // Destroy child (assumed to be the speech bubble)
            Destroy(transform.GetChild(0).gameObject);

            // Destroy object after 1 second
            Destroy(gameObject, 1f);
        }
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }
}

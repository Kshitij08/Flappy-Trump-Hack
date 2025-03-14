using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    // Movement and physics.
    public float flyForce = 5f;              // Force applied when the player jumps.
    public Rigidbody2D rb;                   // Reference to the player's Rigidbody2D.

    // Trump coin shooting.
    public GameObject trumpCoinPrefab;       // Prefab used to instantiate fireballs.
    public float trumpCoinSpeed = 5f;          // Speed at which fireballs travel.

    // Sprite handling.
    private SpriteRenderer spriteRenderer;   // Cached reference to the player's SpriteRenderer.
    public Sprite baseSprite;                // Default sprite.
    public Sprite jumpSprite;                // Sprite displayed during a jump.

    // Power-up state flags.
    public bool isMelania;
    public bool isElon;
    public bool isLiquidityInjection;
    public bool isPepeBoost;
    public bool isMAGAHat;
    public bool isStableGainShield;
    public bool isPumpAndDump;
    public bool isPumping;
    public bool isDiamondHands;

    // Damage handling.
    public bool canTakeDamage = true;        // Indicates if the player is currently vulnerable.

    // Audio components.
    public AudioSource audioSource;          // Audio source for playing sound effects.
    public AudioClip shootAudioClip;         // Sound for shooting a fireball.
    public AudioClip powerUpAudioClip;       // Sound for power-up activation.
    public AudioClip damageAudioClip;        // Sound played when the player takes damage.

    // Camera shake effect.
    private CameraShake camShake;            // Reference to the CameraShake component.

    // Sprite scaling for effects.
    private Vector3 originalScale;           // The player's original scale.
    public float squeezeDuration = 0.2f;       // Total duration of the squeeze effect.
    public float squeezeFactor = 0.2f;         // Factor by which the sprite scales during the effect.

    // Post-processing for visual feedback.
    public PostProcessVolume volume;         // Reference to the post-processing volume (assigned via Inspector).
    public float flashIntensity = 1f;        // Chromatic aberration intensity when flashing.
    public float normalIntensity = 0f;       // Normal chromatic aberration intensity.
    public float flashDuration = 0.5f;       // Duration of the chromatic aberration flash.
    private ChromaticAberration chromaticAberration;  // Cached reference to the ChromaticAberration effect.

    // Initialization.
    void Start()
    {
        // Cache required components.
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = baseSprite;   // Set the initial sprite.
        camShake = Camera.main.GetComponent<CameraShake>();  // Get CameraShake from the main camera.
        originalScale = transform.localScale; // Store the original scale.

        // Retrieve the ChromaticAberration effect from the post-processing volume.
        if (volume != null && volume.profile.TryGetSettings<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.intensity.Override(normalIntensity);
        }
        else
        {
            Debug.LogWarning("ChromaticAberration component not found in Volume profile.");
        }
    }

    // Called once per frame.
    void Update()
    {
        // Check for input when the game menu is not active.
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isMenuEnabled)
        {
            // Reset velocity and apply upward force for jump.
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * flyForce, ForceMode2D.Impulse);

            // Temporarily switch sprite to the jump sprite.
            StartCoroutine(JumpSpriteChange());

            // If the player is not in DiamondHands state, shoot a fireball and apply a squeeze effect.
            if (!isDiamondHands)
            {
                ShootFireball();
                StartCoroutine(SqueezeSpriteEffect(squeezeDuration, squeezeFactor));
            }
        }
    }

    // Shoots a fireball with behavior modified by active power-ups.
    void ShootFireball()
    {
        // Liquidity Injection: fire 5 fireballs in a spread pattern.
        if (isLiquidityInjection)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject fireball = Instantiate(trumpCoinPrefab, transform.position, Quaternion.identity);
                Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();

                // Determine spread direction.
                Vector2 randomDirection = Quaternion.Euler(0, 0, (i - 2) * 15f) * Vector2.right;
                fireballRb.velocity = randomDirection * trumpCoinSpeed;
                fireball.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
        }
        // MAGA Hat: fire 2 fireballs with vertical offsets.
        else if (isMAGAHat)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPosition = (i == 0)
                    ? new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z)
                    : new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z);
                GameObject fireball = Instantiate(trumpCoinPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
                fireballRb.velocity = Vector2.right * trumpCoinSpeed;
                fireball.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
        }
        // Stable Gain Shield: fire a single modified fireball.
        else if (isStableGainShield)
        {
            GameObject fireball = Instantiate(trumpCoinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
            // Disable reverse behavior for this fireball.
            fireball.GetComponent<TrumpCoin>().canReverse = false;
            FireballDirection(fireballRb);
            FireballScale(fireball);
        }
        // Default shooting behavior.
        else
        {
            GameObject fireball = Instantiate(trumpCoinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
            FireballDirection(fireballRb);
            FireballScale(fireball);
        }

        // Play shooting sound effect.
        audioSource.PlayOneShot(shootAudioClip);
    }

    // Determines the velocity of the fireball based on player power-up states.
    void FireballDirection(Rigidbody2D fireballRb)
    {
        // Elon state (with no Liquidity Injection) applies a random angular deviation.
        if (isElon && !isLiquidityInjection)
        {
            if (isPumpAndDump)
            {
                Vector2 randomDirection = Quaternion.Euler(0, 0, Random.Range(-30f, 30f)) * Vector2.right;
                fireballRb.velocity = randomDirection * trumpCoinSpeed * (isPumping ? 2f : 0.5f);
            }
            else
            {
                Vector2 randomDirection = Quaternion.Euler(0, 0, Random.Range(-30f, 30f)) * Vector2.right;
                fireballRb.velocity = randomDirection * trumpCoinSpeed;
            }
        }
        // Pump and Dump without Elon: fire straight right with speed adjustments.
        else if (isPumpAndDump)
        {
            Vector2 direction = Vector2.right;
            fireballRb.velocity = direction * trumpCoinSpeed * (isPumping ? 2f : 0.5f);
        }
        // Default fireball direction.
        else
        {
            fireballRb.velocity = Vector2.right * trumpCoinSpeed;
        }
    }

    // Sets the scale of the fireball based on active power-ups.
    void FireballScale(GameObject fireball)
    {
        if (isMelania && !isLiquidityInjection)
        {
            fireball.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        }
        else if (isPepeBoost)
        {
            fireball.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            fireball.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
    }

    // Applies a squeeze effect to the player's sprite for visual feedback.
    IEnumerator SqueezeSpriteEffect(float squeezeDuration, float squeezeFactor)
    {
        float halfDuration = squeezeDuration / 2f;
        // Calculate target scale during squeeze.
        Vector3 squeezedScale = new Vector3(
            originalScale.x * (1 - squeezeFactor),
            originalScale.y * (1 + squeezeFactor),
            originalScale.z
        );

        float timer = 0f;
        // Lerp from original scale to squeezed scale.
        while (timer < halfDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squeezedScale, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = squeezedScale;

        // Lerp back from squeezed scale to original scale.
        timer = 0f;
        while (timer < halfDuration)
        {
            transform.localScale = Vector3.Lerp(squeezedScale, originalScale, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    // Handles damage taken by the player.
    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage");
        if (canTakeDamage)
        {
            // If not invulnerable via DiamondHands, reduce health.
            if (!isDiamondHands)
            {
                GameManager.Instance.health -= damage;
                if (GameManager.Instance.health <= 0)
                {
                    // Fatal damage: play sound, update UI, and disable player components.
                    audioSource.PlayOneShot(damageAudioClip);
                    GameManager.Instance.UpdateHealthBar();
                    spriteRenderer.enabled = false;
                    GetComponent<BoxCollider2D>().enabled = false;
                    rb.isKinematic = true;
                    StartCoroutine(DisablePlayerGO());
                    DestroyTrumpCoins();
                    GameManager.Instance.menu.ShowSinglePlayerGameOverOverlay();
                    GameManager.Instance.AnalyticsScore();
                }
                else
                {
                    // Non-fatal damage: play sound and update health bar.
                    audioSource.PlayOneShot(damageAudioClip);
                    GameManager.Instance.UpdateHealthBar();
                }
            }

            // Apply camera shake if available.
            if (camShake != null)
            {
                StartCoroutine(camShake.Shake(0.3f, 0.2f));
            }

            // Flash visual damage feedback.
            FlashChromaticAberration();
            StartCoroutine(DamageFlash());
        }
    }

    // Disables the player's GameObject after a short delay.
    IEnumerator DisablePlayerGO()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    // Apply damage when colliding with other objects.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TakeDamage(1);
    }

    // Temporarily switches the sprite to the jump sprite.
    IEnumerator JumpSpriteChange()
    {
        spriteRenderer.sprite = jumpSprite;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.sprite = baseSprite;
    }

    // Reset coroutines for various power-ups.
    public IEnumerator ResetLiquidityInjection(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isLiquidityInjection = false;
    }

    public IEnumerator ResetPepeBoost(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isPepeBoost = false;
    }

    public IEnumerator ResetMAGAHat(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isMAGAHat = false;
    }

    public IEnumerator ResetStableGainShield(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isStableGainShield = false;
    }

    public IEnumerator ResetPumpAndDump(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isPumping = false;
        yield return new WaitForSeconds(resetTime);
        isPumpAndDump = false;
    }

    public IEnumerator ResetDiamondHands(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        isDiamondHands = false;
    }

    // Destroys all fireball objects tagged "TrumpCoin".
    void DestroyTrumpCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("TrumpCoin");
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
    }

    // Coroutine to flash the player's sprite red, indicating damage and temporary invulnerability.
    IEnumerator DamageFlash()
    {
        canTakeDamage = false;           // Disable damage during the flash.
        float flashDur = 1f;             // Total duration of the flash effect.
        float flashInterval = 0.1f;      // Interval between color toggles.
        float timer = 0f;
        Color originalColor = spriteRenderer.color;
        Color flashColor = Color.red;

        while (timer < flashDur)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2;
        }
        canTakeDamage = true;            // Re-enable damage.
    }

    // Initiates a chromatic aberration flash effect for damage feedback.
    public void FlashChromaticAberration()
    {
        if (chromaticAberration != null)
        {
            // Stop any current flash coroutine and start a new one.
            StopAllCoroutines();
            StartCoroutine(DoFlash());
        }
    }

    // Coroutine to handle the chromatic aberration flash effect.
    IEnumerator DoFlash()
    {
        chromaticAberration.intensity.Override(flashIntensity);
        yield return new WaitForSeconds(flashDuration);
        chromaticAberration.intensity.Override(normalIntensity);
    }
}

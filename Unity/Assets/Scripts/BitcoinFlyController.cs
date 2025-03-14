using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitcoinFlyController : MonoBehaviour
{
    // --- State Machine ---
    private enum State { Flying, LaserExtending, LaserHold }
    private State currentState = State.Flying;
    private float stateStartTime; // Timer for the current state

    // --- Flight Parameters ---
    public float flightDuration = 5f; // Time before firing laser
    public float hoverAmplitude = 0.5f, hoverFrequency = 1f; // Vertical oscillation
    public float horizontalAmplitude = 1f, horizontalFrequency = 1f; // Horizontal oscillation
    private Vector3 startPos; // Base position for movement

    // --- Movement Boundaries ---
    public float minX = 1.5f, maxX = 8.5f, minY = -5f, maxY = 3.75f;
    private float phaseX = 0f, phaseY = 0f; // Offsets for sine movement

    // --- Laser Parameters ---
    public LineRenderer laserLineRenderer;
    public Transform eyeTransform; // Laser origin point
    public float targetLaserLength = 15f, laserExtendSpeed = 5f, laserHoldDuration = 1f;
    private float currentLaserLength = 0f;
    public EdgeCollider2D laserCollider;

    // --- Collider Handling ---
    public float delay = 0.5f;
    private BoxCollider2D boxCollider2D;

    // --- Health & Effects ---
    public int health = 4;
    public GameObject[] healthGO;
    public GameObject particleEffectGO;

    // --- Audio ---
    public AudioSource audioSource;
    public AudioClip audioClip;

    // --- Time Management ---
    public float localTime = 0;

    void Start()
    {
        // Assign material based on NFT level
        int nftLevel = GameManager.Instance.web3Manager.bitcoinFlyNFTCurrentLevel - 1;
        GetComponent<SpriteRenderer>().material = GameManager.Instance.nftMaterialArrayList[nftLevel];

        // Cache BoxCollider2D component
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false; // Disable initially
            StartCoroutine(EnableCollider()); // Enable after delay
        }
        else
        {
            Debug.LogWarning($"BoxCollider2D component not found on {gameObject.name}");
        }

        // Initialize movement & state
        startPos = transform.position;
        stateStartTime = localTime;
        currentState = State.Flying;

        // Setup laser
        if (laserLineRenderer != null)
        {
            laserLineRenderer.positionCount = 2;
            laserLineRenderer.enabled = false; // Laser starts off
        }
        else
        {
            Debug.LogError($"Laser LineRenderer is not assigned on {gameObject.name}");
        }
    }

    void Update()
    {
        localTime += Time.deltaTime;

        switch (currentState)
        {
            case State.Flying:
                HandleFlyingState();
                break;

            case State.LaserExtending:
                HandleLaserExtending();
                break;

            case State.LaserHold:
                HandleLaserHold();
                break;
        }
    }

    void HandleFlyingState()
    {
        float elapsed = localTime - stateStartTime;

        // Calculate oscillatory movement
        float computedX = startPos.x + Mathf.Sin(elapsed * horizontalFrequency + phaseX) * horizontalAmplitude;
        float computedY = startPos.y + Mathf.Sin(elapsed * hoverFrequency + phaseY) * hoverAmplitude;

        // Boundary corrections
        if (computedX < minX) { AdjustBoundary(ref computedX, ref startPos.x, minX, ref phaseX, 0.1f); }
        if (computedX > maxX) { AdjustBoundary(ref computedX, ref startPos.x, maxX, ref phaseX, -0.1f); }
        if (computedY < minY) { AdjustBoundary(ref computedY, ref startPos.y, minY, ref phaseY, 0.1f); }
        if (computedY > maxY) { AdjustBoundary(ref computedY, ref startPos.y, maxY, ref phaseY, -0.1f); }

        transform.position = new Vector3(computedX, computedY, transform.position.z);

        // Transition to laser state
        if (elapsed >= flightDuration)
        {
            currentState = State.LaserExtending;
            laserLineRenderer.enabled = true;
        }
    }

    void HandleLaserExtending()
    {
        if (currentLaserLength < targetLaserLength)
        {
            currentLaserLength = Mathf.Min(currentLaserLength + laserExtendSpeed * Time.deltaTime, targetLaserLength);
        }
        else
        {
            currentState = State.LaserHold;
            stateStartTime = localTime;
        }

        UpdateLaser();
    }

    void HandleLaserHold()
    {
        if (localTime - stateStartTime >= laserHoldDuration)
        {
            laserLineRenderer.enabled = false;
            ShrinkLaserCollider();
            ResetLaser();
        }
    }

    void UpdateLaser()
    {
        if (eyeTransform != null && laserLineRenderer != null)
        {
            Vector3 startPoint = eyeTransform.position;
            Vector3 endPoint = startPoint + Vector3.left * currentLaserLength;
            laserLineRenderer.SetPositions(new Vector3[] { startPoint, endPoint });

            if (laserCollider != null)
            {
                Vector2 localStart = laserCollider.transform.InverseTransformPoint(startPoint);
                Vector2 localEnd = laserCollider.transform.InverseTransformPoint(endPoint);
                laserCollider.points = new Vector2[] { localStart, localEnd };
            }
        }
    }

    void ShrinkLaserCollider()
    {
        if (laserCollider != null)
        {
            Vector2 localPoint = laserCollider.transform.InverseTransformPoint(eyeTransform.position);
            laserCollider.points = new Vector2[] { localPoint, localPoint };
        }
    }

    void ResetLaser()
    {
        currentLaserLength = 0f;
        currentState = State.Flying;
        stateStartTime = localTime;
        startPos = transform.position;
    }

    void AdjustBoundary(ref float value, ref float startValue, float boundary, ref float phase, float phaseAdjustment)
    {
        float diff = Mathf.Abs(boundary - value);
        startValue += diff * Mathf.Sign(boundary - value);
        phase += phaseAdjustment;
        value = boundary;
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }

    public void TakeDamage()
    {
        if (health <= 0) return;

        health -= 1;
        int index = Mathf.Clamp(health, 0, 3);
        healthGO[index].GetComponent<SpriteRenderer>().color = new Color(0.68f, 0.42f, 0.4f, 1f);
        audioSource.PlayOneShot(audioClip);

        if (health == 0)
        {
            int score = GameManager.Instance.isBullMarket ? 50 : 250;
            GameManager.Instance.UpdateScore(score * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.bitcoinFlyNFTCurrentLevel - 1]);

            Destroy(transform.GetChild(0).gameObject);
            GetComponent<SpriteRenderer>().enabled = false;
            boxCollider2D.enabled = false;
            particleEffectGO.SetActive(true);
            GameManager.Instance.web3Manager.bitcoinFlyNFTCount += 1;

            Destroy(gameObject, 1f);
        }
    }
}

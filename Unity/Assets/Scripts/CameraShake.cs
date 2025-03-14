using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Call this method to shake the camera.
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition; // Store original position
        float elapsed = 0f;

        // Continue shaking while within the duration and spawning is allowed.
        while (elapsed < duration && GameManager.Instance.enemyWaveSpawner.canSapwn)
        {
            // Generate random offset within a unit circle for smooth shake effect.
            Vector2 randomOffset = Random.insideUnitCircle * magnitude;
            transform.localPosition = originalPos + new Vector3(randomOffset.x, randomOffset.y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after shake ends.
        transform.localPosition = originalPos;
    }
}

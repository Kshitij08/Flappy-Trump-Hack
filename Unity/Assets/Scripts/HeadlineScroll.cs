using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HeadlineScroll : MonoBehaviour
{
    [Header("Ticker Settings")]
    public RectTransform tickerContainer; // The scrolling area container
    public TMP_Text[] textBoxes; // Three TMP text elements that display headlines
    public float scrollSpeed = 100f; // Scrolling speed in pixels per second

    [Header("Headline Settings")]
    public string[] headlines; // Array of headlines to cycle through

    private float leftBoundary;  // Left limit of the ticker container
    private float rightBoundary; // Right limit of the ticker container
    public float gapWidth = 5f;  // Spacing between headlines

    void Start()
    {
        // Ensure necessary references are assigned
        if (!ValidateSetup()) return;

        // Get the container's rect boundaries
        Rect containerRect = tickerContainer.rect;
        leftBoundary = containerRect.xMin;
        rightBoundary = containerRect.xMax;

        // Position the text boxes sequentially, starting from the right edge
        PositionTextBoxes();
    }

    void Update()
    {
        // Move each text box to the left
        foreach (TMP_Text tb in textBoxes)
        {
            tb.rectTransform.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
        }

        // Check if any text box has completely moved out of view
        RecycleTextBoxes();
    }

    /// <summary>
    /// Validates required references and configurations.
    /// </summary>
    /// <returns>Returns true if all conditions are met, otherwise disables the script.</returns>
    private bool ValidateSetup()
    {
        if (tickerContainer == null)
        {
            Debug.LogError("Ticker container is not assigned!");
            enabled = false;
            return false;
        }
        if (textBoxes == null || textBoxes.Length != 3)
        {
            Debug.LogError("Please assign exactly 3 TMP_Text elements to textBoxes.");
            enabled = false;
            return false;
        }
        if (headlines == null || headlines.Length == 0)
        {
            Debug.LogError("No headlines provided!");
            enabled = false;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Positions text boxes sequentially along the x-axis.
    /// </summary>
    private void PositionTextBoxes()
    {
        float currentX = rightBoundary; // Start at the right edge

        for (int i = 0; i < textBoxes.Length; i++)
        {
            TMP_Text tb = textBoxes[i];

            // Assign a random headline
            tb.text = headlines[Random.Range(0, headlines.Length)];
            tb.ForceMeshUpdate();

            // Get text width for accurate positioning
            float textWidth = tb.GetPreferredValues(tb.text).x;

            // Position the text with an appropriate gap
            tb.rectTransform.anchoredPosition = new Vector2(currentX + textWidth / 2f, tb.rectTransform.anchoredPosition.y);

            // Update position for the next text box
            currentX += textWidth + gapWidth;
        }
    }

    /// <summary>
    /// Recycles text boxes when they scroll out of view.
    /// </summary>
    private void RecycleTextBoxes()
    {
        foreach (TMP_Text tb in textBoxes)
        {
            tb.ForceMeshUpdate();
            float textWidth = tb.GetPreferredValues(tb.text).x;

            // Check if the right edge of the text has moved past the left boundary
            if (tb.rectTransform.anchoredPosition.x + textWidth / 2f < leftBoundary)
            {
                // Assign a new random headline
                tb.text = headlines[Random.Range(0, headlines.Length)];
                tb.ForceMeshUpdate();
                float newWidth = tb.GetPreferredValues(tb.text).x;

                // Find the rightmost text box to determine the new position
                float maxRight = rightBoundary;
                foreach (TMP_Text other in textBoxes)
                {
                    if (other == tb) continue;

                    other.ForceMeshUpdate();
                    float otherWidth = other.GetPreferredValues(other.text).x;
                    float otherRightEdge = other.rectTransform.anchoredPosition.x + otherWidth / 2f;

                    if (otherRightEdge > maxRight)
                        maxRight = otherRightEdge;
                }

                // Position the recycled text box to the right of the rightmost text
                tb.rectTransform.anchoredPosition = new Vector2(maxRight + gapWidth + newWidth / 2f, tb.rectTransform.anchoredPosition.y);
            }
        }
    }
}

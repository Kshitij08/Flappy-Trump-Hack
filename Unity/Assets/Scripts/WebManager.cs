using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages web interactions such as retrieving/updating user scores, fetching the leaderboard,
/// and handling JSON metadata uploads/updates for NFTs.
/// </summary>
public class WebManager : MonoBehaviour
{
    // Base URLs for PHP endpoints on the backend server.
    private string getUserScoreUrl = "https://plum-coyote-976718.hostingersite.com/FlappyTrumpBackend/GetOrCreateUserScore.php";
    private string updateUserScoreUrl = "https://plum-coyote-976718.hostingersite.com/FlappyTrumpBackend/UpdateUserScore.php";
    private string getLeaderboardUrl = "https://plum-coyote-976718.hostingersite.com/FlappyTrumpBackend/GetLeaderboard.php";
    // URL for uploading JSON metadata.
    private string uploadUrl = "https://plum-coyote-976718.hostingersite.com/MetadataUpload.php";
    // URL for updating existing metadata on the server.
    private string updateUrl = "https://plum-coyote-976718.hostingersite.com/MetadataUpdate.php";

    /// <summary>
    /// Retrieves the user's score from the server or creates a new record if none exists.
    /// </summary>
    public IEnumerator GetOrCreateUserScoreCoroutine(string walletAddress)
    {
        // Prepare form data with the wallet address.
        WWWForm form = new WWWForm();
        form.AddField("walletAddress", walletAddress);

        using (UnityWebRequest www = UnityWebRequest.Post(getUserScoreUrl, form))
        {
            yield return www.SendWebRequest();

            // Check for connection or protocol errors.
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error in GetOrCreateUserScore: " + www.error);
                // Optionally, notify the UI about the error.
            }
            else
            {
                // Retrieve and log the server response.
                string responseText = www.downloadHandler.text;
                Debug.Log("Response from GetOrCreateUserScore: " + responseText);

                // Deserialize the JSON response.
                UserScoreResponse responseData = JsonUtility.FromJson<UserScoreResponse>(responseText);
                if (responseData.status == "success")
                {
                    // Update the GameManager with the retrieved wallet and score information.
                    GameManager.Instance.serverWalletAddress = responseData.walletAddress;
                    GameManager.Instance.serverTotalScore = responseData.score;
                    Debug.Log("Wallet: " + responseData.walletAddress + " | Score: " + responseData.score);
                }
                else
                {
                    Debug.LogError("GetOrCreateUserScore failed: " + responseData.message);
                }
            }
        }
    }

    /// <summary>
    /// Updates the user's score on the server (e.g., when the game is over).
    /// </summary>
    public IEnumerator UpdateUserScoreCoroutine(string walletAddress, int newScore)
    {
        // Prepare form data with wallet address and new score.
        WWWForm form = new WWWForm();
        form.AddField("walletAddress", walletAddress);
        form.AddField("score", newScore);

        using (UnityWebRequest www = UnityWebRequest.Post(updateUserScoreUrl, form))
        {
            yield return www.SendWebRequest();

            // Check for connection or protocol errors.
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error in UpdateUserScore: " + www.error);
                // Optionally, display an error to the player.
            }
            else
            {
                // Process the server response.
                string responseText = www.downloadHandler.text;
                Debug.Log("Response from UpdateUserScore: " + responseText);

                // Deserialize the JSON response.
                UpdateScoreResponse responseData = JsonUtility.FromJson<UpdateScoreResponse>(responseText);
                if (responseData.status == "success")
                {
                    Debug.Log("Score updated successfully! New Score: " + responseData.score);
                    // Update any local score variables or UI elements here.
                }
                else
                {
                    Debug.LogError("UpdateUserScore failed: " + responseData.message);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the leaderboard data (top 10 entries) from the server and the current user's rank.
    /// </summary>
    public IEnumerator GetLeaderboardCoroutine(string walletAddress)
    {
        // Prepare form data with the current user's wallet address.
        WWWForm form = new WWWForm();
        form.AddField("walletAddress", walletAddress);

        using (UnityWebRequest www = UnityWebRequest.Post(getLeaderboardUrl, form))
        {
            yield return www.SendWebRequest();

            // Check for errors in the web request.
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error in GetLeaderboard: " + www.error);
                // Optionally, show an error popup.
            }
            else
            {
                // Get and log the server response.
                string responseText = www.downloadHandler.text;
                Debug.Log("Response from GetLeaderboard: " + responseText);

                // Deserialize the JSON into a LeaderboardResponse object.
                LeaderboardResponse responseData = JsonUtility.FromJson<LeaderboardResponse>(responseText);
                if (responseData.status == "success")
                {
                    // Loop through the leaderboard entries (up to 10) and update GameManager arrays.
                    for (int i = 0; i < responseData.leaderboard.Count && i < 10; i++)
                    {
                        LeaderboardEntry entry = responseData.leaderboard[i];
                        GameManager.Instance.serverLeaderboardWalletAddresseArray[i] = entry.walletAddress;
                        GameManager.Instance.serverLeaderboardScoreArray[i] = entry.score;
                        Debug.Log("Wallet: " + entry.walletAddress + " | Score: " + entry.score);
                    }
                    // Update the current user's rank.
                    GameManager.Instance.serverUserRank = responseData.userRank;
                    Debug.Log("Your Rank: " + responseData.userRank);
                    // Optionally, update the UI with the leaderboard data.
                }
                else
                {
                    Debug.LogError("GetLeaderboard failed: " + responseData.message);
                }
            }
        }
    }

    /// <summary>
    /// Uploads NFT metadata JSON to the server.
    /// </summary>
    /// <param name="jsonData">The JSON metadata string.</param>
    /// <param name="customString">A custom string to send (e.g., NFT name).</param>
    /// <param name="onComplete">Callback to receive the file URL returned by the server.</param>
    public IEnumerator UploadJson(string jsonData, string customString, Action<string> onComplete)
    {
        // Prepare the form with API key, metadata, and custom field.
        WWWForm form = new WWWForm();
        form.AddField("apiKey", "ZOT9It6g4dk3eNpfww5YUlP1KpJJqMXTWcrDiQpRsTlKArw3pUzmDNS0EWaPFeICaWIn8y1pCBAYpKoe4kdOlUTUVTvX6kkX0Clj3WX4qoeJ8VzHI1vB9vZZiukr3Q4f");
        form.AddField("metadata", jsonData);
        form.AddField("custom", customString);

        using (UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form))
        {
            yield return www.SendWebRequest();

            // Check if upload failed.
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Upload failed: " + www.error);
                onComplete?.Invoke(null);
            }
            else
            {
                // Get the file URL returned by the server.
                string fileUrl = www.downloadHandler.text;
                Debug.Log("Upload complete! Server response:\n" + fileUrl);
                onComplete?.Invoke(fileUrl);
            }
        }
    }

    /// <summary>
    /// Updates existing NFT metadata on the server.
    /// </summary>
    /// <param name="fileUrl">URL of the metadata file to update.</param>
    /// <param name="newJsonData">New JSON metadata string.</param>
    public IEnumerator UpdateMetadata(string fileUrl, string newJsonData)
    {
        // Prepare form data with API key, file URL, and new metadata.
        WWWForm form = new WWWForm();
        form.AddField("apiKey", "ZOT9It6g4dk3eNpfww5YUlP1KpJJqMXTWcrDiQpRsTlKArw3pUzmDNS0EWaPFeICaWIn8y1pCBAYpKoe4kdOlUTUVTvX6kkX0Clj3WX4qoeJ8VzHI1vB9vZZiukr3Q4f");
        form.AddField("fileUrl", fileUrl);
        form.AddField("metadata", newJsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(updateUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Update failed: " + www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                Debug.Log("Update complete! Server response:\n" + response);
            }
        }
    }
}

#region JSON Response Classes

[System.Serializable]
public class UserScoreResponse
{
    public string status;       // "success" or "error"
    public string message;      // Error or success message
    public string walletAddress;
    public int score;
}

[System.Serializable]
public class UpdateScoreResponse
{
    public string status;       // "success" or "error"
    public string message;      // Message from the server
    public string walletAddress;
    public int score;
}

[System.Serializable]
public class LeaderboardResponse
{
    public string status;       // "success" or "error"
    public string message;      // Message from the server
    public List<LeaderboardEntry> leaderboard;  // List of top 10 entries
    public int userRank;        // Current user's rank
}

[System.Serializable]
public class LeaderboardEntry
{
    public string walletAddress;
    public int score;
}

#endregion

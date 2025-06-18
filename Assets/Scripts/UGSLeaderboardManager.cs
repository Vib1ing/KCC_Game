using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UGSLeaderboardManager : MonoBehaviour
{
    public static UGSLeaderboardManager Instance { get; private set; }

    public const string leaderboardId = "SpeedrunLeaderboard"; // Replace with your actual Leaderboard ID

    public static string leaderboardTopTen = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persists across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public async void SubmitScore(float timeInSeconds)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Not signed in. Score not submitted.");
            return;
        }

        // Explicit cast to float to avoid CS0266 error
        long score = Mathf.RoundToInt((float)(timeInSeconds * 1000)); // Convert to milliseconds
        string displayName = PlayerPrefs.GetString("PlayerName", "Guest");

        try
        {
            // Update the player's display name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName);
            var metadata = new Dictionary<string, object>
            {
                { "realName", displayName }
            };

            // Submit the score
            await LeaderboardsService.Instance.AddPlayerScoreAsync(
                leaderboardId,
                score,
                new AddPlayerScoreOptions
                {
                    Metadata = metadata
                }
            );

            Debug.Log($"Score submitted: {score}ms for player {displayName}");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public static async Task RefreshLeaderboard(int rowCount = 10)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Not signed in. Cannot load leaderboard.");
        }

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions
                {
                    Limit = rowCount,
                    IncludeMetadata = true
                }
            );

            string text = "";
            int i = 1;
            foreach (var entry in scoresResponse.Results)
            {
                string name;
                string realName = GetDisplayNameFromMetadata(entry.Metadata);
                if (!string.IsNullOrWhiteSpace(realName)) name = realName;
                else if (!string.IsNullOrWhiteSpace(entry.PlayerName)) name = entry.PlayerName;
                else name = "Guest";
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(entry.Score);
                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                    timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                string row = $"{entry.Rank + 1}. {name} - {formattedTime}\n";
                bool isPersonal =
                    entry.PlayerId == AuthenticationService.Instance.PlayerId ||
                    (
                        !string.IsNullOrWhiteSpace(realName) &&
                        realName == PlayerPrefs.GetString("PlayerName", "Guest") &&
                        realName != "Guest"
                    ) ||
                    (
                        !string.IsNullOrWhiteSpace(entry.PlayerName) &&
                        entry.PlayerName == AuthenticationService.Instance.PlayerName &&
                        entry.PlayerName != "Guest"
                    );
                if (isPersonal)
                {
                    row = BeautifyPersonalRow(row);
                }
                text += row;
                i++;
            }

            for (; i <= rowCount; i++)
            {
                text += $"{i}. ---\n";
            }

            leaderboardTopTen = text;
            Debug.Log("Leaderboard loaded successfully:\n" + leaderboardTopTen);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load leaderboard: " + e.Message);
        }
    }

    private static string BeautifyPersonalRow(string text)
    {
        return $"<b><color=#66CCFF>{text}</color></b>";
    }

    private static string GetDisplayNameFromMetadata(string metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
            return null;

        try
        {
            var meta = JsonUtility.FromJson<LeaderboardMetadata>(metadataJson);
            return string.IsNullOrWhiteSpace(meta.realName) ? null : meta.realName;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to parse metadata: {e.Message}");
            return null;
        }
    }

    // This method can be called from any other script (e.g., Circle.cs)
    public static void SubmitPlayerScore(float finalTime)
    {
        if (Instance != null)
        {
            Instance.SubmitScore(finalTime);
        }
        else
        {
            Debug.LogWarning("UGSLeaderboardManager instance not found.");
        }
    }
}

[Serializable]
class LeaderboardMetadata
{
    public string realName;
}
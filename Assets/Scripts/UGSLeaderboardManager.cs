using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System;
using System.Collections.Generic;

public class UGSLeaderboardManager : MonoBehaviour
{
    public static UGSLeaderboardManager Instance { get; private set; }

    public string leaderboardId = "SpeedrunLeaderboard"; // Replace with your actual Leaderboard ID

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
            var metadata = new Dictionary<string, string>
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

    public async void GetLeaderboard()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Not signed in. Cannot load leaderboard.");
        }

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions { Limit = 10 }
            );

            string text = "";

            foreach (var entry in scoresResponse.Results)
            {
                float timeInSeconds = (float)entry.Score / 1000f;
                string name = string.IsNullOrEmpty(entry.PlayerName) ? "Guest" : entry.PlayerName;
                text += $"{entry.Rank + 1}. {name} - {timeInSeconds:0.000}s\n";
            }

            leaderboardTopTen = text;
            Debug.Log("Leaderboard loaded successfully:\n" + leaderboardTopTen);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load leaderboard: " + e.Message);
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




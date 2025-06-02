using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using System;

public class UGSLeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    public string leaderboardId = "SpeedrunLeaderboard"; // Replace with your actual Leaderboard ID

    [Header("UI References")]
    public TMP_InputField playerNameInput;
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;

    // Singleton instance
    private static UGSLeaderboardManager instance;
    public static UGSLeaderboardManager Instance => instance;

    public Canvas canvas;

    void Start()
    {
        canvas.enabled = false;
    }

    async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeUGSAsync();
    }

    private async Task InitializeUGSAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("UGS Initialization Failed: " + e.Message);
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
        string displayName = string.IsNullOrWhiteSpace(playerNameInput.text) ? "Guest" : playerNameInput.text;

        try
        {
            // Update the player's display name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName);

            // Submit the score
            await LeaderboardsService.Instance.AddPlayerScoreAsync(
                leaderboardId,
                score
            );

            Debug.Log($"Score submitted: {score}ms for player {displayName}");

            // Automatically load leaderboard after submission
            LoadLeaderboard();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public async void LoadLeaderboard()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("Not signed in. Cannot load leaderboard.");
            return;
        }

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions { Limit = 10 }
            );

            // Clear previous entries
            foreach (Transform child in leaderboardContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var entry in scoresResponse.Results)
            {
                var item = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
                float timeInSeconds = (float)entry.Score / 1000f;
                string name = string.IsNullOrEmpty(entry.PlayerName) ? "Guest" : entry.PlayerName;
                item.GetComponent<TMP_Text>().text = $"{entry.Rank + 1}. {name} - {timeInSeconds:0.000}s";
            }
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




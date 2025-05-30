using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    DatabaseReference dbReference;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContainer;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }
        });
    }

    public void SubmitScore(string playerName, float time)
    {
        string key = dbReference.Child("leaderboard").Push().Key;
        LeaderboardEntry entry = new LeaderboardEntry(playerName, time);
        string json = JsonUtility.ToJson(entry);
        dbReference.Child("leaderboard").Child(key).SetRawJsonValueAsync(json);
    }

    public void LoadLeaderboard()
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject); // Clear old entries
        }

        dbReference.Child("leaderboard").OrderByChild("time").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    var entry = JsonUtility.FromJson<LeaderboardEntry>(child.GetRawJsonValue());
                    var go = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
                    go.GetComponent<TMP_Text>().text = $"{entry.playerName}: {entry.time:0.000}s";
                }
            }
        });
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public float time;

        public LeaderboardEntry(string name, float t)
        {
            playerName = name;
            time = t;
        }
    }
}
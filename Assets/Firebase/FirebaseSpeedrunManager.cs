using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Linq;

[Serializable]
public class FirebaseEntry
{
    public string playerName;
    public float time;

    public FirebaseEntry() {} // Firebase requires empty constructor

    public FirebaseEntry(string name, float time)
    {
        this.playerName = name;
        this.time = time;
    }
}

public class FirebaseSpeedrunManager : MonoBehaviour
{
    private DatabaseReference dbRef;

    public List<FirebaseEntry> leaderboardEntries = new List<FirebaseEntry>();

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SubmitEntry(string playerName, float time)
    {
        string key = dbRef.Child("leaderboard").Push().Key;
        FirebaseEntry newEntry = new FirebaseEntry(playerName, time);
        string json = JsonUtility.ToJson(newEntry);

        dbRef.Child("leaderboard").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Entry submitted!");
            }
        });
    }

    public void LoadLeaderboard(Action<List<FirebaseEntry>> onLoaded)
    {
        dbRef.Child("leaderboard").OrderByChild("time").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                leaderboardEntries.Clear();
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot child in snapshot.Children)
                {
                    FirebaseEntry entry = JsonUtility.FromJson<FirebaseEntry>(child.GetRawJsonValue());
                    leaderboardEntries.Add(entry);
                }

                onLoaded?.Invoke(leaderboardEntries);
            }
        });
    }
}
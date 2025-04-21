using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedrunLeaderboardUI : MonoBehaviour
{
    public FirebaseSpeedrunManager firebaseManager;
    public TextMeshProUGUI leaderboardText;

    void Start()
    {
        firebaseManager.LoadLeaderboard(UpdateLeaderboardUI);
    }

    void UpdateLeaderboardUI(List<FirebaseEntry> entries)
    {
        leaderboardText.text = "Leaderboard\n";
        int rank = 1;

        foreach (var entry in entries)
        {
            leaderboardText.text += $"{rank++}. {entry.playerName} - {FormatTime(entry.time)}\n";
        }
    }

    string FormatTime(float time)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);
        return t.ToString(@"mm\:ss\:fff");
    }
}
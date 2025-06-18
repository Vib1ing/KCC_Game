using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class LeaderboardMenu : MonoBehaviour
{

    public TMP_Text leaderboardText;

    void Awake()
    {
        RequestRefresh();
    }

    void Start() {
        RequestRefresh();
    }

    public void RequestRefresh()
    {
        _ = RefreshCurrentLeaderboard();
    }

    public async Task RefreshCurrentLeaderboard()
    {
        leaderboardText.text = "Loading...";
        await UGSLeaderboardManager.RefreshLeaderboard();
        leaderboardText.text = UGSLeaderboardManager.leaderboardTopTen;
    }
}

using TMPro;
using UnityEngine;

public class ProfanityMenu : MonoBehaviour
{
    public GameObject ProfanityPopup;
    public TMP_Text ProfaneName;
    private string profaneName;

    public static ProfanityMenu Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(string censoredName)
    {
        profaneName = censoredName;
        ProfaneName.text = $"Censored Name:\n{censoredName}";
        ProfanityPopup.SetActive(true);
    }

    public void OnChangeName()
    {
        ProfanityPopup.SetActive(false);
    }

    public void OnUseCensored()
    {
        ProfanityPopup.SetActive(false);
        PlayerNameManager.Instance.updateName(profaneName);
    }
}

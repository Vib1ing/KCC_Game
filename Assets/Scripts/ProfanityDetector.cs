using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ProfanityDetector : MonoBehaviour
{

    public static async Task<string> Censor(string name)
    {
        try
        {
            string url = "https://www.purgomalum.com/service/json?text=" + UnityWebRequest.EscapeURL(name);
            UnityWebRequest request = UnityWebRequest.Get(url);

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Purgomalum request failed: " + request.error);
                return null;
            }

            var json = request.downloadHandler.text;
            APIResponse response = JsonUtility.FromJson<APIResponse>(json);
            return response.result;
        }
        catch (Exception)
        {
            Debug.LogWarning("Purgomalum request failed, returning original name");
            return name;
        }
    }
}

[Serializable]
class APIResponse
{
    public string result;
}
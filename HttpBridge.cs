using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum GameState
{
    InProgress,
    Draw,
    PlayerXWon,
    PlayerOWon
}

public class Turn
{
    public int playerId;
    public int selectedField;

    public Turn(int playerId, int selectedField)
    {
        this.playerId = playerId;
        this.selectedField = selectedField;
    }
}
public class HttpBridge : MonoBehaviour
{
    [SerializeField]
    private string BaseUrl = "http://192.168.50.107:8080";
    
    public void MakeTurn(Turn data, Action<GameState> callback)
    {
        Post($"/MakeTurn?playerId={data.playerId}&selectedField={data.selectedField}", (handler) =>
        {
            Enum.TryParse<GameState>(handler.text, out var state);
            callback(state);
        }); 
    }
    
    public void GetFields(Action<int[]> callback)
    {
        Get("/GetField", (handler) =>
        {
            var array = JsonConvert.DeserializeObject<int[]>(handler.text);
            callback(array); 
        });
    }
    
    public void CreateGame(Action callback)
    {
        Post("/CreateGame", (handler) => callback()); 
    }
    
    private void Post(string subUrl, Action<DownloadHandler> callback)
    {
        StartCoroutine(PostRequest(BaseUrl + subUrl, "", callback));
    }

    private void Get(string subUrl, Action<DownloadHandler> callback)
    {
        StartCoroutine(GetRequest(BaseUrl + subUrl, callback)); 
    }
    
    IEnumerator PostRequest(string uri, string data, Action<DownloadHandler> callback)
    {
        Debug.Log(uri);
        using UnityWebRequest webRequest = UnityWebRequest.Post(uri, data);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Recieved Data from Post URL {uri}");
            callback(webRequest.downloadHandler);
        }
        else
        {
            Debug.LogError($"Error Post URL {uri}: {webRequest.error}"); 
        }
    }

    IEnumerator GetRequest(string uri, Action<DownloadHandler> callback)
    {
        Debug.Log(uri);
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Recieved Data from GET URL {uri}");
            callback(webRequest.downloadHandler);
        }
        else
        {
            Debug.LogError($"Error Get URL {uri}: {webRequest.error}");
        }
    }
}

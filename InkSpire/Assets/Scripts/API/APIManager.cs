using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager api;
    private readonly string BASE_URL = "http://3.38.126.43:8080";

    private void Awake()
    {
        if(api == null){
            api = this;
        }
        else if(api != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator GetRequest<T>(string url, Action<Response<T>> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(BASE_URL + url);
        yield return www.SendWebRequest();

        if (www != null)
        {
            Debug.Log(www.downloadHandler.text);
            Response<T> response_json = JsonUtility.FromJson<Response<T>>(www.downloadHandler.text);
            callback(response_json);
        }
        www.Dispose();
    }

    public IEnumerator PostRequest<T>(string url, string jsonfile, Action<Response<T>> callback)
    {
        byte[] json = System.Text.Encoding.UTF8.GetBytes(jsonfile);
        UnityWebRequest www = new(BASE_URL + url, "POST");
        UploadHandlerRaw uhr = new(json)
        {
            contentType = "application/json"
        };
        www.uploadHandler = uhr;
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www != null)
        {
            Debug.Log(www.downloadHandler.text);
            Response<T> response_json = JsonUtility.FromJson<Response<T>>(www.downloadHandler.text);
            callback(response_json);
        }
        www.Dispose();
    }

    public IEnumerator PutRequest<T>(string url, string jsonfile, Action<Response<T>> callback)
    {
        byte[] json = System.Text.Encoding.UTF8.GetBytes(jsonfile);
        UnityWebRequest www = new(BASE_URL + url, "PUT");
        UploadHandlerRaw uhr = new(json)
        {
            contentType = "application/json"
        };
        www.uploadHandler = uhr;
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www != null)
        {
            Debug.Log(www.downloadHandler.text);
            Response<T> response_json = JsonUtility.FromJson<Response<T>>(www.downloadHandler.text);
            callback(response_json);
        }
        www.Dispose();
    }
}
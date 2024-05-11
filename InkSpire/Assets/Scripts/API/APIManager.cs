using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager api;

    private void Awake()
    {
        if(api == null){
            api = this;
            DontDestroyOnLoad(api);
        }
        else if(api != this){
            Destroy(this);
        }
    }

    public IEnumerator GetRequest<T>(string url, Action<Response<T>> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(Const.BASE_URL + url);
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
        Debug.Log(jsonfile);
        byte[] json = System.Text.Encoding.UTF8.GetBytes(jsonfile);
        UnityWebRequest www = new(Const.BASE_URL + url, "POST");
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
        Debug.Log(jsonfile);
        byte[] json = System.Text.Encoding.UTF8.GetBytes(jsonfile);
        UnityWebRequest www = new(Const.BASE_URL + url, "PUT");
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

    public IEnumerator DeleteRequest<T>(string url, Action<Response<T>> callback)
    {
        UnityWebRequest www = UnityWebRequest.Delete(Const.BASE_URL + url);
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
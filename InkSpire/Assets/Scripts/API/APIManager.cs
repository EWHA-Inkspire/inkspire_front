using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager api;
    private string BASE_URL = "http://3.38.126.43:8080";

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

    public IEnumerator PostRequest(string url, string jsonfile, Action<Response> callback)
    {
        byte[] json = System.Text.Encoding.UTF8.GetBytes(jsonfile);
        UnityWebRequest www = new UnityWebRequest(BASE_URL + url, "POST");
        UploadHandlerRaw uhr = new UploadHandlerRaw(json);
        uhr.contentType = "application/json";
        www.uploadHandler = uhr;
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www != null)
        {
            Debug.Log(www.downloadHandler.text);
            Response response_json = JsonUtility.FromJson<Response>(www.downloadHandler.text);
            callback(response_json);
        }
        www.Dispose();
    }
}
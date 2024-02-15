using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    // 스크립트 정보를 싱글톤으로 관리
    public static ScriptManager scriptinfo;

    private string final_obj;
    private string[] chapter_obj = new string[5];
    private string time_background;
    private string space_background;
    private string genre;

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(scriptinfo == null){
            scriptinfo=this;
        }

    }
    public void setBackground(string time, string space, string gen){
        time_background = time;
        space_background = space;
        genre = gen;
    }
}

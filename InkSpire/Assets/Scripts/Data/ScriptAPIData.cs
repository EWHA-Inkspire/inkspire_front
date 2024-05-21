using System;

[Serializable]
// 캐릭터 & 스크립트 정보 저장 요청 (POST)
public class ScenarioInfo
{
    public CharacterInfo character;
    public ScriptInfo script;
}

[Serializable]
// 스크립트 정보 저장 요청 (POST)
public class ScriptInfo
{
    public string timeBackground;
    public string spaceBackground;
    public string genre;
    public string worldDetail;

    public ScriptInfo(string genre, string timeBackground, string spaceBackground, string worldDetail)
    {
        this.genre = genre;
        this.timeBackground = timeBackground;
        this.spaceBackground = spaceBackground;
        this.worldDetail = worldDetail;
    }
}

[Serializable]
public class GetScriptInfo
{
    public int scriptId;
    public string timeBackground;
    public string spaceBackground;
    public string genre;
    public string worldDetail;
    public string intro;
}

[Serializable]
// 캐릭터 & 스크립트 정보 저장 응답
public class PostScriptResponse
{
    public int characterId;
    public int scriptId;
}

[Serializable]
// 인트로 정보 저장 요청 (POST)
public class IntroInfo
{
    public int scriptId;
    public string intro;
}

[Serializable]
public class PostImageInfo
{
    public int scriptId;
    public int chapter;
    public string url;
}
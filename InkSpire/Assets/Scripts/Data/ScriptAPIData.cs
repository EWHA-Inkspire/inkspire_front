using System;

[Serializable]
// 캐릭터 & 스크립트 정보 저장 요청 (POST)
class ScenarioInfo
{
    public CharacterInfo character;
    public ScriptInfo script;
}

[Serializable]
// 스크립트 정보 저장 요청 (POST)
class ScriptInfo
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
// 캐릭터 & 스크립트 정보 저장 응답
class PostScriptResponse
{
    public int characterId;
    public int scriptId;
}

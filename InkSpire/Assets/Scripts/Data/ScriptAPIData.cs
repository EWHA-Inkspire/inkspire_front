using UnityEngine;
using System;

[Serializable]
class ScenarioInfo
{
    public CharacterInfo character;
    public ScriptInfo script;
}

[Serializable]
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
class PostScriptResponse
{
    public int characterId;
    public int scriptId;
}

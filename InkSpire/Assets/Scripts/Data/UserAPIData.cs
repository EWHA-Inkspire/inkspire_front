using System;
using System.Collections.Generic;

[Serializable]
public class ProfileInfo {
    public string nickname;
    public string email;
    public int endingCount;

    public override string ToString()
    {
        return "nickname: " + nickname + "\nemail: " + email + "\nendingCount: " + endingCount;
    }
}

[Serializable]
public class CharacterList {
    public List<Character> characters = new();
}

[Serializable]
public class Character {
    public int id;
    public string name;
    public string success;
    public string fail;

    public override string ToString()
    {
        return "id: " + id + "\nname: " + name + "\nsuccess: " + success + "\nfail: " + fail;
    }
}
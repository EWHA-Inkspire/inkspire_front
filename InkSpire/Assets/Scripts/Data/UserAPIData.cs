using System;
using System.Collections.Generic;

// 회원가입 요청 API (POST)
public class NewAccount {
    public string email;
    public string password;
    public string nickname;
}

// 로그인 요청 API (POST)
public class LoginInfo {
    public string email;
    public string password;
}

[Serializable]
// 프로필 정보 응답 API (GET)
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
// 캐릭터 리스트 응답 API (GET)
public class CharacterList {
    public List<Character> characters = new();
}

[Serializable]
// 캐릭터 정보 응답 API (GET)
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
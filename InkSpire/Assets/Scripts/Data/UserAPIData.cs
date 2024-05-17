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

[Serializable]
// 업적 리스트 응답 API (GET)
public class AchieveList {
    public List<Achieve> achievements = new();
}

[Serializable]
// 업적 정보 응답 API (GET)
public class Achieve {
    public string title;
    public string genre;

    public override string ToString()
    {
        return "title: " + title + "\ngenre: " + genre;
    }
}
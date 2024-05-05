using System;
using System.Collections.Generic;

[Serializable]
public class ChatList
{
    public List<ChatInfo> chats;
}

[Serializable]
public class ChatInfo
{
    public int scriptId;
    public string role;
    public string content;
    public int chapter;
}
using System;
using System.Collections.Generic;

[Serializable]
public class GetGoalList
{
    public List<GetGoalInfo> goals;
}

[Serializable]
public class GetGoalInfo
{
    public int goalId;
    public int chapter;
    public string type;
    public string title;
    public string detail;
    public string etc;
}

[Serializable]
public class PostGoalInfo
{
    public int scriptId;
    public int chapter;
    public string type;
    public string title;
    public string detail;
    public string etc;
}
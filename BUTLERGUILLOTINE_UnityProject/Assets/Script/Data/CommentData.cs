using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CommentData")]
public class CommentData : ScriptableObject
{
    public List<Comment> CommentList = new List<Comment>();

    public bool HasCommentWithID(string commentID)
    {
        foreach (var item in CommentList)
        {
            if (item.ID == commentID)
                return true;
        }

        return false;
    }

    public string GetCommentWithID(string commentID)
    {
        foreach (var item in CommentList) 
        { 
            if (item.ID == commentID)
                return item.Content;
        }

        return "COMMENT MISSING";
    }

    public string GetCommentWithIndex(int index)
    {
        if (index >= CommentList.Count)
            return "COMMENT INDEX DOES NOT EXIST";
        else
            return CommentList[index].Content;
    }
}

[System.Serializable]
public class Comment
{
    public string ID;
    public string Content;
}

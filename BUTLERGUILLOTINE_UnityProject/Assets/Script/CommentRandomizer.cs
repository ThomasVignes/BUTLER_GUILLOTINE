using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommentRandomizer : MonoBehaviour
{
    [SerializeField] List<string> comments = new List<string>();
    [SerializeField] GameObject[] targets;
    public UnityEvent EventDelegates;

    List<string> dump = new List<string>();
    List<GameObject> interactables = new List<GameObject>();

    private void Start()
    {
        foreach (var item in targets)
        {
            CommentInteractable commentInteractable = item.GetComponentInChildren<CommentInteractable>();

            if (commentInteractable == null)
                commentInteractable = item.GetComponent<CommentInteractable>();

            if (commentInteractable != null)
            {
                commentInteractable.UpdateComment(GetComment());
                commentInteractable.OnCommentEnd.AddListener(TriggerDelegates);

                interactables.Add(commentInteractable.gameObject);
                commentInteractable.gameObject.SetActive(false);
            }
        }
    }

    void TriggerDelegates()
    {
        EventDelegates?.Invoke();
    }

    public void EnableAllComments()
    {
        foreach (var item in interactables)
        {
            item.SetActive(true);
        }
    }

    string GetComment()
    {
        string comment = "null";

        if (comments.Count > 0)
        {
            int rand = Random.Range(0, comments.Count);

            comment = comments[rand];

            comments.RemoveAt(rand);
            dump.Add(comment);
        }
        else
        {
            int rand = Random.Range(0, dump.Count);

            comment = dump[rand];
        }

        return comment; 
    }
}

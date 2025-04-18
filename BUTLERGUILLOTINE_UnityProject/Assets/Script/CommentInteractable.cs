using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommentInteractable : Interactable
{
    [SerializeField] private string comment;
    public UnityEvent OnCommentStart, OnCommentEnd;
    [SerializeField] CinemachineVirtualCamera commentCam;
    [SerializeField] float delayBeforeCamera = 0.6f;

    protected override void InteractEffects(Character character)
    {
         GameManager.Instance.WriteComment(comment, this);
    }

    public void StandaloneComment()
    {
        GameManager.Instance.WriteComment(comment, this);
    }
    public void UpdateComment(string newComment)
    {
        comment = newComment;
    }

    public void ToggleCommenting(bool toggle)
    {
        if (commentCam != null)
            StartCoroutine(C_CameraToggle(toggle));

        if (!toggle)
            OnCommentEnd?.Invoke();
        else
            OnCommentStart?.Invoke();
    }

    IEnumerator C_CameraToggle(bool toggle)
    {
        float delay = delayBeforeCamera;

        if (!toggle)
            delay = 0;

        yield return new WaitForSeconds(delay);

        commentCam.gameObject.SetActive(toggle);
    }
}

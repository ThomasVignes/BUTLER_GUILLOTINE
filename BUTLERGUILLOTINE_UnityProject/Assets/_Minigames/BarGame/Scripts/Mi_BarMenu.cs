using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarMenu : MonoBehaviour
{
    [Header("Params")]
    public bool Visible;
    public bool Hovering;
    [SerializeField] private float ClickCD;


    [Header("References")]
    public Transform Hidden, Half, Shown;
    [SerializeField] private List<Mi_BarMenuPage> Pages = new List<Mi_BarMenuPage>();
    [SerializeField] private Mi_BarMenuPage Cover;

    private float ClickCDFloat;

    private int currentPage;

    private void Start()
    {
        Cover.Menu = this;
        foreach (var p in Pages)
        {
            p.Menu = this;
        }

        Visible = false;
        currentPage = 0;
    }

    private void Update()
    {
        if (Visible)
        {
            Pages[currentPage].Shown = true;
            Cover.Shown = true;
        }
        else
        {
            Pages[currentPage].Shown = false;
            Cover.Shown = false;
            if (Hovering)
            {
                Pages[currentPage].Semi= true;
                Cover.Semi = true;
            }
            else
            {
                Pages[currentPage].Semi = false;
                Cover.Semi = false;
            }
        }

        if (ClickCDFloat > 0)
            ClickCDFloat -= Time.deltaTime;
    }

    private void OnMouseOver()
    {
        Hovering = true;

        if (Input.GetMouseButtonDown(0))
        {
            Visible = !Visible;
            EffectsManager.Instance.audioManager.Play("Froufrou");
        }
    }

    private void OnMouseExit()
    {
        Hovering = false;
    }

   public void Flip()
   {
        if (ClickCDFloat <= 0)
        {
            EffectsManager.Instance.audioManager.Play("Froufrou");
            Pages[currentPage].Shown = false;
            Pages[currentPage].Semi = false;
            if (currentPage < Pages.Count - 1)
                currentPage++;
            else
                currentPage = 0;

            ClickCDFloat = ClickCD;
        }
   }

    public void Close()
    {
        Visible = false;
        EffectsManager.Instance.audioManager.Play("Froufrou");
    }
}

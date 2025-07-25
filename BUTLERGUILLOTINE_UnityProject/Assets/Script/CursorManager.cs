using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D BaseCursor, MoveCursor, LookCursor, AimCursor, AimMoveCursor, AimLookCursor;
    public Texture2D BaseCursor_bw, MoveCursor_bw, LookCursor_bw, AimCursor_bw, AimMoveCursor_bw, AimLookCursor_bw;

    bool blackAndWhite;

    private CursorType current;

    public void Init()
    {
        Cursor.lockState = CursorLockMode.Confined;

        current = CursorType.Base;
    }

    public void ToggleBlackAndWhite(bool blackAndWhite)
    {
        this.blackAndWhite = blackAndWhite;

        CursorType type = current;
        current = CursorType.Other;
        SetCursorType(type);
    }

    public void SetCursorType(CursorType type)
    {
        if (current == type)
            return;

        current = type;

        if (type != CursorType.Invisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        switch (type) 
        {
            case CursorType.Base :
                if (!blackAndWhite)
                    Cursor.SetCursor(BaseCursor, new Vector2(18, 13), CursorMode.Auto);
                else
                    Cursor.SetCursor(BaseCursor_bw, new Vector2(18, 13), CursorMode.Auto);
                break;

            case CursorType.Move:
                if (!blackAndWhite)
                    Cursor.SetCursor(MoveCursor, new Vector2(18, 13), CursorMode.Auto);
                else
                    Cursor.SetCursor(MoveCursor_bw, new Vector2(18, 13), CursorMode.Auto);
                break;

            case CursorType.Look:
                if (!blackAndWhite)
                    Cursor.SetCursor(LookCursor, new Vector2(18, 13), CursorMode.Auto);
                else
                    Cursor.SetCursor(LookCursor_bw, new Vector2(18, 13), CursorMode.Auto);
                break;

            case CursorType.Aim:
                if (!blackAndWhite)
                    Cursor.SetCursor(AimCursor, new Vector2(25, 25), CursorMode.Auto);
                else
                    Cursor.SetCursor(AimCursor_bw, new Vector2(25, 25), CursorMode.Auto);
                break;

            case CursorType.AimMove:
                if (!blackAndWhite)
                    Cursor.SetCursor(AimMoveCursor, new Vector2(25, 25), CursorMode.Auto);
                else
                    Cursor.SetCursor(AimMoveCursor_bw, new Vector2(25, 25), CursorMode.Auto);
                break;

            case CursorType.AimLook:
                if (!blackAndWhite)
                    Cursor.SetCursor(AimLookCursor, new Vector2(25, 25), CursorMode.Auto);
                else
                    Cursor.SetCursor(AimLookCursor_bw, new Vector2(25, 25), CursorMode.Auto);
                break;

            case CursorType.Invisible:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}

public enum CursorType
{
    Base,
    Move,
    Look,
    Aim,
    AimMove,
    AimLook,
    Invisible,
    Other
}

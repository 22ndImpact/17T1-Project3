using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputController
{
    #region Basic Input States
    public static bool LeftMouseButtonDown
    {
        get
        {
            return Input.GetMouseButtonDown(0);
        }
    }
    public static bool LeftMouseButtonUp
    {
        get
        {
            return Input.GetMouseButtonUp(0);
        }
    }
    public static bool LeftMouseButton
    {
        get
        {
            return Input.GetMouseButton(0);
        }
    }
    #endregion

    #region Complex Input States
    /// <summary>
    /// Returns true if the cursor is currently over a UI Element
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUIObject()
    {
        // Code utility from http://answers.unity3d.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html
        // This works the same as the usual event systems "cursor on object" but also works with touch interfaces
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    /// <summary>
    /// Returns true if the cursor is within the cube boundary
    /// </summary>
    /// <param name="_Point">The center point of the bounding box</param>
    /// <param name="_Boundary">The distances in both + and - from the centerpoint in all directions that determines the bounding box</param>
    /// <returns></returns>
    public static bool MouseOnPoint(Vector3 _Point, Vector3 _Boundary)
    {
        //Gets the mouse position on screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //If the mouse is within 
        if (mousePosition.x > _Point.x - _Boundary.x &&
            mousePosition.x < _Point.x + _Boundary.x &&
            mousePosition.y > _Point.y - _Boundary.y &&
            mousePosition.y < _Point.y + _Boundary.y)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// returns true if the cursor is a certian range to the given point
    /// </summary>
    /// <param name="_Point"></param>
    /// <param name="_Range"></param>
    /// <returns></returns>
    public static bool MouseOnPoint(Vector3 _Point, float  _Range)
    {
        //Gets the mouse position on screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePosition.z = 0;
        Vector3 pointPosition = _Point; pointPosition.z = 0;

        //If the distance between the mouse and the object is within range
        if ((_Point - mousePosition).magnitude < _Range)
        {
            return true;
        }

        return false;
    }
    #endregion
}

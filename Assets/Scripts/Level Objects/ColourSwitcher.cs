using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourSwitcher : ColouredObject
{
    void OnTriggerEnter(Collider _collider)
    {
        //If it collides with another coloured object
        if (_collider.gameObject.GetComponent<ColouredObject>() != null)
        {
            //Change the colour of the other object to that of the switcher
            _collider.gameObject.GetComponent<ColouredObject>().ChangeColour(objectColour);
        }
    }
}

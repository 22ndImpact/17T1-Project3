using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : ColouredObject
{
    void OnCollisionEnter(Collision _collision)
    {
        //If it collides with another coloured object
        if(_collision.gameObject.GetComponent<ColouredObject>() != null)
        {
            //If they are the same colour
            if(_collision.gameObject.GetComponent<ColouredObject>().objectColour == objectColour)
            {
                //Destroy the object and let the game director know
                GameDirector.LevelManager.CurrentLevel.ObjectDestroyed(this);
                Destroy(this.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColouredObject : MonoBehaviour
{
    //Component References
    MeshRenderer MR;


    //Colour Materials
    public Material mat_Neutural;
    public Gradient gra_Neutural;
    public Material mat_AltOne;
    public Gradient gra_AltOne;
    public Material mat_AltTwo;
    public Gradient gra_AltTwo;

    //State Variables
    public ObjectColour objectColour;

    public enum ObjectColour
    {
        Neutural,
        AltOne,
        AltTwo,
        OrbColourCount
    }

    protected virtual void Awake()
    {
        //Componenet Referencing
        MR = GetComponent<MeshRenderer>();
    }

    //Changes the colour to the one given then updates the colour state
    public void ChangeColour(ObjectColour _ObjectColour)
    {
        objectColour = _ObjectColour;
        UpdateColour();
    }

    //Updates the trail and texture colour of the object based on the colour state
    public virtual void UpdateColour()
    {
        switch (objectColour)
        {
            case ObjectColour.Neutural:
                MR.material = mat_Neutural;
                break;
            case ObjectColour.AltOne:
                MR.material = mat_AltOne;
                break;
            case ObjectColour.AltTwo:
                MR.material = mat_AltTwo;
                break;
        }
    }
}

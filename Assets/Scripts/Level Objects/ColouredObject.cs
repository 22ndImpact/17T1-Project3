using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColouredObject : MonoBehaviour
{
    #region Tweaing Variables
    public Material mat_Neutural;
    public Gradient gra_Neutural;
    public Material mat_AltOne;
    public Gradient gra_AltOne;
    public Material mat_AltTwo;
    public Gradient gra_AltTwo;
    public ObjectColour objectColour;
    public enum ObjectColour
    {
        Neutural,
        AltOne,
        AltTwo,
        OrbColourCount
    }
    #endregion

    #region Component References
    MeshRenderer MR;
    #endregion

    protected virtual void Awake()
    {
        //Componenet Referencing
        MR = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Changes the colour to the one given then updates the colour state
    /// </summary>
    /// <param name="_ObjectColour"></param>
    public void ChangeColour(ObjectColour _ObjectColour)
    {
        objectColour = _ObjectColour;
        UpdateColour();
    }

    /// <summary>
    /// Updates the trail and texture colour of the object based on the colour state
    /// </summary>
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

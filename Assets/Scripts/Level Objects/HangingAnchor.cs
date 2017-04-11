using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingAnchor : MonoBehaviour
{

    public DestructibleObject hangingCube;
    public HingeJoint cubeHinge;

    LineRenderer Tether;

    void Awake()
    {
        Tether = GetComponent<LineRenderer>();
        InitializeHingeJoint();
    }


	
	// Update is called once per frame
	void Update ()
    {
        UpdateTether();
	}

    void UpdateTether()
    {
        //Your Position
        Tether.SetPosition(0, transform.position);
        //Mid point
        Tether.SetPosition(1, (hangingCube.transform.position));
    }

    void InitializeHingeJoint()
    {
        cubeHinge = hangingCube.gameObject.AddComponent<HingeJoint>();
        cubeHinge.connectedBody = gameObject.GetComponent<Rigidbody>();
    }
}

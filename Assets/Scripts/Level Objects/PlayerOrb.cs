using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrb : ColouredObject
{

    #region Variables

    //Balance variables
    [SerializeField] private float orbDragLerpSpeed;
    [SerializeField] private float launchForceMultiplier;

    //Object References
    private Transform Anchor;

    //Component References
    Rigidbody RB;
    SphereCollider SC;
    TrailRenderer Trail;
    LineRenderer Tether;


    //Used to track the state of the orb
    public OrbState orbState;
    public enum OrbState
    {
        Static,
        Charging,
        Active
    }
    #endregion

    #region Mono Behaviour Events
    protected override void Awake()
    {
        //Run the parents Awake Function
        base.Awake();

        //Componenet Referencing
        RB = GetComponent<Rigidbody>();
        SC = GetComponent<SphereCollider>();
        Trail = GetComponent<TrailRenderer>();
        Tether = GetComponent<LineRenderer>();
    }
    void Update()
    {
        UpdateInput();       

        //If the player is draggign the object around, update orb position
        if (orbState == OrbState.Charging)
        {
            FollowMouse();
        }
    }

    void LateUpdate()
    {
        UpdateTether();
    }
    #endregion

    //Initializes the Orb with spawn info from the level controller
    public void Initialize(LevelController _Level, Transform _Anchor)
    {
        //Sets the anchors position from the level
        Anchor = _Anchor;
        //moves the player orb to the anchor
        transform.position = Anchor.position;
        //Makes the orb a child of the level object
        transform.parent = _Level.transform;
        //Updates the initial state of the orb
        UpdateState();
        //Updates the initial colour at spawn
        UpdateColour();
    }

    //Checks updates states
    void UpdateInput()
    {
        //If you click within range of the orb change to chargign state
        if (InputController.LeftMouseButtonDown && orbState == OrbState.Static && InputController.MouseOnPoint(transform.position, 1))
            ChangeState(OrbState.Charging);

        //If you left go of the left mouse button which charging, fire the orb
        if (InputController.LeftMouseButtonUp && orbState == OrbState.Charging)
            LaunchOrb();
    }

    //Changes the orbs state based on given parameter and Updates the state.
    void ChangeState(OrbState _orbState)
    {
        //Changes the state to the state given
        orbState = _orbState;

        //Runs the update state function, determine what actions to take based on current state
        UpdateState();
    }

    // Changes the properties of the orb based on its current state (Static, Charging, Active).
    void UpdateState()
    {
        //Turns off all elements, so the switch statment can turn on the apprioriate ones
        RB.useGravity = false;
        SC.enabled = false;
        Tether.enabled = false;
        Trail.enabled = false;
        Trail.Clear();

        switch (orbState)
        {
            case OrbState.Static:
                break;
            case OrbState.Charging:
                Tether.enabled = true;
                break;
            case OrbState.Active:
                RB.useGravity = true;
                SC.enabled = true;
                Trail.enabled = true;
                break;
        }
    }

    // Changes the position of the orb to lerp towards the mouse position
    void FollowMouse()
    {
        //Determine the target position of the Orb
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); newPosition.z = 0;

        //Lerps towards the target position based on orbDragLerpSpeed
        transform.position = Vector3.Lerp(transform.position, newPosition, orbDragLerpSpeed);
    }

    // Launches the orb from the anchor 
    void LaunchOrb()
    {
        //Change state to being active
        ChangeState(OrbState.Active);

        //Determine the direction and distance form the anchor
        Vector3 positionDelta = Anchor.position - transform.position;

        //Apply force
        RB.AddForce(positionDelta * launchForceMultiplier);

        //Let the game director know youve shot your orb
        GameDirector.LevelManager.CurrentLevel.OrbShot();
    }

    //Updates the trail and texture colour of the object based on the colour state
    public override void UpdateColour()
    {
        //Does the base object update colour function
        base.UpdateColour();

        //Then continues with our specific additions
        switch (objectColour)
        {
            case ObjectColour.Neutural:
                Trail.colorGradient = gra_Neutural;
                break;
            case ObjectColour.AltOne:
                Trail.colorGradient = gra_AltOne;
                break;
            case ObjectColour.AltTwo:
                Trail.colorGradient = gra_AltTwo;
                break;
        }
    }

    //Updates the properties of the tether connecting the anchor and orb
    void UpdateTether()
    {
        //Your Position
        Tether.SetPosition(0, transform.position);
        //Mid point
        Tether.SetPosition(1, (transform.position + Anchor.position) * 0.5f);
        //Anchor Position
        Tether.SetPosition(2, Anchor.position);
    }
}

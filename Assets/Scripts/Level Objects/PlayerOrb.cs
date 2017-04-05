using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrb : MonoBehaviour
{

    #region Variables

    //Balance variables
    [SerializeField] private float orbDragLerpSpeed;
    [SerializeField] private float launchForceMultiplier;

    //Object References
    private Transform Anchor;

    //Component References
    MeshRenderer MR;
    Rigidbody RB;
    SphereCollider SC;

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
    void Awake()
    {
        //Componenet Referencing
        MR = GetComponent<MeshRenderer>();
        RB = GetComponent<Rigidbody>();
        SC = GetComponent<SphereCollider>();
    }
    void Update()
    {

        UpdateInput();

        if (orbState == OrbState.Charging)
        {
            FollowMouse();
        }
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
        //Sets properties to false, to be turned on with switch statement
        RB.useGravity = false;
        SC.enabled = false;

        //Checks state and enables properties 
        switch (orbState)
        {
            case OrbState.Static:
                break;
            case OrbState.Charging:
                break;
            case OrbState.Active:
                RB.useGravity = true;
                SC.enabled = true;
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
}

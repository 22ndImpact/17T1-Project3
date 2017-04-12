using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerOrb : ColouredObject
{

    #region Variables

    //Balance variables
    [SerializeField] private float orbDragLerpSpeed;
    [SerializeField] private float launchForceMultiplier;
    //The radius around the orb that can be clicked to begin a drag
    [SerializeField] private float dragRadius;
    //The maximum distance the orb can travel from the anchor while being dragged
    [SerializeField] private float dragLimit;

    //Object References
    private Transform Anchor;

    //Effects
    public GameObject Particle_Collision;
    public AudioClip HitNoise;



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

    //Trajectory Predictor
    public TrajectoryPredictor TrajPred = new TrajectoryPredictor();

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
    void Start()
    {
        TrajPred.LoadTrajectoryPredictor();
    }
    void Update()
    {
        UpdateInput();       

        //If the player is draggign the object around, update orb position
        if (orbState == OrbState.Charging)
        {
            //Track the position of the mouse
            FollowMouse();
            //Update the trajectory nodes
            TrajPred.UpdateTrajectory(transform.position, DetermineLaunchForce());
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
        //If you click within range of the orb change to chargign state. And not over a button
        if (InputController.LeftMouseButtonDown && orbState == OrbState.Static && InputController.MouseOnPoint(transform.position, dragRadius) && !EventSystem.current.IsPointerOverGameObject())
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
                TrajPred.HideTrajectory();
                break;
            case OrbState.Charging:
                Tether.enabled = true;
                TrajPred.ShowTrajectory();
                break;
            case OrbState.Active:
                RB.useGravity = true;
                SC.enabled = true;
                Trail.enabled = true;
                TrajPred.DestroyTrajectory();
                break;
        }
    }

    // Changes the position of the orb to lerp towards the mouse position
    void FollowMouse()
    {
        //Determine the target position of the Orb
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); targetPosition.z = 0;

        //If too large, change the target position
        if ((targetPosition - Anchor.position).magnitude > dragLimit)
        {
            targetPosition = Anchor.position + ((targetPosition - Anchor.position).normalized * dragLimit);
        }

        //Lerps towards the target position based on orbDragLerpSpeed
        transform.position = Vector3.Lerp(transform.position, targetPosition, orbDragLerpSpeed);
    }

    // Launches the orb from the anchor 
    void LaunchOrb()
    {
        //Change state to being active
        ChangeState(OrbState.Active);

        //Apply force
        RB.AddForce(DetermineLaunchForce());

        //Let the game director know youve shot your orb
        GameDirector.LevelManager.CurrentLevel.OrbShot();
    }

    Vector3 DetermineLaunchForce()
    {
        //Determine the direction and distance form the anchor
        return (Anchor.position - transform.position) * launchForceMultiplier;
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

    //TODO code review this hack job
    void OnCollisionEnter(Collision _Collision)
    {
        //Create a particle prefab
        GameObject newParticleEmitter = Instantiate(Particle_Collision);

        //Change the position to the collision point
        newParticleEmitter.transform.position = _Collision.contacts[0].point;

        //Adjust its colour
        ParticleSystem.MainModule settings = newParticleEmitter.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(gameObject.GetComponent<MeshRenderer>().material.color);

        //Adjust its rotation
        newParticleEmitter.transform.rotation *= Quaternion.FromToRotation(Vector3.up, _Collision.contacts[0].normal);

        //Activate sound
        GameDirector.LevelManager.CurrentLevel.PlaySound(HitNoise);
    }
}

[Serializable]
public class TrajectoryPredictor
{
    public int TrajectoryNodes;
    public float DistanceBetweenNodes;
    public float DistanceOfFirstNode;

    public GameObject TrajectoryOrb;
    List<GameObject> TrajectoryNodeList = new List<GameObject>();

    public float VelocityAdjustment; //0.01985 FOR SOME REASON!!!!

    public void LoadTrajectoryPredictor()
    {
        Debug.Log("Create TrajectoryList");
        //Loads the predictor
        LoadTrajectoryPredictor(TrajectoryNodes);
    }

    public void LoadTrajectoryPredictor(int _Steps)
    {
        for (int i = 0; i < _Steps; i++)
        {
            GameObject newNode = GameObject.Instantiate(TrajectoryOrb);
            //Turns all of the off to start with
            newNode.SetActive(false);
            TrajectoryNodeList.Add(newNode);

        }
    }

    public void UpdateTrajectory(Vector3 pStartPosition, Vector3 pVelocity)
    {
        Vector3 AdjustedVelocity = pVelocity * VelocityAdjustment; 

        float velocity = Mathf.Sqrt((AdjustedVelocity.x * AdjustedVelocity.x) + (AdjustedVelocity.y * AdjustedVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(AdjustedVelocity.y, AdjustedVelocity.x));

        float fTime = 0;
        fTime += DistanceOfFirstNode;

        for (int i = 0; i < TrajectoryNodeList.Count; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics.gravity.magnitude * fTime * fTime / 2.0f);

            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
            TrajectoryNodeList[i].transform.position = pos;
            fTime += DistanceBetweenNodes;
        }
    }

    public void ShowTrajectory()
    {
        //Loops through each predictor
        for (int i = 0; i < TrajectoryNodeList.Count; i++)
        {
            //Turns on the orbs
            TrajectoryNodeList[i].SetActive(true);           
        }
    }

    public void HideTrajectory()
    {
        //Loops through each predictor
        for (int i = 0; i < TrajectoryNodeList.Count; i++)
        {
            //Turns on the orbs
            TrajectoryNodeList[i].SetActive(false);
        }
    }

    public void DestroyTrajectory()
    {
        //Loops through each predictor
        for (int i = 0; i < TrajectoryNodeList.Count; i++)
        {
            //Turns on the orbs
            GameObject.Destroy(TrajectoryNodeList[i]);
        }
    }
}



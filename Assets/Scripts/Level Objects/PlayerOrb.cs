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

        TrajPred.LoadTrajectoryPredictor();
    }
    void Start()
    {

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

    void OnDrawGizmos()
    {
        if (orbState == OrbState.Charging)
        {
            //Vector3[] points = Curver.MakeSmoothCurve(TrajPred.GetTrajectoryLocations(), 3f);

            Vector3[] points = TrajPred.GetTrajectoryLocations();

            bool ptset = false;
            Vector3 lastpt = Vector3.zero;
            for (int j = 0; j < points.Length; j++)
            {
                Vector3 wayPoint = points[j];
                if (ptset)
                {
                    //Gizmos.color = new Color(0, 0, 1, 0.5f);
                    //Gizmos.DrawLine(lastpt, wayPoint);
                    Debug.DrawLine(lastpt, wayPoint);
                }
                lastpt = wayPoint;
                ptset = true;
            }
        }
    }
}

[Serializable]
public class TrajectoryPredictor
{
    public int TrajectoryNodes;
    public float DistanceBetweenNodes;
    public float DistanceOfFirstNode;

    public GameObject TrajectoryLinePrefab;
    GameObject TrajectoryLine;
    public GameObject TrajectoryOrb;
    public List<GameObject> TrajectoryNodeList = new List<GameObject>();
    LineRenderer LR;
    public float TrajectoryFadeInTime;
    float TrajectoryFadeInTimer;
    Gradient BaseGradient;

    public float VelocityAdjustment; //0.01985 FOR SOME REASON!!!!

    public void LoadTrajectoryPredictor()
    {
        #region Set up Trail Container
        //Instanciate the line object
        TrajectoryLine = GameObject.Instantiate(TrajectoryLinePrefab);
        //Extract the line renderer
        LR = TrajectoryLine.GetComponent<LineRenderer>();
        //Parent the trajectory to the anchor object
        TrajectoryLine.transform.parent = GameObject.Find("Anchor").transform;
        //Centering the line object on the enchor
        TrajectoryLine.transform.localPosition = Vector3.zero;
        //Starts the fade in timer
        TrajectoryFadeInTimer = TrajectoryFadeInTime;
        //Storing the base gradient for fading in
        BaseGradient = LR.colorGradient;
        #endregion

        //Loads the predictor
        PopulateTrajectoryPredictor(TrajectoryNodes);
    }

    public Vector3[] GetTrajectoryLocations()
    {
        Vector3[] PositionList = new Vector3[TrajectoryNodes];

        for (int i = 0; i < TrajectoryNodes; i++)
        {
            PositionList[i] = TrajectoryNodeList[i].transform.position;
        }

        return PositionList;
    }

    public void PopulateTrajectoryPredictor(int _Steps)
    {
        for (int i = 0; i < _Steps; i++)
        {
            GameObject newNode = GameObject.Instantiate(TrajectoryOrb);
            //Set parent to container
            newNode.transform.parent = TrajectoryLine.transform;
            //Add the node to the list
            TrajectoryNodeList.Add(newNode);
        }
    }

    public void UpdateTrajectory(Vector3 pStartPosition, Vector3 pVelocity)
    {
        #region Determine Node Positions
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

        for (int i = 0; i < TrajectoryNodeList.Count; i++)
        {
            if (i == 0)
            {
                TrajectoryNodeList[i].transform.LookAt(pStartPosition);
            }
            else
            {
                TrajectoryNodeList[i].transform.LookAt(TrajectoryNodeList[i - 1].transform.position);
            }
        }
        #endregion

        #region Adjust Line Renderer
        LR.numPositions = GetTrajectoryLocations().Length;

        Vector3[] points = GetTrajectoryLocations();
        for (int j = 0; j < points.Length; j++)
        {
            Vector3 wayPoint = points[j];
            LR.SetPosition(j, wayPoint);
        }

        #region Trajectory fade in
        //Counts down the trajectory fade in
        if (TrajectoryFadeInTimer > 0)
        {
            //Reduce the timer
            TrajectoryFadeInTimer -= Time.smoothDeltaTime;
            //Create a placeholder gradient
            Gradient newGradient = new Gradient();
            //Alter alpha on gradient
            newGradient.SetKeys(LR.colorGradient.colorKeys, new GradientAlphaKey[] {
            new GradientAlphaKey(Mathf.Lerp(BaseGradient.alphaKeys[0].alpha, 0, TrajectoryFadeInTimer / TrajectoryFadeInTime), 0.0f),
            new GradientAlphaKey(Mathf.Lerp(BaseGradient.alphaKeys[1].alpha, 0, TrajectoryFadeInTimer / TrajectoryFadeInTime), 1.0f) });
            //Apply new gradient
            LR.colorGradient = newGradient;
        }

        #endregion

        #endregion
    }

    public void ShowTrajectory()
    {
        TrajectoryLine.SetActive(true);
    }

    public void HideTrajectory()
    {
        TrajectoryLine.SetActive(false);
    }

    public void DestroyTrajectory()
    {
        GameObject.Destroy(TrajectoryLine);
    }

    
}
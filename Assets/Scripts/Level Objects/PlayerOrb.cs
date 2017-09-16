using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerOrb : ColouredObject
{
    #region Tracking variables
    public OrbState orbState;
    public enum OrbState
    {
        Static,
        Charging,
        Active
    }
    public bool alive;
    #endregion

    #region Tweaking Variables
    //Balance variables
    [SerializeField] private float orbDragLerpSpeed;
    [SerializeField] private float launchForceMultiplier;
    //The radius around the orb that can be clicked to begin a drag
    [SerializeField] private float dragRadius;
    //The maximum distance the orb can travel from the anchor while being dragged
    [SerializeField] private float dragLimit;
    //The y position that the orb destorys itself when goes below
    public float yPositionBoundary;
    //The amoutn of fade time there is if the orb dies
    public float FadeTime;
    #endregion

    #region Object References
    private Transform Anchor;
    public GameObject Particle_Collision;
    public AudioClip HitNoise;
    public TrajectoryPredictor TrajPred = new TrajectoryPredictor();
    #endregion

    #region Component References
    SphereCollider SC;
    TrailRenderer Trail;
    LineRenderer Tether;
    #endregion

    protected override void Awake()
    {
        //Run the parents Awake Function
        base.Awake();

        alive = true;

        //Componenet Referencing
        RB = GetComponent<Rigidbody>();
        SC = GetComponent<SphereCollider>();
        Trail = GetComponent<TrailRenderer>();
        Tether = GetComponent<LineRenderer>();

        //Initialize the trajectory predictor
        TrajPred.LoadTrajectoryPredictor(this);
    }

    void Update()
    {
        //Update the input states of the player
        UpdateInput();       

        //If the player is draggign the object around, update orb position and the trajectory
        if (orbState == OrbState.Charging)
        {
            //Track the position of the mouse
            FollowMouse();
            //Update the trajectory nodes
            TrajPred.UpdateTrajectory(transform.position, DetermineLaunchForce());
        }


        //check position of orb to destroy it
        CheckIfBelowBoundary();
    }

    void LateUpdate()
    {
        //Update the position and thickness of the tether (done in late update to keep shit smooth and in sync)
        UpdateTether();
    }

    /// <summary>
    /// Initializes the Orb with spawn info from the level controller
    /// </summary>
    /// <param name="_Level"></param>
    /// <param name="_Anchor"></param>
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

    /// <summary>
    /// Checks input states and changes orbstate based on input
    /// </summary>
    void UpdateInput()
    {
        //If you click within range of the orb change to chargign state. And not over a button
        if (InputController.LeftMouseButton && orbState == OrbState.Static && InputController.MouseOnPoint(transform.position, dragRadius) && !InputController.IsPointerOverUIObject())
            ChangeState(OrbState.Charging);

        //If you left go of the left mouse button which charging, fire the orb
        if (InputController.LeftMouseButtonUp && orbState == OrbState.Charging)
            LaunchOrb();
    }

    /// <summary>
    /// Changes the orbs state based on given parameter and Updates the state.
    /// </summary>
    /// <param name="_orbState"></param>
    void ChangeState(OrbState _orbState)
    {
        //Changes the state to the state given
        orbState = _orbState;

        //Runs the update state function, determine what actions to take based on current state
        UpdateState();
    }

    /// <summary>
    /// Changes the properties of the orb based on its current state (Static, Charging, Active).
    /// </summary>
    void UpdateState()
    {
        //Turns off all elements, so the switch statment can turn on the apprioriate ones
        RB.useGravity = false;
        SC.enabled = false;
        Tether.enabled = false;
        Trail.enabled = false;
        Trail.Clear();

        //Turns on elements based on current state
        switch (orbState)
        {
            case OrbState.Static:
                TrajPred.HideTrajectory();
                break;
            case OrbState.Charging:
                Tether.enabled = true;
                TrajPred.ShowTrajectory();
                Anchor.GetComponent<Anchor>().TurnOff();
                break;
            case OrbState.Active:
                //TEMP//RB.useGravity = true;
                SC.enabled = true;
                Trail.enabled = true;
                TrajPred.DestroyTrajectory();
                break;
        }
    }

    /// <summary>
    /// Changes the position of the orb to lerp towards the mouse position
    /// </summary>
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

    /// <summary>
    /// Launches the orb from the anchor 
    /// </summary>
    void LaunchOrb()
    {
        //Change state to being active
        ChangeState(OrbState.Active);

        //Apply force
        RB.AddForce(DetermineLaunchForce());

        //Let the game director know youve shot your orb
        GameDirector.LevelManager.CurrentLevel.OrbShot();
    }

    /// <summary>
    /// Determines the force that would be applied to the orb if it were to be released on this frame
    /// </summary>
    /// <returns></returns>
    Vector3 DetermineLaunchForce()
    {
        //Determine the direction and distance form the anchor
        return (Anchor.position - transform.position) * launchForceMultiplier;
    }

    /// <summary>
    /// Returns the percentage of distance dragged from the anchor based on max drag distance
    /// </summary>
    public float CurrentDragPercentage
    {
        get
        {
            return (transform.position - Anchor.position).magnitude / dragLimit;
        } 
    }

    /// <summary>
    /// Updates the trail and texture colour of the object based on the colour state
    /// </summary>
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

    /// <summary>
    /// Updates the properties of the tether connecting the anchor and orb
    /// </summary>
    void UpdateTether()
    {
        //Your Position
        Tether.SetPosition(0, transform.position);
        //Mid point
        Tether.SetPosition(1, (transform.position + Anchor.position) * 0.5f);
        //Anchor Position
        Tether.SetPosition(2, Anchor.position);
    }

    /// <summary>
    /// TODO code review this hack job
    /// </summary>
    /// <param name="_Collision"></param>
    void OnCollisionEnter(Collision _Collision)
    {
        //TODO Debug testing for new scoring method
        if(GameDirector.LevelManager.CurrentLevel.LevelOver == false)
        {
            //If you hit anything that isnt a player, add 1 point
            if (_Collision.contacts[0].otherCollider.gameObject.tag == "Wall")
            {
                //GameDirector.LevelManager.CurrentLevel.AdjustOrbsUsed(1);
            }
        
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
            GameDirector.audioController.PlayEffectPitchLoop(GameDirector.audioController.AudioCollection.OrbHit);
        }
    }

    /// <summary>
    /// checks if the player orb is below the specific boundary, and destorys it if it is
    /// </summary>
    void CheckIfBelowBoundary()
    {
        //Checks if the block is not lower than a certain cut off point, and if it is, Fade/Destroy it
        if (gameObject.transform.position.y <= yPositionBoundary && alive)
        {
            alive = false;
            //Take away an active orb
            GameDirector.LevelManager.CurrentLevel.ActiveOrbs -= 1;
            //Destroy the orb
            Destroy(this.gameObject, 5);

        }
    }

    public IEnumerator FadeOutDestroy()
    {
        //Take away an active orb
        GameDirector.LevelManager.CurrentLevel.ActiveOrbs -= 1;

        //Activate sound
        GameDirector.audioController.PlayEffectClip(GameDirector.audioController.AudioCollection.DestructibleObjectDeath);

        //Turns off the collider
        SC.enabled = false;

        //Stores the initial alpha of the object to lerp from
        float startignAlpha = GetComponent<MeshRenderer>().material.color.a;
        //Start a timer
        float timeTracker = FadeTime;

        //Setting the gradient base of the trail
        Gradient BaseGradient = Trail.colorGradient;

        while (timeTracker > 0)
        {
            //Store a temp colour to allow us to modify only the alpha
            Color tempColor = GetComponent<MeshRenderer>().material.color;
            //Lerp the alpha of the temp colour in time with the percentage of fade
            tempColor.a = Mathf.Lerp(0, startignAlpha, timeTracker / FadeTime);
            //Apply the temp colour back to the real material
            GetComponent<MeshRenderer>().material.color = tempColor;

            //Increase the completion by delta time
            timeTracker -= Time.smoothDeltaTime;

            #region Fade out trail
            //Create a placeholder gradient
            Gradient newGradient = new Gradient();
            //Alter alpha on gradient
            newGradient.SetKeys(Trail.colorGradient.colorKeys, new GradientAlphaKey[] {
            new GradientAlphaKey(Mathf.Lerp(0, BaseGradient.alphaKeys[0].alpha, timeTracker / FadeTime), 0.0f),
            new GradientAlphaKey(Mathf.Lerp(0, BaseGradient.alphaKeys[1].alpha, timeTracker / FadeTime), 1.0f) });
            //Apply new gradient
            Trail.colorGradient = newGradient;
            #endregion

            yield return null;
        }
        Destroy(this.gameObject);
    }
}

[Serializable]
public class TrajectoryPredictor
{
    #region Tracking variables
    public List<GameObject> TrajectoryNodeList = new List<GameObject>();
    float TrajectoryFadeInTimer;
    Gradient BaseGradient;
    #endregion

    #region Tweaking Variables
    public int TrajectoryNodes;
    public float DistanceBetweenNodes;
    public float DistanceOfFirstNode;
    public float TrajectoryFadeInTime;
    public float MaxLineThickness;
    public float MinLineThickness;
    public float VelocityAdjustment; //0.01985 FOR SOME REASON!!!!
    #endregion

    #region Object References
    PlayerOrb PlayerOrb;
    public GameObject TrajectoryLinePrefab;
    GameObject TrajectoryLine;
    public GameObject TrajectoryOrb;
    #endregion

    #region Component References
    LineRenderer LR;
    #endregion

    /// <summary>
    /// Initializes all the objects and variables required to display the trajectory of the given orb
    /// </summary>
    /// <param name="_PlayerOrb"></param>
    public void LoadTrajectoryPredictor(PlayerOrb _PlayerOrb)
    {
        #region Set up Trail Container
        //Set the player orb for reference
        PlayerOrb = _PlayerOrb;
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

    /// <summary>
    /// Extracts the positoins from each of the nodes to be used in the line renderer TODO needs updating to not need objects
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetTrajectoryLocations()
    {
        Vector3[] PositionList = new Vector3[TrajectoryNodes];

        for (int i = 0; i < TrajectoryNodes; i++)
        {
            PositionList[i] = TrajectoryNodeList[i].transform.position;
        }

        return PositionList;
    }

    /// <summary>
    /// Creates and lists all the node objects required to predict the trajectory
    /// </summary>
    /// <param name="_Steps"></param>
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

    /// <summary>
    /// Alter the position of the nodes based on the current position and velocity the orb will be shot
    /// </summary>
    /// <param name="pStartPosition"></param>
    /// <param name="pVelocity"></param>
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
            //Use for gravity prediction
            //float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics.gravity.magnitude * fTime * fTime / 2.0f);
            //Use for no gravity prediction
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad);

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
        LR.positionCount = GetTrajectoryLocations().Length;

        Vector3[] points = GetTrajectoryLocations();
        for (int j = 0; j < points.Length; j++)
        {
            Vector3 wayPoint = points[j];
            LR.SetPosition(j, wayPoint);
        }

        //Based on velocity of orb determine the thickness of the 
        LR.widthMultiplier = Mathf.Clamp(MaxLineThickness * (1 - PlayerOrb.CurrentDragPercentage), MinLineThickness, MaxLineThickness);

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

    /// <summary>
    /// Turns on the trajectoy line object
    /// </summary>
    public void ShowTrajectory()
    {
        TrajectoryLine.SetActive(true);
    }

    /// <summary>
    /// Turns off the trajectoy line object
    /// </summary>
    public void HideTrajectory()
    {
        TrajectoryLine.SetActive(false);
    }

    /// <summary>
    /// Destroys the trajectoy line object
    /// </summary>
    public void DestroyTrajectory()
    {
        GameObject.Destroy(TrajectoryLine);
    }
}
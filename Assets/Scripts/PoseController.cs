
using UnityEngine;
using System.Collections;
using Vuforia;

public class PoseController : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    #endregion //PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS

    [SerializeField] private UDTEventHandler udtEventHandler;
    [SerializeField] private ProximityDetector proximityDetector;
    [SerializeField] private Collider bugdroidCollider;
    [SerializeField] private GameObject msgFindTheBugDroid;
    [SerializeField] private GameObject msgTapTheBugDroid;
    [SerializeField] private GameObject msgTapTheCircle;
    [SerializeField] private GameObject msgTryAgain;
    [SerializeField] private GameObject msgGetCloser;

	public AudioSource effectAudio;

    private enum TrackingMode
    {
        DEVICE_ORIENTATION,
        CONSTRAINED_TO_CAMERA,
        UDT_BASED,
    }

    // initial mode
    private TrackingMode mTrackingMode = TrackingMode.DEVICE_ORIENTATION;

    private Vector3 mPosOffsetAtTargetCreation;

    private const float mInitialDistance = 2.5f;

    private bool mBuildingUDT = false;

    private Camera cam;

    private GameObject bugdroidModel;
    private GameObject bugdroidShadow;

    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Awake()
    {
        msgFindTheBugDroid.SetActive(true);
        msgTapTheBugDroid.SetActive(false);
        msgTapTheCircle.SetActive(false);
        msgTryAgain.SetActive(false);
        msgGetCloser.SetActive(false);

    }

    void Start()
    {
        mTrackingMode = TrackingMode.DEVICE_ORIENTATION;

        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);

        udtEventHandler.ShowQualityIndicator(false); // don't show at start

		bugdroidModel = GameObject.Find("Android_fbx");
    }

    void Update()
    {
        if (CheckTapOnObject()) {
			effectAudio.Play ();
            ChangeMode();
        }

       
    }

    void LateUpdate()
    {

        switch (mTrackingMode) {
        case TrackingMode.DEVICE_ORIENTATION:
            {
                // In this mode, the device orientation is tracked
                // and the object stays at a fixed position in world space

                // Update object rotation so that it always look towards the camera
                // and its "up vector" is always aligned with the gravity direction.
                // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                // to be aligned (approximately) with the real world gravity direction 
                RotateToLookAtCamera();

                if (IsBugDroidInView()) {
                    DisplayMessage(msgTapTheBugDroid);
                } else {
                    DisplayMessage(msgFindTheBugDroid);
                }
            }
            break;
        case TrackingMode.CONSTRAINED_TO_CAMERA:
            {
                // In this phase, the BugDroid is constrained to remain
                // in the camera view, so it follows the user motion
                Vector3 constrainedPos = cam.transform.position + cam.transform.forward * mInitialDistance;
                this.transform.position = constrainedPos;
                    
                // Update object rotation so that it always look towards the camera
                // and its "up vector" is always aligned with the gravity direction.
                // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                // to be aligned (approximately) with the real world gravity direction 
                RotateToLookAtCamera();

                // Check if we were waiting for a UDT creation,
                // and switch mode if UDT was created
                if (mBuildingUDT && udtEventHandler && udtEventHandler.TargetCreated) {

                    ImageTargetBehaviour trackedTarget = GetActiveTarget();

                    if (trackedTarget != null) {
                        mBuildingUDT = false;

                        // Switch mode to UDT based tracking
                        mTrackingMode = TrackingMode.UDT_BASED;

                        // Update header text
                        DisplayMessage(msgGetCloser);

                        // Hide quality indicator
                        udtEventHandler.ShowQualityIndicator(false);

                        // Show the bugdroid
                        ShowBugDroid(true);

                        // Wake up the proximity detector
                        if (proximityDetector) {
                            proximityDetector.Sleep(false);
                        }

                        // Save a snapshot of the current position offset
                        // between the object and the target center
                        mPosOffsetAtTargetCreation = this.transform.position - trackedTarget.transform.position;
                    }
                }
            }
            break;
        case TrackingMode.UDT_BASED:
            {
                // Update the object world position according to the UDT target position
                ImageTargetBehaviour trackedTarget = GetActiveTarget();
                if (trackedTarget != null) {
                    this.transform.position = trackedTarget.transform.position + mPosOffsetAtTargetCreation;
                }

                // Update object rotation so that it always look towards the camera
                // and its "up vector" is always aligned with the gravity direction.
                // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                // to be aligned (approximately) with the real world gravity direction 
                RotateToLookAtCamera();
            }
            break;
        }
    }

    #endregion //MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    public void ResetState()
    {
        mTrackingMode = TrackingMode.DEVICE_ORIENTATION;
        mBuildingUDT = false;

        // place the bugdroid relative to camera position and direction
        Vector3 direction = new Vector3(cam.transform.forward.x * -1.0f, 0, cam.transform.forward.z * -1.0f);
        Vector3 position = new Vector3(cam.transform.position.x, 0.2f, cam.transform.position.z);
        this.transform.position = position + direction * mInitialDistance;
        this.transform.rotation = Quaternion.identity;

        // Update message and mode text
        DisplayMessage(msgFindTheBugDroid);

        // Hide the quality indicator
        udtEventHandler.ShowQualityIndicator(false);

        // Show the bugdroid
        ShowBugDroid(true);

        // Make the proximity detector sleep
        if (proximityDetector) {
            proximityDetector.Sleep(true);
        }
    }

    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    // Callback called when Vuforia has started
    private void OnVuforiaStarted()
    {
        cam = Vuforia.DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        StartCoroutine(ResetAfter(0.5f));
    }

    private IEnumerator ResetAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetState();
    }

    private void ChangeMode()
    {
        if (mTrackingMode == TrackingMode.DEVICE_ORIENTATION) {
            SwitchToCameraMode();
        } else if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA) {
            SwitchToUDTMode();
        }
    }

    private void SwitchToCameraMode()
    {
        if (mTrackingMode == TrackingMode.DEVICE_ORIENTATION) {

            mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;

            DisplayMessage(msgTapTheCircle);

            // Show the quality indicator
            udtEventHandler.ShowQualityIndicator(true);

            // Hide the bugdroid
            ShowBugDroid(false);
        }
    }

    private void SwitchToUDTMode()
    {
        if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA) {
            // check if UDT frame quality is medium or high
            if (udtEventHandler.IsFrameQualityHigh() || udtEventHandler.IsFrameQualityMedium()) {
                // Build a new UDT
                // Note that this may take more than one frame
                CreateUDT();

            } else {
                DisplayMessage(msgTryAgain);
            }
        }
    }

    private void CreateUDT()
    {
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float halfSizeY = mInitialDistance * Mathf.Tan(0.5f * fovRad);
        float targetWidth = 2.0f * halfSizeY; // portrait
        if (Screen.width > Screen.height) { // landscape
            float screenAspect = Screen.width / (float)Screen.height;
            float halfSizeX = screenAspect * halfSizeY;
            targetWidth = 2.0f * halfSizeX;
        }

        mBuildingUDT = true;
        udtEventHandler.BuildNewTarget(targetWidth);
    }

    private void RotateToLookAtCamera()
    {
        Vector3 objPos = this.transform.position;
        Vector3 objGroundPos = new Vector3(objPos.x, 0, objPos.z); // y = 0
        Vector3 camGroundPos = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
        Vector3 objectToCam = camGroundPos - objGroundPos;
        objectToCam.Normalize();
        this.transform.rotation *= Quaternion.FromToRotation(this.transform.forward, objectToCam);
    }

    private void DisplayMessage(GameObject messageObj)
    {
        msgFindTheBugDroid.SetActive((msgFindTheBugDroid == messageObj));
        msgTapTheBugDroid.SetActive((msgTapTheBugDroid == messageObj));
        msgTapTheCircle.SetActive((msgTapTheCircle == messageObj));
        msgTryAgain.SetActive((msgTryAgain == messageObj));
        msgGetCloser.SetActive((msgGetCloser == messageObj));
    }

   

    private bool CheckTapOnObject()
    {
        if (bugdroidCollider == null)
            return false;

        // Test picking to check if user tapped on bugdroid
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit)) {
                return (hit.collider == bugdroidCollider);
            }
        }

        return false;
    }

    private ImageTargetBehaviour GetActiveTarget()
    {
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        foreach (var tb in stateManager.GetActiveTrackableBehaviours()) {
            if (tb is ImageTargetBehaviour) {
                // found target
                return (ImageTargetBehaviour)tb;
            }
        }
        return null;
    }

    private void ShowBugDroid(bool isVisible)
    {
		if (bugdroidModel != null  ){ //&& bugdroidShadow != null) {
			var renderers = bugdroidModel.GetComponentsInChildren<Renderer>();
			foreach(var r in renderers) {
				r.enabled = isVisible;

			}
            //bugdroidModel.GetComponent<Renderer>().enabled = isVisible;
            //bugdroidShadow.GetComponent<Renderer>().enabled = isVisible;
        }
    }

    private bool IsBugDroidInView()
    {
        Vector3 viewPos = cam.WorldToViewportPoint(bugdroidCollider.bounds.center);
        return (viewPos.x > 0.0f && viewPos.x < 1.0f && viewPos.y > 0.0f && viewPos.y < 1.0f && viewPos.z > 1.0f);
    }

    #endregion //PRIVATE_METHODS

}

/*****************************************************************************
*                                                                            *
*  Unity Wrapper                                                             *
*  Copyright (C) 2010 PrimeSense Ltd.                                        *
*                                                                            *
*  This file is part of OpenNI.                                              *
*                                                                            *
*  OpenNI is free software: you can redistribute it and/or modify            *
*  it under the terms of the GNU Lesser General Public License as published  *
*  by the Free Software Foundation, either version 3 of the License, or      *
*  (at your option) any later version.                                       *
*                                                                            *
*  OpenNI is distributed in the hope that it will be useful,                 *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of            *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the              *
*  GNU Lesser General Public License for more details.                       *
*                                                                            *
*  You should have received a copy of the GNU Lesser General Public License  *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.            *
*                                                                            *
*****************************************************************************/
//Author: Shlomo Zippel

/*****************************************************************************
* This file is a modified and extended version of the original Unity Wrapper 	*
* (as shown above). More information about our Kinect Superman project can   	*
* be found at: http://github.com/kinectsuperman/Kinect-Superman				*
*                                                                            							*
* Authors: Daniël Karavolos, Sicco van Sas & Maarten van der Velden          	*
* January 27th, 2011                                    University of Amsterdam 		*
*****************************************************************************/

// Basic imports.
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text; 
	
// The original Nite Unity Wrapper.
public class NiteWrapper {
	public enum SkeletonJoint {
		NONE = 0,
		HEAD = 1,
        NECK = 2,
        TORSO_CENTER = 3,
		WAIST = 4,

		LEFT_COLLAR = 5,
		LEFT_SHOULDER = 6,
        LEFT_ELBOW = 7,
        LEFT_WRIST = 8,
        LEFT_HAND = 9,
        LEFT_FINGERTIP = 10,

        RIGHT_COLLAR = 11,
		RIGHT_SHOULDER = 12,
		RIGHT_ELBOW = 13,
		RIGHT_WRIST = 14,
		RIGHT_HAND = 15,
        RIGHT_FINGERTIP = 16,

        LEFT_HIP = 17,
        LEFT_KNEE = 18,
        LEFT_ANKLE = 19,
        LEFT_FOOT = 20,

        RIGHT_HIP = 21,
		RIGHT_KNEE = 22,
        RIGHT_ANKLE = 23,
		RIGHT_FOOT = 24,

		END 
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointPosition {
        public float x, y, z;
        public float confidence;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointOrientation {
        public float    m00, m01, m02,
							m10, m11, m12,
							m20, m21, m22;
        public float confidence;
    }
	
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointTransformation {
        public SkeletonJointPosition pos;
        public SkeletonJointOrientation ori;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XnVector3D {
        public float x, y, z;
    }

	[DllImport("UnityInterface.dll")]
	public static extern uint Init(StringBuilder strXmlPath);
	[DllImport("UnityInterface.dll")]
	public static extern void Update(bool async);
	[DllImport("UnityInterface.dll")]
	public static extern void Shutdown();
	
	[DllImport("UnityInterface.dll")]
	public static extern IntPtr GetStatusString(uint rc);
	[DllImport("UnityInterface.dll")]
	public static extern int GetDepthWidth();
	[DllImport("UnityInterface.dll")]
	public static extern int GetDepthHeight();
	[DllImport("UnityInterface.dll")]
	public static extern IntPtr GetUsersLabelMap();
    [DllImport("UnityInterface.dll")]
    public static extern IntPtr GetUsersDepthMap();

	[DllImport("UnityInterface.dll")]
    public static extern void SetSkeletonSmoothing(double factor);
    [DllImport("UnityInterface.dll")]
    public static extern bool GetJointTransformation(uint userID, SkeletonJoint joint, ref SkeletonJointTransformation pTransformation);

    [DllImport("UnityInterface.dll")]
    public static extern void StartLookingForUsers(IntPtr NewUser, IntPtr CalibrationStarted, IntPtr CalibrationFailed, IntPtr CalibrationSuccess, IntPtr UserLost);
    [DllImport("UnityInterface.dll")]
    public static extern void StopLookingForUsers();
    [DllImport("UnityInterface.dll")]
    public static extern void LoseUsers();
    [DllImport("UnityInterface.dll")]
    public static extern bool GetUserCenterOfMass(uint userID, ref XnVector3D pCenterOfMass);
	
	// We detect the 'Pause' gesture ourselves for other purposes (camera switch).
    /*
	[DllImport("UnityInterface.dll")]
    public static extern float GetUserPausePoseProgress(uint userID);
	*/

    public delegate void UserDelegate(uint userId);

    public static void StartLookingForUsers(UserDelegate NewUser, UserDelegate CalibrationStarted, UserDelegate CalibrationFailed, UserDelegate CalibrationSuccess, UserDelegate UserLost) {
        StartLookingForUsers(
		Marshal.GetFunctionPointerForDelegate(NewUser),
		Marshal.GetFunctionPointerForDelegate(CalibrationStarted),
		Marshal.GetFunctionPointerForDelegate(CalibrationFailed),
		Marshal.GetFunctionPointerForDelegate(CalibrationSuccess),
		Marshal.GetFunctionPointerForDelegate(UserLost));
    }
}

// Our wrapper which receives the gyroscope information (yaw, pitch, roll) from the Vuzix iWear VR920.
public class VRWrapper {
	[DllImport("./iWear/iWearWrapper.dll")]
    public static extern char WrapIWROpenTracker();

    [DllImport("./iWear/iWearWrapper.dll")]
    public static extern char WrapIWRGetTracking(ref int yaw, ref int pitch, ref int roll);
	
	[DllImport("./iWear/iWearWrapper.dll")]
    public static extern void WrapIWRCloseTracker();	
}

// Used to find the game objects in Unity. It is only used during
// initialization in the Avatar class.
public class UnityUtils {
    // Recursive
    public static Transform FindTransform(GameObject parentObj, string objName) {
        if (parentObj == null) return null;

        foreach (Transform trans in parentObj.transform) {
            if (trans.name == objName)
                return trans;

            Transform foundTransform = FindTransform(trans.gameObject, objName);
            if (foundTransform != null)
                return foundTransform;
        }
        return null;
    }
}

// Every instance of an avatar is based on this class. It holds all skeleton information
// and detects the gestures.
public class Avatar {
	// All joint transforms.
    private Transform rightElbow;	
    private Transform leftElbow;
    private Transform rightArm;
    private Transform leftArm;
    private Transform rightKnee;
    private Transform leftKnee;
    private Transform rightHip;
    private Transform leftHip;
    private Transform spine;
    private Transform root;
	
	// The current position of the avatar in the game (this needs to be explicitly stored in order to offset
	// the kinect position after flight).
	private Vector3 characterPosition;

	// The initial rotations taken right after calibration succeeded.
    private Quaternion[] initialRotations;
	
	// Whether the user flies with the left and/or right arm.
	private bool flyLeftArm;
	private bool flyRightArm;
	
	// Whether the user shoot a laser with the left and/or right arm.
	private bool leftShootGesture;
	private bool rightShootGesture;
	
	// The transform information of the user's arm which is currently used for flight.
	private Transform mainFlyArm;
	
	// Used to toggle the left and right eye particle emitters of the lasers.
	private ParticleEmitter leftEye;
	private ParticleEmitter rightEye;
	
	// Used to toggle the ice breath particle emitter.
	private ParticleEmitter breath;
	
	// The first and third person camera's.
	public Camera firstCamera;
	public Camera thirdCamera;
	
	// A timer to avoid too fast switching between the first and third person camera.
	private int coolDownTimer = 0;
	
	// A flag used to check if the user has stopped flying in order to set the value
	// of kinectOffsetPosition.
	private bool firstMoveAfterFlight = true;
	private Vector3 kinectOffsetPosition;
	
	// The current flight speed.
	private float currentSpeed = 0;
	
	// Variables needed for the Vuzix iWear VR920.
    int yaw;
    int pitch;
    int roll;    
	GameObject head;
	Transform headTrans;	
	float maxRotation = 32768;
	public float smoothness = 100;
	private bool triggered = false; //portal
		//used for portals
	public Vector3 warpToPosition = new Vector3(0,0,0);


    public Avatar() {
    }

	// Constructor.
    public Avatar(GameObject go) {
        Initialize(go);
    }
	
	// The next 4 functions are used by a portal
	//Change characterPosition
	public void SetCharacterPosition (Vector3 newPosition) {
		characterPosition = newPosition;
	}
	
	public bool GetTriggered() { return triggered;}
	
	public void SetTriggered(bool newVal) {
		triggered = newVal;
	}
	
	public void OnTriggerEnter() {
		Debug.Log("Superman should be warped now");
		triggered = true;
	}

	// Initialization of a new avatar.
    public void Initialize(GameObject go) {
		// Find all joint game objects.
        rightElbow = UnityUtils.FindTransform(go, "R_Elbow");
        leftElbow = UnityUtils.FindTransform(go, "L_Elbow");
        rightArm = UnityUtils.FindTransform(go, "R_Arm");
        leftArm = UnityUtils.FindTransform(go, "L_Arm");
        rightKnee = UnityUtils.FindTransform(go, "R_Knee");
        leftKnee = UnityUtils.FindTransform(go, "L_Knee");
        rightHip = UnityUtils.FindTransform(go, "R_Hip");
        leftHip = UnityUtils.FindTransform(go, "L_Hip");		
		spine = UnityUtils.FindTransform(go, "Root");	
	
		flyLeftArm = false;
		flyRightArm = false;
		
		leftShootGesture = false;
		rightShootGesture = false;
		
		// Find the breath, left eye laser and right eye laser particle emitter game objects.
		// NOTE: These objects are found based on their alphabetical occurrence in the unity hierarchy,
		// so breath takes the value in [0], leftEye in [1] and rightEye in [2]. Keep this in mind (or change it)
		// if you add more particle emitters or if you change the names of these game objects.
		breath = go.GetComponentsInChildren<ParticleEmitter>()[0];
		breath.emit = false;
		Debug.Log(String.Format("PARTICLE BREATH? = {0}", breath.name));
		
		leftEye = go.GetComponentsInChildren<ParticleEmitter>()[1];
		leftEye.emit = false;
		Debug.Log(String.Format("PARTICLE LEFTEYE? = {0}", leftEye.name));
		
		rightEye = go.GetComponentsInChildren<ParticleEmitter>()[2];
		rightEye.emit = false;
		Debug.Log(String.Format("PARTICLE RIGHTEYE? = {0}", rightEye.name));
		
		// Find the camera game objects, the same NOTE holds as shown above.
		firstCamera = go.GetComponentsInChildren<Camera>()[0];
		firstCamera.enabled = false;
		Debug.Log(String.Format("CAMERA1? = {0}", firstCamera.name));
		
		thirdCamera = go.GetComponentsInChildren<Camera>()[1];
		thirdCamera.enabled = true;
		Debug.Log(String.Format("CAMERA3? = {0}", thirdCamera.name));

		// Create a new Quaternion array which holds all initial quaternion (i.e. rotation) information
		// of all joints.
        initialRotations = new Quaternion[(int)NiteWrapper.SkeletonJoint.END];
        initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_ELBOW] = leftElbow.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_ELBOW] = rightElbow.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_SHOULDER] = leftArm.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_SHOULDER] = rightArm.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_KNEE] = rightKnee.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_KNEE] = leftKnee.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_HIP] = rightHip.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_HIP] = leftHip.rotation;
        initialRotations[(int)NiteWrapper.SkeletonJoint.TORSO_CENTER] = spine.rotation;
		
		// Initialize the current position of the avatar/character to the initial spine postion.
		characterPosition = spine.position;
		
        RotateToCalibrationPose();
		
		// Initialize the iWear variables.
        yaw = 0;
        pitch = 0;
        roll = 0;		
		VRWrapper.WrapIWROpenTracker();
		head = GameObject.Find("Head");
		headTrans = head.transform;
    }
	
	// This function is called on every update in order to update the joints of the avatar
	// and to detect gestures.
	public void UpdateAvatar(uint userId) {
		// Receive all new joint rotations from the kinect.
		RotateBone(userId, NiteWrapper.SkeletonJoint.TORSO_CENTER, spine);
		RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_HIP, rightHip);
		RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_HIP, leftHip);
		RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_KNEE, rightKnee);
		RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_KNEE, leftKnee);
		RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_SHOULDER, rightArm);
        RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_ELBOW, rightElbow);
		RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_SHOULDER, leftArm);
		RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_ELBOW, leftElbow);

		// Fly gesture detection.
		flyRightArm = DetectFlyArm(userId, rightArm, rightElbow);
		flyLeftArm = DetectFlyArm(userId, leftArm, leftElbow);
		
		// Shoot gesture detection.
		leftShootGesture = DetectLeftShootGesture(userId, leftArm, leftElbow);
		rightShootGesture = DetectRightShootGesture(userId, rightArm, rightElbow);
		
		// The left shoot gesture is mapped to the ice breath super power.
		if (leftShootGesture)
			breath.emit = true;
		else
			breath.emit = false;
		
		// The right shoot gesture is mapped to the laser super power.
		if (rightShootGesture) {
			leftEye.emit = true;
			rightEye.emit = true;
		}
		else {
			leftEye.emit = false;
			rightEye.emit = false;
		}
		
		// The timer is incread up to 31. Other values can be used to allow or faster or slower
		// toggling between first and third person camera.
		if (coolDownTimer < 31)
			coolDownTimer++;
		
		// This first 'if' statement is accessed if at least one arm is in flight mode.
		if (flyRightArm || flyLeftArm) {
			// A special case if both arms are in flight mode.
			if (flyRightArm && flyLeftArm) {
				// Detect whether both the right and the left arm are in the special 'camera switch' pose.
				if ((rightArm.localEulerAngles.y > 340 | rightArm.localEulerAngles.y < 20) & (rightArm.localEulerAngles.z > 340 | rightArm.localEulerAngles.z < 20)) {
					if ((leftArm.localEulerAngles.y > 340 | leftArm.localEulerAngles.y < 20) & (leftArm.localEulerAngles.z > 340 | leftArm.localEulerAngles.z < 20)) {
						// If both arms are in the correct pose and the timer is at 31, then switch camera.
						if (coolDownTimer > 30) {
                           coolDownTimer = 0;
							if (firstCamera.enabled ==  false) {
                               thirdCamera.enabled = false;
                               firstCamera.enabled = true;
							}
							else {
								firstCamera.enabled = false;
								thirdCamera.enabled = true;
							}
						}
					}
				}
				
				// If both arms are in flight mode, then determine the main fly arm. The direction of this main fly arm will
				// be used to fly the avatar in that direction. The direction of the arm which is closest to the forward direction
				// of the spine will be used as main fly arm.
				if (Vector3.Angle(spine.forward, -rightArm.right) < Vector3.Angle(spine.forward, -leftArm.right))
					mainFlyArm = rightArm;
				else
					mainFlyArm = leftArm;
				
				// XXX Klopt dit?
				mainFlyArm.right = (mainFlyArm.right * 10);
				// Update the avatar's position based on the direction of the main fly arm.
				UpdatePositionInFlight(mainFlyArm,true);

				RotateSpineInFlight(userId, NiteWrapper.SkeletonJoint.TORSO_CENTER);
				// In the case of flying with both arms, the rotations of the shoulders and
				// elbows need to be reset, since they have been incorrectly rotated
				// when the spine was rotated in RotateSpineInFlight.
				RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_SHOULDER, rightArm);
				RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_ELBOW, rightElbow);
				RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_SHOULDER, leftArm);
				RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_ELBOW, leftElbow);
			}
			else if (flyRightArm) {
				mainFlyArm = rightArm;
				// Update the avatar's position based on the direction of the main fly arm.
				UpdatePositionInFlight(mainFlyArm,false);
				// Rotate the spine of the avatar to the direction of the main fly arm in order to
				// let the avatar's body follow the direction of the main fly arm.
				RotateSpineInFlight(userId, NiteWrapper.SkeletonJoint.TORSO_CENTER);
				// The rotations of the right shoulder and elbow needs to be reset,
				// since they have been incorrectly rotated due to the rotation of the spine in RotateSpineInFlight.				
				RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_SHOULDER, rightArm);
				RotateBone(userId, NiteWrapper.SkeletonJoint.RIGHT_ELBOW, rightElbow);
			}
			else if (flyLeftArm) {
				mainFlyArm = leftArm;
				// Update the avatar's position based on the direction of the main fly arm.
				UpdatePositionInFlight(mainFlyArm,false);
				// Rotate the spine of the avatar to the direction of the main fly arm in order to
				// let the avatar's body follow the direction of the main fly arm.
				RotateSpineInFlight(userId, NiteWrapper.SkeletonJoint.TORSO_CENTER);
				// The rotations of the left shoulder and elbow needs to be reset,
				// since they have been incorrectly rotated due to the rotation of the spine in RotateSpineInFlight.	
				RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_SHOULDER, leftArm);
				RotateBone(userId, NiteWrapper.SkeletonJoint.LEFT_ELBOW, leftElbow);
			}
		} // If the user has no arms in fly mode, then the avatar is still able to move
		  // in the small area tracked by the kinect, using MoveBone.
		else
			MoveBone(userId, NiteWrapper.SkeletonJoint.TORSO_CENTER, spine);
			
		// Retrieve the laster gyroscope information.
        VRWrapper.WrapIWRGetTracking(ref yaw, ref pitch, ref roll);
		//Debug.Log(String.Format("yaw: {0}, pitch: {1}, roll: {2}, res = {3}",(yaw/maxRotation)*180, (pitch/maxRotation)*180, (roll/maxRotation)*180, res));	
		
		// Change the rotation of the head according to the gyroscope information.
		headTrans.eulerAngles = new Vector3((pitch / maxRotation) * 180, -((yaw / maxRotation) * 180 + 180), (roll / maxRotation) * 180 + 270);
		headTrans.eulerAngles = Vector3.Slerp(headTrans.eulerAngles, new Vector3((pitch/maxRotation)*180, -((yaw/maxRotation)*180 + 180), (roll/maxRotation)*180 + 270), Time.deltaTime*smoothness);
    }
	
	// This function updates the position of the avatar when in flight.
	public void UpdatePositionInFlight(Transform joint, bool twoArms) {
		// If the user flies with both arms then the current speed is accelerated with steps of 0.1
		// up to the maximum speed of 3. If the user flies with one arm then the current speed
		// is accelerated or decelerated with steps of 0.05 until it is equal to 1.
		if (twoArms) {
			if (currentSpeed < 3)
               currentSpeed += 0.1f;
		} else {
			if (currentSpeed < 1)
				currentSpeed += 0.05f;
			else if (currentSpeed > 1)
				currentSpeed -= 0.1f;
		}

		// The actual update of the position based on the -joint.right position (i.e. the direction of the main fly arm)
		// times the current speed.
		if (Input.GetButton("Teleport") == true)
			spine.position = new Vector3(1043.2F, 49.0F, 700.0F);

		
		if ( spine.position.y <= 1 ){
						spine.position = characterPosition + new Vector3(0,5,0);
		} else
			spine.position -= (joint.right * currentSpeed);
		
		
		// This new position is explicitly saved in the characterPosition variable, which
		// is used as reference once the avatar is not in flight.
		characterPosition = spine.position;
	}

	// Rotates all joints to their initial position.
    public void RotateToInitialPosition() {
        spine.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.TORSO_CENTER];
        rightArm.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_SHOULDER];
        leftArm.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_SHOULDER];
        rightElbow.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_ELBOW];        
		leftElbow.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_ELBOW];
        rightHip.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_HIP];
        leftHip.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_HIP];
        rightKnee.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_KNEE];
        leftKnee.rotation = initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_KNEE];
    }

	// Rotates all joints to their initial position and changes the elbow joints in order
	// to mimic the kinect calibration position.
    public void RotateToCalibrationPose() {
        RotateToInitialPosition();
        //rightElbow.rotation = Quaternion.Euler(0, -90, 90) * initialRotations[(int)NiteWrapper.SkeletonJoint.RIGHT_ELBOW];
        //leftElbow.rotation = Quaternion.Euler(0, 90, -90) * initialRotations[(int)NiteWrapper.SkeletonJoint.LEFT_ELBOW];
    }

	// Retrieves the latest rotation information from the kinect.
    void RotateBone(uint userId, NiteWrapper.SkeletonJoint joint, Transform dest) {
        NiteWrapper.SkeletonJointTransformation trans = new NiteWrapper.SkeletonJointTransformation();
        NiteWrapper.GetJointTransformation(userId, joint, ref trans);

        // Only modify joint if confidence is high enough in this frame.
		// We set it to 0 since we always want the latest rotation information.
        if (trans.ori.confidence > 0.0) {
            // Z coordinate in OpenNI is opposite from Unity. We will create a quaternion
            // to rotate from OpenNI to Unity (relative to initial rotation).
            Vector3 worldZVec = new Vector3(-trans.ori.m02, -trans.ori.m12, trans.ori.m22);
            Vector3 worldYVec = new Vector3(trans.ori.m01, trans.ori.m11, -trans.ori.m21);
            Quaternion jointRotation = Quaternion.LookRotation(worldZVec, worldYVec);

			Quaternion newRotation = jointRotation * initialRotations[(int)joint];

            // Smoothing results in a shaky avatar, but might be useful if it is made to work.
			// dest.rotation = Quaternion.Slerp(dest.rotation, newRotation, Time.deltaTime * 20);
			dest.rotation = newRotation;
        }
	}
	
	// This function moves the avatar around according to the movement of the user relative to the kinect.
	void MoveBone(uint userId, NiteWrapper.SkeletonJoint joint, Transform dest) {
		NiteWrapper.SkeletonJointTransformation trans = new NiteWrapper.SkeletonJointTransformation();
		NiteWrapper.GetJointTransformation(userId, joint, ref trans);

		// Save the first position of the user relative to the kinect once it stopped flying.
		if (firstMoveAfterFlight == true)
			kinectOffsetPosition = new Vector3(trans.pos.x/1000, trans.pos.y/1000 -1, -trans.pos.z/1000);

		// The position of the avatar is based on the characterPosition (i.e. the position where the avatar
		// has flown to) and the movement around the initial position of the user after flight (kinectOffsetPosition)
		// relative to the kinect.
		dest.position = characterPosition - kinectOffsetPosition + new Vector3(trans.pos.x/1000, trans.pos.y/1000 -1, -trans.pos.z/1000);
		Debug.Log("Checking button pressed");
		if(Input.GetButton("Teleport")) {
			spine.position = new Vector3(0,0,0);
			Debug.Log("Trigger key recognized");
		}
	}
	
	// Rotate the spine of the avatar to the direction of the main fly arm.
	void RotateSpineInFlight(uint userId, NiteWrapper.SkeletonJoint joint) {
		NiteWrapper.SkeletonJointTransformation trans = new NiteWrapper.SkeletonJointTransformation();
        NiteWrapper.GetJointTransformation(userId, joint, ref trans);

		// Smooth spine rotation, but currently results in a buggy avatar.
		//spine.forward = Vector3.SmoothDamp(spine.forward, -mainFlyArm.right, ref velocity, smoothTime);
		// Instant spine rotation to the direction of the main fly arm.
		spine.forward = -mainFlyArm.right;
	}
	
	// Method to detect the gesture of a straight arm (needed for flying).
	bool DetectFlyArm(uint userId, Transform arm, Transform elbow) {
		//Debug.Log(String.Format("[{0}] DetectFlyArm: arm: [{1}], elbow: [{2}]", userId, arm.rotation, elbow.rotation));
		// The angle between the elbow's and arm's direction should be less than 30 degrees, otherwise the
		// arm is not stretched enough.
		if (Quaternion.Angle(arm.rotation, elbow.rotation) < 30)
			//Debug.Log(String.Format("Elbow is straight-like: [{0}]", Quaternion.Angle(arm.rotation, elbow.rotation) ));
			// We don't want the avatar to fly down when your arms are just hanging
			// down, so the y angles of the user's arm should not be in between 260 and 310 degrees.
			if (arm.localEulerAngles.y > 310 | arm.localEulerAngles.y < 260)				
				return true;
			else
				return false;
		else
			return false;
	}
	
	// Shooting gesture detection. If the left arm and elbow angles have certain values corresponding to our shoot gesture then return true.
	bool DetectLeftShootGesture(uint userId, Transform arm, Transform elbow) {
		if (arm.localEulerAngles.y < 330 & arm.localEulerAngles.y > 280) {
			if ((elbow.localEulerAngles.x < 15 | elbow.localEulerAngles.x > 345) & (elbow.localEulerAngles.y < 25 | elbow.localEulerAngles.y > 345) & (elbow.localEulerAngles.z < 160 & elbow.localEulerAngles.z > 120)) {
				return true;
			} else
				return false;
		} else
			return false;
	}
	
	// Shooting gesture detection. If the right arm and elbow angles have certain values corresponding to our shoot gesture then return true.
	bool DetectRightShootGesture(uint userId, Transform arm, Transform elbow) {
		if (arm.localEulerAngles.y < 350 & arm.localEulerAngles.y > 310) {
			if ((elbow.localEulerAngles.x < 65 & elbow.localEulerAngles.x > 25) & (elbow.localEulerAngles.y < 45 & elbow.localEulerAngles.y > 5) & (elbow.localEulerAngles.z < 250 & elbow.localEulerAngles.z > 210)) {
				return true;
			} else
				return false;
		} else
			return false;
	}
	
	// Return the current flight speed.
	public float GetFlyingSpeed() {
		return currentSpeed;
	}
}

// The main Unity script class.
public class Nite : MonoBehaviour {
    Texture2D usersLblTex;
    Color[] usersMapColors;
    Rect usersMapRect;
    int usersMapSize;
    short[] usersLabelMap;
    short[] usersDepthMap;
    float[] usersHistogramMap;

	// This array will hold all detected characters.
    Avatar[] characters;
	
    GUIText caption;

    NiteWrapper.UserDelegate NewUser;
    NiteWrapper.UserDelegate CalibrationStarted;
    NiteWrapper.UserDelegate CalibrationFailed;
    NiteWrapper.UserDelegate CalibrationSuccess;
    NiteWrapper.UserDelegate UserLost;

    List<uint> allUsers;
    Dictionary<uint, Avatar> calibratedUsers;

    void OnNewUser(uint UserId) {
        Debug.Log(String.Format("[{0}] New user", UserId));
        allUsers.Add(UserId);
    }   

    void OnCalibrationStarted(uint UserId) {
		Debug.Log(String.Format("[{0}] Calibration started", UserId));
    }

    void OnCalibrationFailed(uint UserId) {
        Debug.Log(String.Format("[{0}] Calibration failed", UserId));
    }

    void OnCalibrationSuccess(uint UserId) {
        Debug.Log(String.Format("[{0}] Calibration success", UserId));
		
        // Associate this user to an unused character avatar.
        for (int i = 0; i < characters.Length; i++) {
            if (!calibratedUsers.ContainsValue(characters[i])) {
                calibratedUsers.Add(UserId, characters[i]);
                break;
            }
        }

        // Should we stop looking for users?
        if (calibratedUsers.Count == characters.Length) {
			Debug.Log("Stopping to look for users");
            NiteWrapper.StopLookingForUsers();
        }
		//set camera  on first person after calibration
		characters[0].firstCamera.enabled = true;
		characters[0].thirdCamera.enabled = false;
    }

    void OnUserLost(uint UserId) {
        Debug.Log(String.Format("[{0}] User lost", UserId));

        // If this was one of our calibrated users, mapped to an avatar.
        if (calibratedUsers.ContainsKey(UserId)) {
            // Reset the avatar and remove from list.
            calibratedUsers[UserId].RotateToCalibrationPose();
            calibratedUsers.Remove(UserId);

            // Should we start looking for users again?
            if (calibratedUsers.Count < characters.Length) {
                Debug.Log("Starting to look for users");
                NiteWrapper.StartLookingForUsers(NewUser, CalibrationStarted, CalibrationFailed, CalibrationSuccess, UserLost);
            }
        }

        // Remove from global users list.
        allUsers.Remove(UserId);
    }

    void Start() {
		uint rc = NiteWrapper.Init(new StringBuilder(".\\OpenNI.xml"));
        if (rc != 0)
            Debug.Log(String.Format("Error initing OpenNI: {0}", Marshal.PtrToStringAnsi(NiteWrapper.GetStatusString(rc))));

        // Init depth & label map related stuff.
        usersMapSize = NiteWrapper.GetDepthWidth() * NiteWrapper.GetDepthHeight();
        usersLblTex = new Texture2D(NiteWrapper.GetDepthWidth(), NiteWrapper.GetDepthHeight());
        usersMapColors = new Color[usersMapSize];
        usersMapRect = new Rect(Screen.width - usersLblTex.width / 2, Screen.height - usersLblTex.height / 2, usersLblTex.width / 2, usersLblTex.height / 2);
        usersLabelMap = new short[usersMapSize];
        usersDepthMap = new short[usersMapSize];
        usersHistogramMap = new float[5000];

        // GUI text.
        caption = GameObject.Find("GUI Text").guiText;

        // Init our avatar controllers. In our case we have only 1 character.
        characters = new Avatar[1];
		characters[0] = new Avatar(GameObject.Find("Superman"));
		
        // Init user lists - one will contain all users, the second will contain only calibrated & mapped users.
        allUsers = new List<uint>();
        calibratedUsers = new Dictionary<uint, Avatar>();
        
        // Init user callbacks.
        NewUser = new NiteWrapper.UserDelegate(OnNewUser);
        CalibrationStarted = new NiteWrapper.UserDelegate(OnCalibrationStarted);
        CalibrationFailed = new NiteWrapper.UserDelegate(OnCalibrationFailed);
        CalibrationSuccess = new NiteWrapper.UserDelegate(OnCalibrationSuccess);
        UserLost = new NiteWrapper.UserDelegate(OnUserLost);

        // Start looking.
        NiteWrapper.StartLookingForUsers(NewUser, CalibrationStarted, CalibrationFailed, CalibrationSuccess, UserLost);
		Debug.Log("Waiting for users to calibrate");
		
		// Set default smoothing.
		NiteWrapper.SetSkeletonSmoothing(0.6);	
	}
	
	void Update() {
        // Next NITE frame.
		NiteWrapper.Update(false);

        // Update the visual user map which is overlaid on the screen. This makes calibration easier as the user
		// sees what the kinect detects, but creating this user map is quite computationally expensive so it is best
		// to don't use it if you don't need it.
		/*
		if (calibratedUsers.Count < characters.Length)
			UpdateUserMap();
		*/

        // Update the avatars.
        foreach (KeyValuePair<uint, Avatar> pair in calibratedUsers)
            pair.Value.UpdateAvatar(pair.Key);
	}
	
	// Quite the Nite and iWear wrappers.
	void OnApplicationQuit() {
		NiteWrapper.Shutdown();
		VRWrapper.WrapIWRCloseTracker();
	}

    void OnGUI() {
        if (calibratedUsers.Count < characters.Length) {
            GUI.DrawTexture(usersMapRect, usersLblTex);
            caption.text = String.Format("Waiting for {0} more users to calibrate...", characters.Length - calibratedUsers.Count);
        } else
            caption.text = "All users calibrated";

		// We user the pause pose as the switch camera gesture, so we don't use this piece of code.
		/*
        foreach (uint userId in allUsers) {
            float progress = NiteWrapper.GetUserPausePoseProgress(userId);
            if (NiteWrapper.GetUserPausePoseProgress(userId) > 0.0) {
                caption.text = String.Format("[User {0}] Pause pose progress: {1}", userId, progress);
                break;
            }
        }*/
    }

	// Creates the user map of what is seen by the kinect.
    void UpdateUserMap() {
        // copy over the maps
        Marshal.Copy(NiteWrapper.GetUsersLabelMap(), usersLabelMap, 0, usersMapSize);
        Marshal.Copy(NiteWrapper.GetUsersDepthMap(), usersDepthMap, 0, usersMapSize);

        // we will be flipping the texture as we convert label map to color array
        int flipIndex, i;
        int numOfPoints = 0;
		Array.Clear(usersHistogramMap, 0, usersHistogramMap.Length);

        // calculate cumulative histogram for depth
        for (i = 0; i < usersMapSize; i++) {
            // only calculate for depth that contains users
            if (usersLabelMap[i] != 0) {
                usersHistogramMap[usersDepthMap[i]]++;
                numOfPoints++;
            }
        }
        if (numOfPoints > 0) {
            for (i = 1; i < usersHistogramMap.Length; i++)
		        usersHistogramMap[i] += usersHistogramMap[i-1];
            for (i = 0; i < usersHistogramMap.Length; i++)
                usersHistogramMap[i] = 1.0f - (usersHistogramMap[i] / numOfPoints);
        }

        // create the actual users texture based on label map and depth histogram
        for (i = 0; i < usersMapSize; i++) {
            flipIndex = usersMapSize - i - 1;
            if (usersLabelMap[i] == 0)
                usersMapColors[flipIndex] = Color.clear;
            else {
                // create a blending color based on the depth histogram
                Color c = new Color(usersHistogramMap[usersDepthMap[i]], usersHistogramMap[usersDepthMap[i]], usersHistogramMap[usersDepthMap[i]], 0.9f);
                switch (usersLabelMap[i] % 4) {
                    case 0:
                        usersMapColors[flipIndex] = Color.red * c;
                        break;
                    case 1:
                        usersMapColors[flipIndex] = Color.green * c;
                        break;
                    case 2:
                        usersMapColors[flipIndex] = Color.blue * c;
                        break;
                    case 3:
                        usersMapColors[flipIndex] = Color.magenta * c;
                        break;
                }
            }
        }

        usersLblTex.SetPixels(usersMapColors);
        usersLblTex.Apply();
    }
	
	public float GetFlyingSpeed() {
		return characters[0].GetFlyingSpeed();
	}
}
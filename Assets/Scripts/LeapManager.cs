using UnityEngine;
using System.Collections;
using Leap;

public class LeapManager : MonoBehaviour
{
	public static float ROTATE_MOD = 3.14F;

	public static float _forwardFingerContraint = 0.7f; // Min result of dot product between finger direction and hand direction to determine if finger is facing forward.

	public Camera _mainCam; //Not required to be set. Defaults to camera tagged "MainCamera".

	//member variables;
	private static Controller _leapController = new Controller();
	private static Frame _currentFrame = Frame.Invalid;
	private static bool _pointerAvailible = false;
	// Is Leap Motion detect a hand ?
	private static bool _handavailible = false;
	private Vector2 _pointerPositionScreen = new Vector3(0,0);
	private Vector3 _pointerPositionWorld = new Vector3(0,0,0);
	private Vector3 _pointerPositionScreenToWorld = new Vector3(0,0,0);
	private float _screenToWorldDistance = 10.0f;

	private static Frame _PreFrame = Frame.Invalid;
	private static Hand _hand = Hand.Invalid;
	private static Hand _PreHand = Hand.Invalid;
	public static bool isHaveHand = false;

	//Accessors
	/*
	 * A direct reference to the Controller for accessing the LeapMotion data yourself rather than going through the helper.
	 */
	public Controller leapController{
		get { return _leapController; }
	}

	/*
	 * The most recent frame of data from the LeapMotion controller.
	 */
	public Frame currentFrame
	{
		get { return _currentFrame; }
	}

	// need?
	public static Hand Hand
	{
		get	{ return _hand;	}
	}

	/*
	 * Is there a pointing finger currently tracked in the scene.
	 */
	public bool pointerAvailible 
	{
		get{ return _pointerAvailible; }
	}

	/*
	 * Is there a hand detect by Leap Motion
	 */
	public bool handAvailible
	{
		get{ return _handavailible; }
	}

	/*
	 * The currently tracked (if any) pointing finger in screen space.
	 */
	public Vector2 pointerPositionScreen 
	{
		get { return _pointerAvailible ? _pointerPositionScreen : Vector2.zero; }
	}

	/*
	 * The currently tracked (if any) pointing finger in world space.
	 */
	public Vector3 pointerPositionWorld 
	{
		get { return _pointerAvailible ? _pointerPositionWorld : Vector3.zero; }
	}

	/*
	 * The screen position of the currently tracked (if any) pointing finger projected into world space
	 * at a distance of [screenToWorldDistance].
	 */
	public Vector3 pointerPositionScreenToWorld 
	{
		get { return _pointerPositionScreenToWorld; }
	}

	/*
	 * The projection distance for the pointerPositionScreenToWorld calculation. 
	 * Default Value is 10.0f
	 */
	public float screenToWorldDistance {
		get { return _screenToWorldDistance; }
		set { _screenToWorldDistance = value; }
	}

	//Public Static Functions

	/*
	 * Returns the most likely finger to be pointing on the given hand. 
	 * Returns Finger.Invalid if no such finger exists.
	 */
	public static Finger pointingFigner(Hand hand)
	{
		Finger forwardFinger = Finger.Invalid;
		ArrayList forwardFingers = forwardFacingFingers(hand);
		
		if(forwardFingers.Count > 0)
		{
			
			float minZ = float.MaxValue;
			
			foreach(Finger finger in forwardFingers)
			{
				if(finger.TipPosition.z < minZ)
				{
					minZ = finger.TipPosition.z;
					forwardFinger = finger;
				}
			}
		}
		
		return forwardFinger;
	}

	/*
	 * Returns a list of fingers whose position is in front 
	 * of the hand (relative to the hand direction). 
	 * 
	 * This is most useful in trying to lower the chances 
	 * of detecting a thumb (though not a perfect method).
	 */
	public static ArrayList forwardFacingFingers(Hand hand)
	{
		ArrayList forwardFingers = new ArrayList();
		
		foreach(Finger finger in hand.Fingers)
		{
			if(isForwardRelativeToHand(finger, hand)) { forwardFingers.Add(finger); }
		}
		
		return forwardFingers;
	}

	/*
	 * Returns whether or not the given hand is open.
	 */
	public static bool isHandOpen(Hand hand)
	{
		return hand.Fingers.Count > 2;
	}

	public static bool isForwardRelativeToHand(Pointable item, Hand hand)
	{
		return Vector3.Dot((item.TipPosition.ToUnity() - hand.PalmPosition.ToUnity()).normalized, hand.Direction.ToUnity()) > _forwardFingerContraint;
	}

	// Unity Monobehavior Defenitions

	/*
	 * If the _mainCam isn't overridden, 
	 * find the camera with the "MainCamera" tag.
	 */
	void Start () 
	{
		if(_mainCam == null)
		{
			_mainCam = (GameObject.FindGameObjectWithTag("MainCamera") as GameObject).GetComponent(typeof(Camera)) as Camera;
		}
		//Debug.Log(_mainCam);
	}
	
	/*
	 * get latest frame called each second
	 * Set the pointer world and screen positions each frame.
	 */
	void Update () 
	{
		_currentFrame = _leapController.Frame();
		_PreFrame = _leapController.Frame(1);
		if(_currentFrame.Hands.Count > 0)
		{
			isHaveHand = true;
			_hand = _currentFrame.Hands[0];
		}
		else
		{
			isHaveHand = false;
		}
		if(_PreFrame != null)
		{
			if(_PreFrame.Hands.Count > 0)
			{
				_PreHand = _PreFrame.Hands[0];
			}
		}

		//=========
		Hand primeHand = frontmostHand();
		
		Finger primeFinger = Finger.Invalid;
		
		if(primeHand.IsValid)
		{
			_handavailible = true;

			primeFinger = pointingFigner(primeHand);
			
			if(primeFinger.IsValid) 
			{ 
				_pointerAvailible = true; 
				
				_pointerPositionWorld = primeFinger.TipPosition.ToUnityTranslated();
				// when switch scene, the _mainCam will be null
				if(_mainCam == null)
				{
					GameObject gos; 
					gos = GameObject.FindGameObjectWithTag("MainCamera");
					if(gos != null)
					{
						_mainCam = gos.GetComponent(typeof(Camera)) as Camera;
					}
					//_mainCam = (GameObject.FindGameObjectWithTag("MainCamera") as GameObject).GetComponent(typeof(Camera)) as Camera;
				}
				if(_mainCam != null)
				{
					_pointerPositionScreen = _mainCam.WorldToScreenPoint(_pointerPositionWorld);
					_pointerPositionScreenToWorld = _mainCam.ScreenToWorldPoint(new Vector3(pointerPositionScreen.x,
				                                                                        pointerPositionScreen.y,
				                                                                        _screenToWorldDistance));
				}
			}
			else
			{ 
				_pointerAvailible = false; 
			}
		}
		else
		{
			_handavailible = false;
		}
	}

	//Public Instance Methods
	
	/*
	 * Get the screen coordinates of all the tracked fingers 
	 * on the given hand.
	 */
	public Vector2[] getScreenFingerPositions(Hand hand)
	{
		Vector2[] retArr = new Vector2[hand.Fingers.Count];
		
		for(int i=0;i<hand.Fingers.Count;i++) { retArr[i] = leapPositionToScreen(hand.Fingers[i].TipPosition); }
		
		return retArr;
	}
	
	/*
	 * Get the world coordinates of all the tracked 
	 * fingers on the given hand.
	 */
	public Vector3[] getWorldFingerPositions(Hand hand)
	{
		Vector3[] retArr = new Vector3[hand.Fingers.Count];
		
		for(int i=0;i<hand.Fingers.Count;i++) { retArr[i] = hand.Fingers[i].TipPosition.ToUnityTranslated(); }
		
		return retArr;
	}
	
	/*
	 * Take a Leap.Vector and convert it to Screen coordinates.
	 * 
	 * Exmaple: Vector3 handScreenPosition = _leapManager.leapPositionToScreen(_leapManager.frontmostHand().palmPosition);
	 */
	public Vector2 leapPositionToScreen(Vector leapVector)
	{
		return _mainCam.WorldToScreenPoint(leapVector.ToUnityTranslated());
	}
	
	/*
	 * Get the frontmost detected hand in the scene. 
	 * Returns Leap.Hand.Invalid if no hands are being tracked.
	 */
	public Hand frontmostHand()
	{
		float minZ = float.MaxValue;
		Hand forwardHand = Hand.Invalid;
		
		foreach(Hand hand in _currentFrame.Hands)
		{
			if(hand.PalmPosition.z < minZ)
			{
				minZ = hand.PalmPosition.z;
				forwardHand = hand;
			}
		}
		
		return forwardHand;
	}
}
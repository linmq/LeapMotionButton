using UnityEngine;
using System.Collections;

public class FingerBehavior : MonoBehaviour 
{
	private LeapManager _leapManager;
	private GameObject[] arrowTexture;

	// Use this for initialization
	void Start () 
	{
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;

		//DontDestroyOnLoad(_leapManager);

		arrowTexture = GameObject.FindGameObjectsWithTag("ArrowTexture");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_leapManager)
		{
			if(_leapManager.pointerAvailible && !LeapManager.isHandOpen(_leapManager.frontmostHand()) /* && !any_menu_active*/)
			{
				arrowTexture[0].SetActive(true);
				
				gameObject.transform.position = new Vector3( _leapManager.pointerPositionScreen.x/Camera.main.pixelWidth, _leapManager.pointerPositionScreen.y/Camera.main.pixelHeight, 0.0f );
			}
			else
			{
				arrowTexture[0].SetActive(false);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class ButtonsManager : MonoBehaviour 
{
	private GUIButton m_button1;
	private GUIButton m_button2;
	private int m_SelectedButtonID;
	private LeapManager _leapManager;

	// Use this for initialization
	void Start () 
	{
		m_button1 = this.transform.FindChild("button_1").GetComponent<GUIButton>();
		m_button2 = this.transform.FindChild("button_2").GetComponent<GUIButton>();

		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
	}

	// Update is called once per frame
	void Update () 
	{
		if(_leapManager)
		{
			if(_leapManager.pointerAvailible && !LeapManager.isHandOpen(_leapManager.frontmostHand()))
			{
				Vector2 pointerPositionScreen = _leapManager.pointerPositionScreen;
				pointerPositionScreen.y = pointerPositionScreen.y+30;
				
				m_button1.UpdateState(pointerPositionScreen);
				m_button2.UpdateState(pointerPositionScreen);
			}
		}
	}
}

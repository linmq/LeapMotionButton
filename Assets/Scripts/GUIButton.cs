using UnityEngine;
using System.Collections;

public class GUIButton : MonoBehaviour 
{
	private bool bShowBloodBar = false;
	private float tmpValue = 0.0f;
	private Rect rctBloodBar;
	public GUISkin theBloodBarSkin;

    // button state
    protected enum StateID
    {
        NORMAL=0,
        HOVER,
    }
    protected StateID m_state = StateID.NORMAL;
	
    public Texture[] m_ButtonSkin;

    // button id
    public int m_ID = 0;

    private Vector2 m_screenPosition;

    private GUITexture m_texture;
   
    void Awake()
    {
        m_texture = this.guiTexture;

        m_screenPosition = new Vector3(m_texture.pixelInset.x, m_texture.pixelInset.y, 0);

        // set default state
        SetState(StateID.NORMAL);
    }

	void Start()
	{
		rctBloodBar = new Rect (20,20,200,20);
	}

	void OnGUI()
	{
		GUI.skin = theBloodBarSkin; 

		if(bShowBloodBar)
		{
			GUI.HorizontalScrollbar(rctBloodBar, 0.0f, tmpValue, 0.0f, 1.0f, GUI.skin.GetStyle("HorizontalScrollbar"));
		}
	}

	void Update()
	{

	}

	public void AddBlood()
	{
		tmpValue += 0.01f;
		
		if(tmpValue > 1.0f)
		{
			tmpValue = 1.0f;
			if( 1 == m_ID )
			{
				Debug.Log( "Button 1" );
			}
			else if( 2 == m_ID )
			{
				Debug.Log( "Button 2" );
			}
		}
	}
	
	public void Reset()
	{
		tmpValue = 0.0f;
	}
	
    public void UpdateState(Vector3 mousepos)
    {
        if (m_texture.HitTest(mousepos))
        {
        	SetState(StateID.HOVER);
			Rect r = Camera.main.pixelRect;
			Vector3 viewPos = gameObject.transform.position;
			rctBloodBar = new Rect( viewPos.x * r.width, (1-viewPos.y) * r.height, 128, 128);
			bShowBloodBar = true;
			AddBlood();
        }
        else
        {
            SetState(StateID.NORMAL);

			bShowBloodBar = false;
			Reset();
        }
    }
	
    protected virtual void SetState(StateID state)
    {
        if (m_state == state)
		{
            return;
		}

        m_state = state;

        m_texture.texture = m_ButtonSkin[(int)m_state];

        float w = m_ButtonSkin[(int)m_state].width;
        float h = m_ButtonSkin[(int)m_state].height;

        m_texture.pixelInset = new Rect(this.m_screenPosition.x, m_screenPosition.y, w, h);
    }
}

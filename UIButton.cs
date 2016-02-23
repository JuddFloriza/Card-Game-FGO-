using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButton
{
	public delegate void ButtonFunction ();
	public delegate void ReleaseFunction ();

	private enum InputStates
	{
		Clicked,
		Release,
		Size,
	}

	private GameObject 		m_obj 				= null;
	private BoxCollider 	m_objCol 			= null;
	private BoxCollider2D 	m_boxCol 			= null;
	private ButtonFunction 	m_fnc 				= null;
	private	ReleaseFunction	m_releaseFnc		= null;
	private Sprite 			m_spritePressed 	= null;
	private Sprite			m_spriteRelease		= null;
	private InputStates 	m_input 			= InputStates.Size;

	public ButtonFunction OnClickButtonFunction
	{
		get 
		{
			return m_fnc;
		}
		set 
		{
			m_fnc = value;
		}
	}

	public ReleaseFunction OnButtonReleaseFunction
	{
		get
		{
			return m_releaseFnc;
		}
		set
		{
			m_releaseFnc = value;
		}
	}
	/// <summary>
	/// Initializes a new instance of the UILib Button"/> class.
	/// </summary>
	/// <param name="prefab"> prefab = button game object.</param>
	public UIButton (GameObject obj, Sprite onPressedSprite = null)
	{
		m_obj = obj;
		m_boxCol = m_obj.GetComponent<BoxCollider2D> ();
		m_input = InputStates.Release;
		m_spritePressed = onPressedSprite;
		if(onPressedSprite != null)
			m_spriteRelease = m_obj.GetComponent<SpriteRenderer> ().sprite;
	}

	public void RunButtonFunction ()
	{
		if (Input.GetMouseButton (0)) 
		{
			if (m_input == InputStates.Release) 
			{
				m_input = InputStates.Clicked;
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pos = new Vector3 (pos.x, pos.y, 0);
				if (m_boxCol.bounds.Contains (pos)) 
				{
					if(m_spritePressed != null)
						m_obj.GetComponent<SpriteRenderer>().sprite = m_spritePressed;
					if (m_fnc != null) 
						m_fnc ();
				}
			}
		} 
		else if(Input.GetMouseButtonUp(0))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos = new Vector3 (pos.x, pos.y, 0);
			if (m_boxCol.bounds.Contains (pos)) 
			{
				if (m_releaseFnc != null) 
					m_releaseFnc();
			}
		}
		else 
		{
			if(m_spritePressed != null)
				m_obj.GetComponent<SpriteRenderer>().sprite = m_spriteRelease;
			m_input = InputStates.Release;
		}
	}

}

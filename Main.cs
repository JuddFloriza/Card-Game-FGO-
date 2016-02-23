using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Main : MonoBehaviour 
{
	// Use this for initialization
	private static Main s_main = null;
	private CardGame m_cardGame = null;

	private void Awake ()
	{
		s_main = this;
	}

	private const float IDEAL_WIDTH		= 768;
	private const float IDEAL_HEIGHT	= 1024;

	private static float s_screenWidth = IDEAL_WIDTH;
	private static float s_screenHeight = IDEAL_HEIGHT;

	public static Main Instance
	{
		get{ return s_main;}
	}

	public static float Screenwidth
	{
		get{ return Screen.width; }
	}

	public static float Screenheight
	{
		get{ return Screen.height; }
	}

	public Vector3 AspectScale
	{
		get { return Vector3.one * (IsLandscape ? Screen.width / IDEAL_WIDTH : Screen.height / IDEAL_HEIGHT); }
	}

	public bool IsLandscape
	{
		get { return Screen.width > Screen.height;}
	}

	private void Start () 
	{
		m_cardGame = new CardGame ();
		m_cardGame._Initialize ();
//		if (Camera.main != null && Camera.main.orthographic)
//			Camera.main.orthographicSize = s_screenHeight * 0.5f;

	}
	
	// Update is called once per frame
	private void Update () 
	{
		m_cardGame._Update (Time.deltaTime);
	}
}

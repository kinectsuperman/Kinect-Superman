// Make the script also execute in edit mode
@script  ExecuteInEditMode()

var gSkin : GUISkin;
var backdrop : Texture2D;
private var isLoading = false; // if true, we'll display the "Loading..." message.

function OnGUI()
{
	if (gSkin)
		GUI.skin = gSkin;
	else
		Debug.Log("StartMenuGUI: GUI Skin object missing!");
		
	var backgroundStyle : GUIStyle = new GUIStyle();
	backgroundStyle.normal.background = backdrop;
	GUI.Label ( Rect( (Screen.width - (Screen.height * 2)) * 0.75, 0, Screen.height * 2,Screen.height), "", backgroundStyle);
	
	//Title
	GUI.Label ( Rect( (Screen.width/2)-440, 50, 400, 100), "Immersive Virtual Reality: ISLA Man", "mainMenuTitle");
	//Subtitle
	GUI.Label ( Rect( (Screen.width/2)-440, 120, 400, 100), "by:\n Sicco van Sas,\n Maarten van der velden \n & Daniel Karavolos", "mainMenuSubTitle");
	
	//Play button
	if (GUI.Button( Rect( (Screen.width/2)-70, Screen.height - 160, 140, 70), "Play"))
	{
		isLoading = true;
		Application.LoadLevel("SupermanScene"); // load the game level.
	}
	
	//Quit button
	var isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer);
	if (!isWebPlayer)
	{
		if (GUI.Button( Rect( (Screen.width/2)-70, Screen.height - 80, 140, 70), "Quit"))
		Application.Quit();
	}
	
	// show loading when play is pressed
	if (isLoading)
		GUI.Label ( Rect( (Screen.width/2)-110, (Screen.height / 2) - 60, 400, 70), "Loading...", "mainMenuTitle");
}

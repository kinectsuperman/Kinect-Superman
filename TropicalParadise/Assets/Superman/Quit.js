
function Update () {
Debug.Log("quitting");
	if(Input.GetButton("Quit") ){
		var isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer);
		if (!isWebPlayer)
			Application.Quit();
		else
			Application.LoadLevel("Startmenu");
	}
}
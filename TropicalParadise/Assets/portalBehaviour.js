var startposition : Vector3 = Vector3(0,0,0);
private var triggered : System.Boolean = false;
function OnTriggerEnter (other : Collider) {
	Debug.Log("Portal triggered");
	triggered = true;
	other.transform.root.position = startposition;
}
/*
function LateUpdate () {
	if(triggered) {
		var superman : GameObject = GameObject.Find("Superman");
		if(superman.GetComponent("Nite"))
			superman.setCharacterPosition(startposition);
		else
			superman.transform.root.position = startposition;
		triggered = false;
		Debug.Log("Late Update executed");
	}

}*/

//other.transform.root.position = startposition; is genoeg als de kinect niet aangesloten is.

function LateUpdate () {
	if(triggered) {
		var superman : GameObject = GameObject.Find("Superman");
		//superman.transform.root.position = startposition;
		superman.transform.Translate(Time.deltaTime * 100,0, Time.deltaTime * 100);
		//triggered = false;
		Debug.Log("Late Update executed");
		triggered = false;
	}

}
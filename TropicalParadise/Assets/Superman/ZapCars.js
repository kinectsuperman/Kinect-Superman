@script ExecuteInEditMode()

private var blueCount : int = 0;
private var greenCount : int = 0;
private var purpleCount : int = 0;
public var blueMax : int = 10;
public var greenMax : int = 10;
public var purpleMax : int = 5;
private var incrementGreen: boolean;
private var incrementBlue: boolean;
private var incrementPurple: boolean;
private var hitLaraCroft: boolean;
private var hitVader: boolean;
private var hitSuperman: boolean;
private var bonusCounter: int = 3;

private var firstHit : boolean = false;
//private vaer blueTimer : int = 0;
var gSkin : GUISkin;

function OnParticleCollision (other : GameObject) {
	Debug.Log("Zapping Car");
	var name : String = other.transform.name;
	
	if(!firstHit) {
		firstHit = true;
		
		if (name== "SubaruLegacy") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			greenCount ++;
			incrementGreen = true;
		}
		
		if (name== "SubaruLegacyBlue") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			blueCount++;
			incrementBlue = true;
		}
		
		if (name== "SubaruLegacyPurple") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			purpleCount ++;
			incrementPurple = true;
		}
		
		if(name == "laraCroft") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			hitLaraCroft = true;
			bonusCounter--;
		}
	
		if(name == "darthVader") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			hitVader = true;
			bonusCounter--;
		}
		
			if(name == "evilSuperman") {
			yield WaitForSeconds(2);
			other.SetActiveRecursively(false);
			hitSuperman = true;
			bonusCounter--;
		}
		
		firstHit = false;
	}
}

function OnGUI() {
	//Debug.Log("Zap Cars onGUI called");
	if (gSkin)
		GUI.skin = gSkin;
	else
		Debug.Log("Zap GUI: GUI Skin object missing!");
		
	GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 100), 400, 100), "Blue cars zapped: " + blueCount + "/" + blueMax, "zapMessageBlue"); //
	GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 80), 400, 100), "Green cars zapped: " + greenCount + "/" +greenMax, "zapMessageGreen"); 
	GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 60), 400, 100), "Purple cars zapped: " + purpleCount + "/" + purpleMax, "zapMessagePurple");
	
	if(hitLaraCroft)
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 120), 400, 100), "Bonus: Zapped Lara Croft!", "zapMessageLara");
		
	if(hitVader)
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 140), 400, 100), "Bonus: Zapped Darth Vader!", "zapMessageVader");
		
	if(hitSuperman)
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 160), 400, 100), "Bonus: Zapped Evil Superman!", "zapMessageSuperman");
	
	if(blueCount == blueMax && greenCount == greenMax && purpleCount == purpleMax) {
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-200, 400, 100), "Level Completed", "mainMenuTitle");
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-150, 400, 100), bonusCounter + " hidden objectives remaining", "mainMenuSubTitle");
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-130, 400, 100), "Press 'q' to quit.", "mainMenuSubTitle");
	}
	
	if(Input.GetButton("Complete")) {
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 100), 400, 100), "Blue cars zapped: " + blueMax + "/" + blueMax, "zapMessageBlue"); //
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 80), 400, 100), "Green cars zapped: " + greenMax + "/" +greenMax, "zapMessageGreen"); 
		GUI.Label ( Rect( (Screen.width/2-500), (Screen.height - 60), 400, 100), "Purple cars zapped: " + purpleMax + "/" + purpleMax, "zapMessagePurple");
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-200, 400, 100), "Level Completed", "mainMenuTitle");
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-150, 400, 100), bonusCounter + " hidden objectives remaining", "mainMenuSubTitle");
		GUI.Label ( Rect( (Screen.width/2)-200, (Screen.height/2)-130, 400, 100), "Press 'q' to quit.", "mainMenuSubTitle");
	}
	
}
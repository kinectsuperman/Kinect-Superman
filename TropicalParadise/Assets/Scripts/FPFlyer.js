var speed = 6.0;
var jumpSpeed = 8.0;
var gravity = 20.0;

private var moveDirection = Vector3.zero;
private var grounded : boolean = false;

function FixedUpdate() {
	my=moveDirection.y;

	moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	moveDirection = transform.TransformDirection(moveDirection);
	
	if (grounded) {
		// We are grounded, so recalculate movedirection directly from axes
		moveDirection *= speed;
		
	} else {
		moveDirection *= speed+transform.position.y/5;

		moveDirection.y=my;		
	}

	if (Input.GetButton ("Jump")) {
		moveDirection.y = jumpSpeed;
	}

	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	var flags = controller.Move(moveDirection * Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
}

@script RequireComponent(CharacterController)
private var myWalker: FPSWalker=null;

var maxHeight: float=250;

function Start () {
	myWalker = gameObject.GetComponent(FPSWalker);	
}


function Update () {
	if(Input.GetKey("left shift")) {
		myWalker.gravity=-20;
		//myWalker.grounded=true;
	} else {
		myWalker.gravity=10;	
	}


	if (transform.position.y > maxHeight) {
		myWalker.gravity = 20;	
	}


}

@script RequireComponent(FPSWalker)
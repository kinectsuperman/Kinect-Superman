var acceleration = 5.00;
var turning = 3.00;

var maxIdleTime = 4.00;
var seekPlayerTime = 6.00;
var scaredTime = 4.00;
var fishingTime = 30.00;

var shyDistance = 10.00;
var scaredDistance = 5.00;

var strechNeckProbability = 10.00;

var fishWalkSpeed = 1.00;
var walkSpeed = 1.00;
var runSpeed = 1.00;

private var status = HeronStatus.Idle;

private var fishWalkAnimSpeed = 0.50;
private var walkAnimSpeed = 2.00;
private var runAnimSpeed = 9.00;

private var minHeight = 34.1;
private var maxHeight = 42.00;
private var colliders : HeronCollider[];

private var hitTestDistanceIncrement = 1.00;
private var hitTestDistanceMax = 50.00;
private var hitTestTimeIncrement = 0.2;

private var myT : Transform;
private var anim : Animation;
private var leftKnee : Transform;
private var leftAnkle : Transform;
private var leftFoot : Transform;
private var rightKnee : Transform;
private var rightAnkle : Transform;
private var rightFoot : Transform;

private var player : Transform;
private var terrain : TerrainData;

private var offsetMoveDirection : Vector3;
private var usedMoveDirection : Vector3;
private var velocity : Vector3;
private var forward : Vector3;
private var strechNeck = false;
private var fishing = false;
private var lastSpeed = 0.00;

enum HeronStatus {Idle = 0, Walking = 1, Running = 2}

function Start()
{
	forward = transform.forward;
	
	obj = GameObject.FindWithTag("Player");
	player = obj.transform;
	myT = transform;
	terr = Terrain.activeTerrain;
	if(terr)
		terrain = terr.terrainData;
	
	anim = GetComponentInChildren(Animation);
	anim["Walk"].speed = walkSpeed;
	anim["Run"].speed = runSpeed;
	anim["FishingWalk"].speed = fishWalkSpeed;
	
	leftKnee = myT.FindChild("HeronAnimated/MasterMover/RootDummy/Root/Lhip/knee2");
	leftAnkle = leftKnee.FindChild("ankle2");
	leftFoot = leftAnkle.FindChild("foot2");
	rightKnee = myT.FindChild("HeronAnimated/MasterMover/RootDummy/Root/Rhip/knee3");
	rightAnkle = rightKnee.FindChild("ankle3");
	rightFoot = rightAnkle.FindChild("foot3");
	
	colliders =  FindObjectsOfType(HeronCollider);
	
	MainLoop();
	MoveLoop();
	AwareLoop();
}

function MainLoop()
{
	while(true)
	{
		
		yield SeekPlayer();
		yield Idle();
		yield Fish();	
	}
}

function SeekPlayer()
{
	var time = 0.00;
	while(time < seekPlayerTime)
	{
		var moveDirection = player.position - myT.position;
		
		if(moveDirection.magnitude < shyDistance)
		{
			yield;
			return;
		}
		
		moveDirection.y = 0;
		moveDirection = (moveDirection.normalized + (myT.forward * 0.5)).normalized;
		offsetMoveDirection = GetPathDirection(myT.position, moveDirection);
		
		if(offsetMoveDirection != Vector3.zero) status = HeronStatus.Walking;
		else status = HeronStatus.Idle;
		
		yield WaitForSeconds(hitTestTimeIncrement);
		time +=	hitTestTimeIncrement;
	}
}

function Idle ()
{
	strechNeck = false;
	var time = 0.00;
	while(time < seekPlayerTime)
	{	
		if(time > 0.6) strechNeck = true;
		
		status = HeronStatus.Idle;
		offsetMoveDirection = Vector3.zero;
		
		yield WaitForSeconds(hitTestTimeIncrement);
		time +=	hitTestTimeIncrement;
	}
}

function Scared ()
{
	var dist = (player.position - myT.position).magnitude;
	if(dist > scaredDistance) return;
	
	var time = 0.00;
	
	while(time < scaredTime)
	{
		var moveDirection = myT.position - player.position;
		
		if(moveDirection.magnitude > shyDistance * 1.5)
		{
			yield;
			return;
		}
		
		moveDirection.y = 0;
		moveDirection = (moveDirection.normalized + (myT.forward * 0.5)).normalized;
		offsetMoveDirection = GetPathDirection(myT.position, moveDirection);
		
		if(offsetMoveDirection != Vector3.zero) status = HeronStatus.Running;
		else status = HeronStatus.Idle;
		
		yield WaitForSeconds(hitTestTimeIncrement);
		time +=	hitTestTimeIncrement;
	}
}

function Fish()
{
	var height : float = terrain.GetInterpolatedHeight(myT.position.x / terrain.size.x, myT.position.z / terrain.size.z);
	status = HeronStatus.Walking;
	var direction : Vector3;
	var randomDir : Vector3 = Random.onUnitSphere;
	
	if(height > 40)
	{
		maxHeight = 40;
		offsetMoveDirection = GetPathDirection(myT.position, randomDir);
		yield WaitForSeconds(0.5);
		if(velocity.magnitude > 0.01) direction = myT.right * (Random.value > 0.5 ? -1 : 1);
	}
	if(height > 38)
	{
		maxHeight = 38;
		offsetMoveDirection = GetPathDirection(myT.position, randomDir);
		yield WaitForSeconds(1);
		if(velocity.magnitude > 0.01) direction = myT.right * (Random.value > 0.5 ? -1 : 1);
	}
	if(height > 36.5)
	{
		maxHeight = 36.5;
		offsetMoveDirection = GetPathDirection(myT.position, randomDir);
		yield WaitForSeconds(1.5);
		if(velocity.magnitude > 0.01) direction = myT.right * (Random.value > 0.5 ? -1 : 1);
	}
	while(height > 35)
	{
		maxHeight = 35;
		yield WaitForSeconds(0.5);
		if(velocity.magnitude > 0.01) direction = myT.right * (Random.value > 0.5 ? -1 : 1);
		offsetMoveDirection = GetPathDirection(myT.position, randomDir);
		height = terrain.GetInterpolatedHeight(myT.position.x / terrain.size.x, myT.position.z / terrain.size.z);
	}
	
	fishing = true;
	status = HeronStatus.Walking;
	yield WaitForSeconds(fishingTime / 3);
	status = HeronStatus.Idle;
	yield WaitForSeconds(fishingTime / 6);
	status = HeronStatus.Walking;
	yield WaitForSeconds(fishingTime / 3);
	status = HeronStatus.Idle;
	yield WaitForSeconds(fishingTime / 6);
	fishing = false;
	
	maxHeight = 42;
}

function AwareLoop ()
{
	while(true)
	{
		dist = (player.position - myT.position).magnitude;
		
		if(dist < scaredDistance && status != HeronStatus.Running)
		{
			StopCoroutine("Fish");
			maxHeight = 42;
			StopCoroutine("Idle");
			strechNeck = false;
			StopCoroutine("SeekPlayer");
			Scared();
		}
		yield;	
	}	
}

function MoveLoop ()
{
	while(true)
	{
		var deltaTime = Time.deltaTime;
		var targetSpeed = 0.00;
		if(status == HeronStatus.Walking && offsetMoveDirection.magnitude > 0.01)
		{
			if(!fishing)
			{
				targetSpeed = walkAnimSpeed * walkSpeed;
				anim.CrossFade("Walk", 0.4);
			}
			else
			{
				targetSpeed = fishWalkAnimSpeed * fishWalkSpeed;
				anim.CrossFade("FishingWalk", 0.4);
			}
		}
		else if(status == HeronStatus.Running)
		{
			targetSpeed = runAnimSpeed * runSpeed;
			anim.CrossFade("Run", 0.4);
		}
		else
		{
			if(!fishing)
			{
				targetSpeed = 0;
				if(!strechNeck) anim.CrossFade("IdleHold", 0.4);
				else anim.CrossFade("IdleStrechNeck", 0.4);
			}
			else
			{
				targetSpeed = 0;
				anim.CrossFade("IdleFishing", 0.4);
			}
		}
		
		usedMoveDirection = Vector3.Lerp(usedMoveDirection, offsetMoveDirection, deltaTime * 0.7);
		velocity = Vector3.RotateTowards(velocity, offsetMoveDirection * targetSpeed, turning * deltaTime, acceleration * deltaTime);
		velocity.y = 0;
		
		if(velocity.magnitude > 0.01)
		{
			if(lastSpeed < 0.01)
			{
				velocity = forward * 0.1;
			}
			else
			{
				forward = velocity.normalized;
			}
		}
		transform.position += velocity * deltaTime;
		transform.rotation = Quaternion.LookRotation(forward);
		lastSpeed = velocity.magnitude;
		yield;	
	}	
}

function GetPathDirection (curPos : Vector3, wantedDirection : Vector3) : Vector3
{
	var awayFromCollision : Vector3 = TestPosition(curPos);
	if(awayFromCollision != Vector3.zero)
	{
		//Debug.DrawRay(myT.position, awayFromCollision.normalized * 20, Color.yellow);
		return awayFromCollision.normalized;
	}
	else
	{
		///Debug.DrawRay(myT.position, Vector3.up * 5, Color.yellow);
	}
	
	var right = Vector3.Cross(wantedDirection, Vector3.up);
	var currentLength : float = TestDirection(myT.position, wantedDirection);
	if(currentLength > hitTestDistanceMax)
	{
		return wantedDirection;
	}
	else
	{
		var sideAmount = 1 - Mathf.Clamp01(currentLength / 50);
		var rightDirection = Vector3.Lerp(wantedDirection, right, sideAmount * sideAmount);
		var rightLength : float = TestDirection(myT.position, rightDirection);
		var leftDirection = Vector3.Lerp(wantedDirection, -right, sideAmount * sideAmount);
		var leftLength : float = TestDirection(myT.position, leftDirection);
		
		if(rightLength > leftLength && rightLength > currentLength && rightLength > hitTestDistanceIncrement)
		{
			return rightDirection.normalized;
		}
		
		if(leftLength > rightLength && leftLength > currentLength && leftLength > hitTestDistanceIncrement)
		{
			return leftDirection.normalized;
		}
	}
	
	if(currentLength > hitTestDistanceIncrement)
	{
		return wantedDirection;
	}
	
	return Vector3.zero;
}

function TestDirection (position : Vector3, direction : Vector3) : float
{
	var length = 0.00;
	while(true)
	{
		length += hitTestDistanceIncrement;
		if(length > hitTestDistanceMax) return length;
		var testPos : Vector3 = position + (direction * length);
		var height : float = terrain.GetInterpolatedHeight(testPos.x / terrain.size.x, testPos.z / terrain.size.z);
		if(height > maxHeight || height < minHeight)
		{
			break;
		}
		else
		{
			var hit = false;
			var i = 0;
			while(i < colliders.length)
			{
				var collider : HeronCollider = colliders[i];
				x = collider.position.x - testPos.x;
				z = collider.position.z - testPos.z;
				if(x < 0) x = -x;
				if(z < 0) z = -z;
				if(z + x < collider.radius)
				{
					hit = true;
					break;
				}
				i++;	
			}
			
			if(hit) break;
		}
	}
	return length;
}

function TestPosition (testPos : Vector3) : Vector3
{
	var moveDir : Vector3;
	var hieghtPos : Vector3 = testPos;
	var height : float = terrain.GetInterpolatedHeight(testPos.x / terrain.size.x, testPos.z / terrain.size.z);
	if(height > maxHeight || height < minHeight)
	{
		var heightDiff = 100.00;
		var optimalHeight = (maxHeight * 0.5) + (minHeight * 0.5);
		
		var found = false;
		var mult = 1.00;
		while(!found && mult < 5)
		{
			var rotation = 0.00;
			while(rotation < 360)
			{
				forwardDir = Quaternion.Euler(0, rotation, 0) * Vector3.forward;
				forwardPos = testPos + (forwardDir * hitTestDistanceIncrement * mult * 3);
				
				//Debug.DrawRay(forwardPos, Vector3.up, Color(0.9, 0.1, 0.1, 0.7));
				
				forwardHeight = terrain.GetInterpolatedHeight(forwardPos.x / terrain.size.x, forwardPos.z / terrain.size.z);
				diff = Mathf.Abs(forwardHeight - optimalHeight);
				if(forwardHeight < maxHeight && forwardHeight > minHeight && heightDiff > diff)
				{
					//Debug.DrawRay(forwardPos, Vector3.up, Color.green);
					found = true;
					heightDiff = diff;
					hieghtPos = forwardPos;
				}
				rotation += 45;
			}
			mult += 0.5;
		}
	}
	
	move = hieghtPos - testPos;
	if(move.magnitude > 0.01)
	{
		//print("height");
		moveDir = move.normalized;
	}
	else
	{
		//print("noheight");
		moveDir = Vector3.zero;
	}

	var i = 0;
	while(i < colliders.length)
	{
		var collider : HeronCollider = colliders[i];
		x = collider.position.x - testPos.x;
		z = collider.position.z - testPos.z;
		if(x < 0) x = -x;
		if(z < 0) z = -z;
		if(z + x < collider.radius)
		{
			moveDir += (testPos - collider.position).normalized;
			break;
		}
		i++;	
	}
	
	return moveDir;
}

function LateUpdate () // leg IK
{
	rightHeight = terrain.GetInterpolatedHeight(rightFoot.position.x / terrain.size.x, rightFoot.position.z / terrain.size.z);
	rightNormal = terrain.GetInterpolatedNormal(rightFoot.position.x / terrain.size.x, rightFoot.position.z / terrain.size.z);
	leftHeight = terrain.GetInterpolatedHeight(leftFoot.position.x / terrain.size.x, leftFoot.position.z / terrain.size.z);
	leftNormal = terrain.GetInterpolatedNormal(leftFoot.position.x / terrain.size.x, leftFoot.position.z / terrain.size.z);
	
	if(leftHeight < rightHeight)
	{
		transform.position.y = leftHeight;
		leftFoot.rotation = Quaternion.LookRotation(leftFoot.forward, leftNormal);
		leftFoot.Rotate(Vector3.right * 15);
		
		raise = (rightHeight - leftHeight) * 0.5;
		
		rightKnee.position.y += raise;
		rightAnkle.position.y += raise;
		rightFoot.rotation = Quaternion.LookRotation(rightNormal, rightFoot.up);
		rightFoot.Rotate(-Vector3.right * 15);
	}
	else
	{
		transform.position.y = rightHeight;
		rightFoot.rotation = Quaternion.LookRotation(rightNormal, rightFoot.up);
		rightFoot.Rotate(-Vector3.right * 15);
		
		raise = (leftHeight - rightHeight) * 0.5;
		
		leftKnee.position.y += raise;
		leftAnkle.position.y += raise;
		leftFoot.rotation = Quaternion.LookRotation(leftFoot.forward, leftNormal);
		leftFoot.Rotate(Vector3.right * 15);
	}
	
	transform.position.y += 0.1;
}
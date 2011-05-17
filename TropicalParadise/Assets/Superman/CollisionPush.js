public var blowSound : AudioClip;
private var blowSource : AudioSource;

private var blowEmit : ParticleEmitter;



function Awake() {
	blowEmit = GetComponentInChildren(ParticleEmitter);
	if (blowSource == null) {
		blowSource = gameObject.AddComponent("AudioSource") as AudioSource;
		blowSource.playOnAwake = false;
		blowSource.loop = true;
		blowSource.clip = blowSound;
		blowSource.pitch = 0.7;
	}
}

function Update() {
	if (blowEmit.emit == true) {
		if (!blowSource.isPlaying) {
			blowSource.Play();	
		}
	} else {
		blowSource.Stop();
	}
}

function OnParticleCollision (other : GameObject) {
	Debug.Log("Particle Collision Detected");
	var body : Rigidbody = other.rigidbody;
	if (body) {
		var direction : Vector3 = other.transform.position - transform.position;
		direction = direction.normalized;
		body.AddForce (direction * 100);
	}	
}
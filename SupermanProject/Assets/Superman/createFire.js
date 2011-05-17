private var laserEmitter : ParticleEmitter;
private var emitter : ParticleEmitter;

public var fireSource : AudioClip;
//private var fAud : AudioSource;
public var laserSource : AudioClip;
private var lAud : AudioSource;

public var laserSoundRate = 0.5;
private var nextSound = 0.0;

function Awake() {
	laserEmitter = GetComponentInChildren(ParticleEmitter);
	
	if(lAud == null) {
		lAud = gameObject.AddComponent("AudioSource") as AudioSource;
		lAud.playOnAwake = false;
		lAud.loop = false;
		lAud.clip = laserSource;
	}
}

function Update() {
	if (laserEmitter.emit == true && Time.time > nextSound) {
		PlayLaserSound();
		nextSound = Time.time + laserSoundRate;
	}
}

function OnParticleCollision (other : GameObject) {
	Debug.Log("Fire Particle Collision Detected");
	Debug.Log("contact with:" + other.transform.name);
	emitter = other.GetComponentInChildren(ParticleEmitter);

	
	if (emitter != null && emitter.transform.name == "laserfire") {
		FireSound(other);
		Debug.Log("Fire Emitter found:"+ emitter.transform.name);
		// Emit particles for 3 seconds
		emitter.emit = true;
		yield WaitForSeconds(2);
		// Then stop
		emitter.emit = false;
	}
	else
		Debug.Log("No Fire Emitter Found"); 
}

function FireSound(other : GameObject) {
	otherSource = other.GetComponentInChildren(AudioSource);
	if(otherSource == null) {
		otherSource = other.AddComponent("AudioSource") as AudioSource;
		otherSource.playOnAwake = false;
		otherSource.loop = false;
		otherSource.clip = fireSource;
	}
	if (!otherSource.isPlaying) {
		otherSource.Play();
	}
}

function PlayLaserSound() {
	//Debug.Log("Shoot Sounds");
	lAud.Play();	
}
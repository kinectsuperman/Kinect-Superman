var waterLevel : float;
var uAudio : AudioClip;
var aAudio : AudioClip;

var uColor = Color(1,1,1,1);
var uDensity = .05;

var aColor = Color(1,1,1,1);
var aDensity = .008;

var waterSurface : Renderer;
var underwaterSurface : Renderer;

private var below = false;
private var glow : GlowEffect;
private var blur : BlurEffect;


function Awake() {
	if(!waterLevel)
	{
		water = FindObjectOfType(Water);
		if(water) waterLevel = water.gameObject;
	}
	aColor = RenderSettings.fogColor;
	aDensity = RenderSettings.fogDensity;
	
	glow = GetComponent(GlowEffect);
	blur = GetComponent(BlurEffect);
	if( !glow || !blur )
	{
		Debug.LogError("no right Glow/Blur assigned to camera!");
		enabled = false;
	}
	if( !waterSurface || !underwaterSurface )
	{
		Debug.LogError("assign water & underwater surfaces");
		enabled = false;
	}
	if( underwaterSurface != null )
		underwaterSurface.enabled = false; // initially underwater is disabled
}

function Update ()
{
	if (waterLevel < transform.position.y && below)
	{
		audio.clip = aAudio;
		audio.Play();
		RenderSettings.fogDensity = aDensity;
		RenderSettings.fogColor = aColor;
		
		below = false;
		
		glow.enabled = !below; 
		blur.enabled = below; 
		waterSurface.enabled = true;
		underwaterSurface.enabled = false;
	}
	
	if (waterLevel > transform.position.y && !below)
	{
		audio.clip = uAudio;
		audio.Play();
		RenderSettings.fogDensity = uDensity;
		RenderSettings.fogColor = uColor;
		
		below = true;
		
		glow.enabled = !below; 
		blur.enabled = below;
		waterSurface.enabled = false;
		underwaterSurface.enabled = false;
	}
}

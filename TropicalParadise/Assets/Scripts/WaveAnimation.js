var siblings : GameObject[] = new GameObject[0];
var index = 0;
var offset = 0.00;
var slideMin = -0.1;
var slideMax = 0.4;
var slideSpeed = 0.5;
var slideSharpness = 1.00;
var scaleMin = 1.00;
var scaleMax = 0.40;
var scaleSpeed = 0.50;
var scaleSharpness = 0.50;

var fadeSpeed = 0.00;

var baseScroll : Vector3 = Vector3(0.1, 0, 0.3547);
var baseRotation = 0.00;
var baseScale = Vector3 (10.0, 10, 10.0);

private var theMaterial : Material;
private var slide = 0.00;
private var slideInertia = 0.00;
private var scale = 0.00;
private var scaleInertia = 0.00;
private var basePos : Vector3;
private var texScale : Vector3;
private var lastSlide = 0.00;
private var fade = 1.00;
private var color : Color;
private var fadeColor : Color;
var original : WaveAnimation;

function Start ()
{
	CheckHWSupport();
	
	var waves = Array();
	waves = GetComponents(WaveAnimation);
	if(waves.length == 1 && original == null)
	{
		original = this;	
	}
	
	for(var s : GameObject in siblings)
	{
		AddCopy(s, original, false);	
	}
	if(waves.length < renderer.materials.length)
	{
		AddCopy(gameObject, original, true);
	}
	theMaterial = renderer.materials[index];
	color = theMaterial.GetColor("_Color");
	fadeColor = color;
	fadeColor.a = 0;
	texScale = theMaterial.GetTextureScale("_MainTex");	
}

private function CheckHWSupport()
{
	var supported = renderer.sharedMaterial.shader.isSupported;
	for(var s : GameObject in siblings)
		s.renderer.enabled = supported;
	renderer.enabled = supported;
}


function Update ()
{
	CheckHWSupport();
	
	slideInertia = Mathf.Lerp(slideInertia, Mathf.PingPong((Time.time * scaleSpeed) + offset, 1), slideSharpness * Time.deltaTime);
	slide = Mathf.Lerp(slide, slideInertia, slideSharpness * Time.deltaTime);
	theMaterial.SetTextureOffset("_MainTex", Vector3(index * 0.35, Mathf.Lerp(slideMin, slideMax, slide) * 2, 0));
	theMaterial.SetTextureOffset("_Cutout", Vector3(index * 0.79, Mathf.Lerp(slideMin, slideMax, slide) / 2, 0));
	
	fade = Mathf.Lerp(fade, slide - lastSlide > 0 ? 1 : 0, Time.deltaTime * fadeSpeed);
	lastSlide = slide;
	theMaterial.SetColor("_Color", Color.Lerp(fadeColor, color, fade));
	
	scaleInertia = Mathf.Lerp(scaleInertia, Mathf.PingPong((Time.time * scaleSpeed) + offset, 1), scaleSharpness * Time.deltaTime);
	scale = Mathf.Lerp(scale, scaleInertia, scaleSharpness * Time.deltaTime);
	theMaterial.SetTextureScale("_MainTex", Vector3(texScale.x, Mathf.Lerp(scaleMin,scaleMax, scale), texScale.z));
	
	basePos += baseScroll * Time.deltaTime;
	var inverseScale = Vector3 (1 / baseScale.x, 1 / baseScale.y, 1 / baseScale.z);
	var uvMat = Matrix4x4.TRS (basePos, Quaternion.Euler (baseRotation,90,90), inverseScale);
	theMaterial.SetMatrix ("_WavesBaseMatrix", uvMat);
}


function AddCopy (ob : GameObject, original : WaveAnimation, copy : boolean)
{
	newWave = ob.AddComponent(WaveAnimation);
	newWave.original = original;
	if(copy) newWave.index = index + 1;
	else newWave.index = index;
	newWave.offset = original.offset + (2.00 / parseFloat(renderer.materials.length));
	newWave.slideMin = original.slideMin;
	newWave.slideMax = original.slideMax;
	newWave.slideSpeed = original.slideSpeed + Random.Range(-original.slideSpeed / 5, original.slideSpeed / 5);
	newWave.slideSharpness = original.slideSharpness + Random.Range(-original.slideSharpness / 5, original.slideSharpness / 5);
	newWave.scaleMin = original.scaleMin;
	newWave.scaleMax = original.scaleMax;
	newWave.scaleSpeed = original.scaleSpeed + Random.Range(-original.scaleSpeed / 5, original.scaleSpeed / 5);
	newWave.scaleSharpness = original.scaleSharpness + Random.Range(-original.scaleSharpness / 5, original.scaleSharpness / 5);
	
	newWave.fadeSpeed = original.fadeSpeed;
		
	randy = Random.onUnitSphere; randy.y = 0;
	newWave.baseScroll = randy.normalized * original.baseScroll.magnitude;
	newWave.baseRotation = Random.Range(0,360);
	newWave.baseScale = original.baseScale * Random.Range(0.8, 1.2);	
}
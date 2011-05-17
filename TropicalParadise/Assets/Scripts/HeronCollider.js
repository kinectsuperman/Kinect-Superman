var radius = 0.00;
var position : Vector3;

function Awake ()
{
	position = transform.position;	
}

function OnDrawGizmosSelected ()
{
	Gizmos.color = Color(0.32, 0.55, 0.76, 0.7);
	Gizmos.DrawWireSphere(transform.position, radius);
}
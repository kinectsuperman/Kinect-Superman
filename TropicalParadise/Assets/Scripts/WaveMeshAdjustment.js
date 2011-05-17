var col : Collider;

function Start ()
{
	var filter : MeshFilter = GetComponent(MeshFilter);
	var mesh = filter.mesh;
	var mTransform = transform;
	var vertices : Vector3[] = mesh.vertices;
	var i = 1;
	var hit : RaycastHit;
	while(i < vertices.length - 1) // i - 1 == terrain side        // i == water side
	{
		dir = vertices[i-1] - vertices[i];
		if(mTransform.TransformDirection(dir) != Vector3.zero && col.Raycast(Ray(mTransform.TransformPoint(vertices[i]), mTransform.TransformDirection(dir)), hit, 30.00))
		{
			hitPoint = mTransform.InverseTransformPoint(hit.point);
			shorePos = hitPoint + (dir / 3); shorePos.y += 15;
			if(col.Raycast(Ray(mTransform.TransformPoint(shorePos), -Vector3.up), hit, 30.00))
			hitPoint = mTransform.InverseTransformPoint(hit.point);
			if(hitPoint.y > 1.5) hitPoint.y = 0;
			vertices[i-1] = hitPoint;
		}
		i+=2;
	}
	
	mesh.vertices = vertices;
	filter.mesh = mesh;
}
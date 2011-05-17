Shader "FX/Seafloor with fog" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
}

SubShader {
	Cull Off
	Lighting Off
	ColorMask RGB
	Pass {
		SetTexture [_MainTex] { constantColor [_TintColor] combine constant }
	}
}

}

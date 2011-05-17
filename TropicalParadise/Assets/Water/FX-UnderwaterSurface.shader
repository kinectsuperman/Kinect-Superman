Shader "FX/Underwater surface" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
}

SubShader {
	Tags {Queue=Transparent}
	ZWrite Off
	ColorMask RGB
	Fog { Color [_AddFog] }
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		Color [_Color]
		SetTexture [_MainTex] { combine primary }
	}
}

}

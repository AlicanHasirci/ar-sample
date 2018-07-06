Shader "Peak/Card Shader" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}
SubShader {
	Tags { "Queue" = "Geometry" }
	Lighting Off
	Cull Back
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert (appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.texcoord);
				return col * i.color;
			}
		ENDCG
	}
}
}
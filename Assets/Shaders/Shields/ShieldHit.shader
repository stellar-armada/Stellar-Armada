// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SpaceCommander/Shields/ShieldHit" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_PatternTex("Albedo (RGB)", 2D) = "black" {}
		_PatternScale("PatternScale", Range(0.01,100)) = 1.0

		_HitAttenuation("HitAttenuation", Range(0.01,100)) = 1.0
		_HitPower("HitPower", Range(0.01,20)) = 1.0
		_HitRadius("HitRadius", Range(0.01,20)) = 0.25

		_HitPos("HitPos", Vector) = (0,0,-0.5)
		_WorldScale("WorldScale", Vector) = (1,1,1)

		_BlendSrcMode("BlendSrcMode", Int) = 0
		_BlendDstMode("BlendDstMode", Int) = 0
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300

		BlendOp Add
		Blend[_BlendSrcMode][_BlendDstMode]
		ZWrite Off

		Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                half4 vertex : POSITION; // vertex position
                half3 normal : NORMAL;
                half2 uv : TEXCOORD0; // texture coordinate
                 UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                half2 uv : TEXCOORD0; // texture coordinate
                half4 pos : SV_POSITION; // clip space position
 
				half depth : TEXCOORD3;
				half4 objectSpacePos : TEXCOORD4;
				half4 screenPos : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.


				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.objectSpacePos = v.vertex;

				o.screenPos = ComputeScreenPos(o.pos);

                float3 p = UnityObjectToViewPos(v.vertex);
				o.depth = -p.z;

                return o;
            }
            
			sampler2D _PatternTex;
            fixed4 _Color;
            half _HitAttenuation;
			half _HitPower;
			half _PatternScale;
			half3 _HitPos;
			half3 _WorldScale;
			half _HitRadius;
        
			struct fragOutput 
			{
				fixed4 color0 : SV_Target;
			};

            fragOutput frag (v2f i)
			{
			
			UNITY_SETUP_INSTANCE_ID(i);
			
				half3 diff = UNITY_ACCESS_INSTANCED_PROP(Props, _HitPos) - i.objectSpacePos;

                half3 ws = UNITY_ACCESS_INSTANCED_PROP(Props, _WorldScale);
				diff.x *=  ws.x;
				diff.y *=  ws.y;
				diff.z *=  ws.z;

				half dist = length(diff);

				fixed4 pattern = tex2D(_PatternTex, i.uv*_PatternScale);
				
				half att = (1.0 - min(dist / UNITY_ACCESS_INSTANCED_PROP(Props, _HitRadius), 1.0));

				fragOutput o;

				o.color0 = pattern * _Color * _HitPower * att;

                return o;
            }
            ENDCG
        }
	}
	CustomEditor "ShieldHitMaterialEditor"
	
	FallBack "Diffuse"
}

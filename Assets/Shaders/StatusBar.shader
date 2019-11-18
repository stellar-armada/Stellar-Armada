// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpaceCommander/StatusBar"
{
	Properties
	{
		[PerRendererData]_Insignia("Insignia", 2D) = "black" {}
		_Health("Health", Range( 0 , 1)) = 0
		_Shield("Shield", Range( 0 , 1)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[PerRendererData]_Visibility("Visibility", Range( 0 , 1)) = 1
		_ToonLines("Toon Lines", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _ToonLines;
		uniform float4 _ToonLines_ST;
		uniform sampler2D _Insignia;
		uniform half _Health;
		uniform float _Shield;
		uniform half _Visibility;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ToonLines = i.uv_texcoord * _ToonLines_ST.xy + _ToonLines_ST.zw;
			float4 tex2DNode4 = tex2D( _Insignia, i.uv_texcoord );
			float4 temp_output_21_0 = (( tex2DNode4.b > 0.99 ) ? (( tex2DNode4.g < 0.01 ) ? (( tex2DNode4.r < 0.01 ) ? tex2DNode4 :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) );
			float4 temp_output_5_0 = (( tex2DNode4.r > 0.99 ) ? (( tex2DNode4.b < 0.01 ) ? (( tex2DNode4.g < 0.01 ) ? tex2DNode4 :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) );
			float4 temp_cast_0 = 0;
			float4 temp_output_30_0 = (( ( temp_output_21_0 + temp_output_5_0 ) == temp_cast_0 ) ? tex2DNode4 :  ( (( i.uv_texcoord.x < _Health ) ? temp_output_5_0 :  float4( 0,0,0,0 ) ) + (( i.uv_texcoord.x < _Shield ) ? temp_output_21_0 :  float4( 0,0,0,0 ) ) ) );
			o.Emission = ( tex2D( _ToonLines, uv_ToonLines ) * temp_output_30_0 ).rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 temp_cast_2 = (-0.5).xxxx;
			float4 temp_cast_3 = 0;
			float4 break67 = ( ( 1.0 - ( ( 1.0 - _Visibility ) * ( tex2D( _TextureSample0, uv_TextureSample0 ) - temp_cast_2 ) ) ) * temp_output_30_0 );
			o.Alpha = ( ( break67.r + break67.g + break67.b + break67.a ) + 0.0 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17200
91;314;1697;596;740.6022;555.7467;1.864007;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1090.75,-47.38965;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;13;-421.5623,-89.43976;Inherit;False;698.6;207;Is Red?;3;5;17;14;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-406.5013,199.991;Inherit;False;703.4;210.2;IsBlue;3;19;20;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;4;-842.2064,206.2857;Inherit;True;Property;_Insignia;Insignia;1;1;[PerRendererData];Create;True;0;0;False;0;-1;None;217b5f3648a4834448b4d08015fe60c4;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-819.5015,89.09106;Half;False;Constant;_MinThreshold;MinThreshold;2;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;19;-375.2036,247.852;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;14;-376.5019,-44.50895;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;22;419.3759,522.4061;Inherit;False;293.6;290.4;Shield SLider;2;23;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-638.1622,9.660296;Half;False;Constant;_MaxThreshold;MaxThreshold;1;0;Create;True;0;0;False;0;0.99;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;17;-153.2597,-46.5525;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;20;-159.9615,244.2086;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;35;752.3634,-261.1178;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;-1;None;30f384bc1d4410c4c9f953a6c08a3b28;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;180.0483,-371.5872;Half;False;Property;_Health;Health;2;0;Create;True;0;0;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;1006.759,83.01083;Half;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;437.1764,570.8059;Float;False;Property;_Shield;Shield;3;0;Create;True;0;0;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;21;73.93495,246.8147;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareGreater;5;56.63674,-42.3462;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;18;313.4566,-203.3081;Inherit;False;293.6;290.4;Health Slider;1;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;34;774.4674,-369.9956;Half;False;Property;_Visibility;Visibility;5;1;[PerRendererData];Create;True;0;0;True;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;12;379.7512,-66.87463;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;24;475.1762,661.2057;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;54;1158.695,-295.7313;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;1283.367,17.82306;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;952.751,-34.36201;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;652.4319,154.0035;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;29;1035.866,210.8021;Inherit;False;403.2502;244.8244;If Blue is 0;1;30;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;1366.173,-138.9799;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;31;1035.655,523.6927;Float;False;Constant;_Int1;Int 1;3;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.TFHCCompareEqual;30;1202.753,272.0941;Inherit;False;4;0;COLOR;0,0,0,0;False;1;INT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;37;1594.185,-174.3345;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;2126.734,427.0975;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;67;1719.11,585.691;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;44;1334.982,566.8898;Inherit;True;Property;_ToonLines;Toon Lines;6;0;Create;True;0;0;False;0;-1;None;e5d7e169bc52d1f4497b3d2a65240ce7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;2014.422,660.4303;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1632.209,213.3864;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;2173.015,587.5138;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;65;2339.209,229.6215;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;SpaceCommander/StatusBar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.1;True;True;0;True;TransparentCutout;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;1;10;0
WireConnection;19;0;4;1
WireConnection;19;1;15;0
WireConnection;19;2;4;0
WireConnection;14;0;4;2
WireConnection;14;1;15;0
WireConnection;14;2;4;0
WireConnection;17;0;4;3
WireConnection;17;1;15;0
WireConnection;17;2;14;0
WireConnection;20;0;4;2
WireConnection;20;1;15;0
WireConnection;20;2;19;0
WireConnection;21;0;4;3
WireConnection;21;1;7;0
WireConnection;21;2;20;0
WireConnection;5;0;4;1
WireConnection;5;1;7;0
WireConnection;5;2;17;0
WireConnection;12;0;10;1
WireConnection;12;1;9;0
WireConnection;12;2;5;0
WireConnection;24;0;10;1
WireConnection;24;1;23;0
WireConnection;24;2;21;0
WireConnection;54;0;34;0
WireConnection;47;0;35;0
WireConnection;47;1;48;0
WireConnection;32;0;12;0
WireConnection;32;1;24;0
WireConnection;33;0;21;0
WireConnection;33;1;5;0
WireConnection;49;0;54;0
WireConnection;49;1;47;0
WireConnection;30;0;33;0
WireConnection;30;1;31;0
WireConnection;30;2;4;0
WireConnection;30;3;32;0
WireConnection;37;0;49;0
WireConnection;66;0;37;0
WireConnection;66;1;30;0
WireConnection;67;0;66;0
WireConnection;68;0;67;0
WireConnection;68;1;67;1
WireConnection;68;2;67;2
WireConnection;68;3;67;3
WireConnection;45;0;44;0
WireConnection;45;1;30;0
WireConnection;69;0;68;0
WireConnection;65;2;45;0
WireConnection;65;9;69;0
ASEEND*/
//CHKSM=6E7B5090256A1D9C938BA2583A7C59B3024DA1D9
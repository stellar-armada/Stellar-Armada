// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpaceCommander/Toon"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_Color("Color", Color) = (1,1,1,0)
		[NoScaleOffset]_BaseColorRGBOutlineWidthA("Base Color (RGB) Outline Width (A)", 2D) = "gray" {}
		_BaseCellSharpness("Base Cell Sharpness", Range( 0.01 , 1)) = 0.01
		_BaseCellOffset("Base Cell Offset", Range( -1 , 1)) = 0
		_ShadowContribution("Shadow Contribution", Range( 0 , 1)) = 0.5
		_HighlightTint("Highlight Tint", Color) = (1,1,1,1)
		_HighlightCellOffset("Highlight Cell Offset", Range( -1 , -0.5)) = -0.95
		_HighlightCellSharpness("Highlight Cell Sharpness", Range( 0.001 , 1)) = 0.01
		[Toggle(_STATICHIGHLIGHTS_ON)] _StaticHighLights("Static HighLights", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _STATICHIGHLIGHTS_ON
		struct Input
		{
			float3 viewDir;
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform half4 _HighlightTint;
		uniform half _HighlightCellOffset;
		uniform half _HighlightCellSharpness;
		uniform half _BaseCellOffset;
		uniform half _BaseCellSharpness;
		uniform half _ShadowContribution;
		uniform sampler2D _BaseColorRGBOutlineWidthA;
		uniform half4 _Color;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 HighlightColor249 = (_HighlightTint).rgb;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColorFalloff227 = ( ase_lightColor.rgb * ase_lightAtten );
			float temp_output_189_0 = (_HighlightTint).a;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult4_g3 = normalize( ( i.viewDir + ase_worldlightDir ) );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult62 = dot( normalizeResult4_g3 , ase_worldNormal );
			float dotResult54 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL236 = dotResult54;
			#ifdef _STATICHIGHLIGHTS_ON
				float staticSwitch195 = NdotL236;
			#else
				float staticSwitch195 = dotResult62;
			#endif
			float lerpResult159 = lerp( ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) ) , ( saturate( ( ( NdotL236 + _BaseCellOffset ) / _BaseCellSharpness ) ) * ase_lightAtten ) , _ShadowContribution);
			float2 uv_BaseColorRGBOutlineWidthA76 = i.uv_texcoord;
			float3 BaseColor253 = ( lerpResult159 * (( tex2D( _BaseColorRGBOutlineWidthA, uv_BaseColorRGBOutlineWidthA76 ) * _Color )).rgb );
			c.rgb = ( ( ( 1.0 * HighlightColor249 * LightColorFalloff227 * pow( temp_output_189_0 , 1.5 ) * saturate( ( ( staticSwitch195 + _HighlightCellOffset ) / ( ( 1.0 - temp_output_189_0 ) * _HighlightCellSharpness ) ) ) ) + BaseColor253 ) * BaseColor253 );
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				o.worldNormal = worldNormal;
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
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
200;152;1697;867;-3591.72;387.6604;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;241;346.1087,-206.8004;Inherit;False;835.6508;341.2334;Comment;4;53;54;236;269;N dot L;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;396.1086,-44.56697;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;269;436.9136,-163.4066;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;54;720.4,-120.0015;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;242;1324.039,459.9366;Inherit;False;2744.931;803.0454;Comment;20;253;158;235;73;76;159;162;160;214;74;213;215;207;57;60;58;127;59;237;256;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;236;938.759,-116.6859;Float;False;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1374.039,641.3355;Half;False;Property;_BaseCellOffset;Base Cell Offset;3;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;237;1374.247,533.2394;Inherit;False;236;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;251;648.5698,-1229.416;Inherit;False;2234.221;738.9581;Comment;19;180;249;246;181;177;172;175;61;239;62;195;174;173;171;261;260;270;263;186;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1676.114,631.0218;Half;False;Property;_BaseCellSharpness;Base Cell Sharpness;2;0;Create;True;0;0;False;0;0.01;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;177;1329.485,-1179.416;Half;False;Property;_HighlightTint;Highlight Tint;5;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;127;1631.055,790.9418;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;1657.487,534.5102;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;61;698.5698,-733.562;Inherit;False;Blinn-Phong Half Vector;-1;;3;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;270;747.2693,-649.59;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;248;1613.991,-1419.486;Inherit;False;287;165;Comment;1;189;Spec/Smooth;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;240;415.9745,466.0967;Inherit;False;717.6841;295.7439;Comment;4;229;228;230;227;Light Falloff;0.9947262,1,0.6176471,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;62;1043.79,-679.1187;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;189;1665.591,-1364.685;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;207;1626.013,899.7625;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;1956.552,537.3538;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;215;1944.082,816.0775;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;1003.499,-845.3307;Inherit;False;236;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;195;1271.122,-784.4333;Float;False;Property;_StaticHighLights;Static HighLights;8;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;1799.042,-648.0834;Half;False;Property;_HighlightCellSharpness;Highlight Cell Sharpness;7;0;Create;True;0;0;False;0;0.01;0.01;0.001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;229;465.9744,651.8407;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;76;2937.31,875.5428;Inherit;True;Property;_BaseColorRGBOutlineWidthA;Base Color (RGB) Outline Width (A);1;1;[NoScaleOffset];Create;True;0;0;False;0;-1;None;1a115e1ad7af93e469647df10be3b3de;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;173;1405.781,-660.3005;Half;False;Property;_HighlightCellOffset;Highlight Cell Offset;6;0;Create;True;0;0;False;0;-0.95;-0.95;-1;-0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;73;3042.337,1074.833;Half;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,0;0,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;261;1955.959,-879.307;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;228;511.6781,516.0967;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;74;2114.005,542.3831;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;2134.626,851.7131;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;214;2324.502,854.6982;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;2351.156,541.2684;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;2155.415,983.4974;Half;False;Property;_ShadowContribution;Shadow Contribution;4;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;171;1760.558,-788.2134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;699.9734,582.2577;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;2116.472,-654.7482;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;256;3281.878,886.9672;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;235;3441.811,883.3879;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;227;874.6581,578.7247;Float;False;LightColorFalloff;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;180;2038.106,-986.2567;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;172;2179.195,-792.7061;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;159;2709.43,792.4121;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;175;2324.68,-792.1935;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;186;2546.682,-1169.142;Half;False;Constant;_Float5;Float 5;20;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3655.448,806.9639;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;246;2278.439,-885.2326;Inherit;False;227;LightColorFalloff;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;249;2281.878,-985.5474;Float;False;HighlightColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;263;2358.978,-1168.568;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;253;3801.573,744.8395;Float;False;BaseColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;2713.791,-1059.016;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;252;4114.906,77.34266;Inherit;False;253;BaseColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;3976.888,-59.5854;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;268;4404.8,42.5235;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;271;4542.423,-223.8251;Float;False;True;2;ASEMaterialInspector;0;0;CustomLighting;SpaceCommander/Toon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;269;0
WireConnection;54;1;53;0
WireConnection;236;0;54;0
WireConnection;58;0;237;0
WireConnection;58;1;59;0
WireConnection;62;0;61;0
WireConnection;62;1;270;0
WireConnection;189;0;177;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;215;0;127;0
WireConnection;195;1;62;0
WireConnection;195;0;239;0
WireConnection;261;0;189;0
WireConnection;74;0;57;0
WireConnection;213;0;215;0
WireConnection;213;1;207;2
WireConnection;214;0;213;0
WireConnection;160;0;74;0
WireConnection;160;1;127;0
WireConnection;171;0;195;0
WireConnection;171;1;173;0
WireConnection;230;0;228;1
WireConnection;230;1;229;0
WireConnection;260;0;261;0
WireConnection;260;1;174;0
WireConnection;256;0;76;0
WireConnection;256;1;73;0
WireConnection;235;0;256;0
WireConnection;227;0;230;0
WireConnection;180;0;177;0
WireConnection;62;0;61;0
WireConnection;62;1;270;0
WireConnection;214;0;213;0
WireConnection;215;0;127;0
WireConnection;268;0;163;0
WireConnection;268;1;252;0
WireConnection;163;0;181;0
WireConnection;163;1;253;0
WireConnection;263;0;189;0
WireConnection;261;0;189;0
WireConnection;54;0;269;0
WireConnection;54;1;53;0
WireConnection;195;1;62;0
WireConnection;195;0;239;0
WireConnection;171;0;195;0
WireConnection;171;1;173;0
WireConnection;189;0;177;0
WireConnection;172;0;171;0
WireConnection;172;1;260;0
WireConnection;159;0;214;0
WireConnection;159;1;160;0
WireConnection;159;2;162;0
WireConnection;175;0;172;0
WireConnection;158;0;159;0
WireConnection;158;1;235;0
WireConnection;249;0;180;0
WireConnection;263;0;189;0
WireConnection;253;0;158;0
WireConnection;181;0;186;0
WireConnection;181;1;249;0
WireConnection;181;2;246;0
WireConnection;181;3;263;0
WireConnection;181;4;175;0
WireConnection;163;0;181;0
WireConnection;163;1;253;0
WireConnection;268;0;163;0
WireConnection;268;1;252;0
WireConnection;271;13;268;0
ASEEND*/
//CHKSM=FFE1AB4BE9B9169A363ECA701B487BA2822DD26C
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/CustomOutlineToon"
{
    Properties
    {
		[NoScaleOffset]_BaseColorRGBOutlineWidthA("Base Color (RGB) Outline Width (A)", 2D) = "gray" {}
		_BaseTint("Base Tint", Color) = (1,1,1,0)
		_BaseCellSharpness("Base Cell Sharpness", Range( 0.01 , 1)) = 0.01
		_BaseCellOffset("Base Cell Offset", Range( -1 , 1)) = 0
		_IndirectDiffuseContribution("Indirect Diffuse Contribution", Range( 0 , 1)) = 1
		_ShadowContribution("Shadow Contribution", Range( 0 , 1)) = 0.5
		[NoScaleOffset]_Highlight("Highlight", 2D) = "white" {}
		[HDR]_HighlightTint("Highlight Tint", Color) = (1,1,1,1)
		_HighlightCellOffset("Highlight Cell Offset", Range( -1 , -0.5)) = -0.95
		_HighlightCellSharpness("Highlight Cell Sharpness", Range( 0.001 , 1)) = 0.01
		_IndirectSpecularContribution("Indirect Specular Contribution", Range( 0 , 1)) = 1
		[Toggle(_STATICHIGHLIGHTS_ON)] _StaticHighLights("Static HighLights", Float) = 0
		[Normal][NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 1)) = 1
		[HDR]_RimColor("Rim Color", Color) = (1,1,1,0)
		_RimPower("Rim Power", Range( 0.01 , 1)) = 0.4
		_RimOffset("Rim Offset", Range( 0 , 1)) = 0.6
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
    }

    SubShader
    {
		

        Tags { "RenderPipeline"="LightweightPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Back
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
        Pass
        {
            Tags { "LightMode"="LightweightForward" }
            Name "Base"

            Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            // -------------------------------------
            // Lightweight Pipeline keywords
            #pragma shader_feature _SAMPLE_GI

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag

            #define ASE_SRP_VERSION 51601
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature _STATICHIGHLIGHTS_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON


            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/UnlitInput.hlsl"

			float _NormalScale;
			sampler2D _NormalMap;
			float4 _HighlightTint;
			sampler2D _Highlight;
			float _IndirectSpecularContribution;
			float _HighlightCellOffset;
			float _HighlightCellSharpness;
			float _IndirectDiffuseContribution;
			float _BaseCellOffset;
			float _BaseCellSharpness;
			float _ShadowContribution;
			sampler2D _BaseColorRGBOutlineWidthA;
			float4 _BaseTint;
			float _RimOffset;
			float _RimPower;
			float4 _RimColor;

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float4 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct GraphVertexOutput
            {
                float4 position : POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 lightmapUVOrVertexSH : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			
            GraphVertexOutput vert (GraphVertexInput v)
            {
                GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord2.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal.xyz);
				o.ase_texcoord3.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				VertexPositionInputs ase_vertexInput = GetVertexPositionInputs (v.vertex.xyz);
				#ifdef _MAIN_LIGHT_SHADOWS//ase_lightAtten_vert
				o.ase_texcoord5 = GetShadowCoord( ase_vertexInput );
				#endif//ase_lightAtten_vert
				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( ase_worldNormal, o.lightmapUVOrVertexSH.xyz );
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				float3 vertexValue = float3(0,0,0);
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue; 
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }

            half4 frag (GraphVertexOutput IN ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				float3 temp_cast_0 = (1.0).xxx;
				float3 ase_worldPos = IN.ase_texcoord.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float2 uv_NormalMap82 = IN.ase_texcoord1.xy;
				float3 ase_worldTangent = IN.ase_texcoord2.xyz;
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal52 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap82 ), _NormalScale );
				float3 worldNormal52 = float3(dot(tanToWorld0,tanNormal52), dot(tanToWorld1,tanNormal52), dot(tanToWorld2,tanNormal52));
				float3 normalizeResult170 = normalize( worldNormal52 );
				float3 NewNormals220 = normalizeResult170;
				float2 uv_Highlight81 = IN.ase_texcoord1.xy;
				float4 temp_output_184_0 = ( _HighlightTint * tex2D( _Highlight, uv_Highlight81 ) );
				float temp_output_189_0 = (temp_output_184_0).a;
				half3 reflectVector106 = reflect( -ase_worldViewDir, NewNormals220 );
				float3 indirectSpecular106 = GlossyEnvironmentReflection( reflectVector106, 1.0 - temp_output_189_0, 1.0 );
				float3 lerpResult187 = lerp( temp_cast_0 , indirectSpecular106 , _IndirectSpecularContribution);
				float3 HighlightColor249 = (temp_output_184_0).rgb;
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( IN.ase_texcoord5 );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				#ifdef _ADDITIONAL_LIGHTS//ase_lightAtten_frag
				int ase_lightAtten_pixelLightCount = GetAdditionalLightsCount();
				for (int i = 0; i < ase_lightAtten_pixelLightCount; ++i)
				{//ase_lightAtten_frag
				Light ase_lightAtten_pointLight = GetAdditionalLight( i, ase_worldPos );
				ase_lightAtten += ase_lightAtten_pointLight.distanceAttenuation * ase_lightAtten_pointLight.shadowAttenuation;
				}//ase_lightAtten_frag
				#endif//ase_lightAtten_frag
				ase_lightAtten = saturate( ase_lightAtten );
				float3 LightColorFalloff227 = ( _MainLightColor.rgb * ase_lightAtten );
				float3 normalizeResult4_g3 = normalize( ( ase_worldViewDir + _MainLightPosition.xyz ) );
				float dotResult62 = dot( normalizeResult4_g3 , NewNormals220 );
				float dotResult54 = dot( NewNormals220 , _MainLightPosition.xyz );
				float NdotL236 = dotResult54;
				#ifdef _STATICHIGHLIGHTS_ON
				float staticSwitch195 = NdotL236;
				#else
				float staticSwitch195 = dotResult62;
				#endif
				float3 temp_cast_1 = (1.0).xxx;
				float3 bakedGI115 = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, NewNormals220 );
				float3 lerpResult117 = lerp( temp_cast_1 , bakedGI115 , _IndirectDiffuseContribution);
				float temp_output_214_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _MainLightPosition.w ) );
				float lerpResult159 = lerp( temp_output_214_0 , ( saturate( ( ( NdotL236 + _BaseCellOffset ) / _BaseCellSharpness ) ) * ase_lightAtten ) , _ShadowContribution);
				float2 uv_BaseColorRGBOutlineWidthA76 = IN.ase_texcoord1.xy;
				float4 tex2DNode76 = tex2D( _BaseColorRGBOutlineWidthA, uv_BaseColorRGBOutlineWidthA76 );
				float3 BaseColor253 = ( ( ( lerpResult117 * _MainLightColor.a * temp_output_214_0 ) + ( _MainLightColor.rgb * lerpResult159 ) ) * (( tex2DNode76 * _BaseTint )).rgb );
				float dotResult88 = dot( NewNormals220 , ase_worldViewDir );
				
		        float3 Color = ( ( ( lerpResult187 * HighlightColor249 * LightColorFalloff227 * pow( temp_output_189_0 , 1.5 ) * saturate( ( ( staticSwitch195 + _HighlightCellOffset ) / ( ( 1.0 - temp_output_189_0 ) * _HighlightCellSharpness ) ) ) ) + BaseColor253 + ( ( saturate( NdotL236 ) * pow( ( 1.0 - saturate( ( dotResult88 + _RimOffset ) ) ) , _RimPower ) ) * HighlightColor249 * LightColorFalloff227 * (_RimColor).rgb ) ) * BaseColor253 );
		        float Alpha = 1;
		        float AlphaClipThreshold = 0;
         #if _AlphaClip
                clip(Alpha - AlphaClipThreshold);
        #endif
                return half4(Color, Alpha);
            }
            ENDHLSL
        }

		
        Pass
        {
			
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
			ZWrite On
			ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #define ASE_SRP_VERSION 51601


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			
            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float4 ase_normal : NORMAL;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // x: global clip space bias, y: normal world space bias
            float3 _LightDirection;

			
            VertexOutput ShadowPassVertex(GraphVertexInput v )
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
				
				float3 vertexValue = float3(0,0,0);
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldDir(v.ase_normal.xyz);

                float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                // normal bias is negative since we want to apply an inset normal offset
                positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
				positionWS = normalWS * scale.xxx + positionWS;
                float4 clipPos = TransformWorldToHClip(positionWS);

                // _ShadowBias.x sign depens on if platform has reversed z buffer
                //clipPos.z += _ShadowBias.x; 

            #if UNITY_REVERSED_Z
                clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
            #else
                clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
            #endif
                o.clipPos = clipPos;

                return o;
            }

            half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
        		

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;
         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
                return 0;
            }

            ENDHLSL
        }

		
        Pass
        {
			
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            ZWrite On
			ZTest LEqual
			ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #define ASE_SRP_VERSION 51601


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			
			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			
			VertexOutput vert( GraphVertexInput v  )
			{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					
					float3 vertexValue = float3(0,0,0);	
					#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
					#else
					v.vertex.xyz += vertexValue;
					#endif
					v.ase_normal =  v.ase_normal ;
					o.clipPos = TransformObjectToHClip(v.vertex.xyz);
					return o;
			}

            half4 frag( VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
                return 0;
            }
            ENDHLSL
        }
		
    }
    Fallback "Hidden/InternalErrorShader"
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=16800
2186;190;1484;754;-4053.993;-297.0586;1.410663;True;True
Node;AmplifyShaderEditor.CommentaryNode;226;-803.833,-214.5792;Float;False;1370.182;280;Comment;5;82;52;170;220;109;Normals;0.5220588,0.6044625,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-753.8331,-138.0697;Float;False;Property;_NormalScale;Normal Scale;13;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;-422.226,-164.5792;Float;True;Property;_NormalMap;Normal Map;12;2;[Normal];[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;52;-103.4431,-159.3391;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;170;138.1924,-160.2827;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;241;-676.3679,366.5508;Float;False;835.6508;341.2334;Comment;4;53;223;54;236;N dot L;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;323.3487,-158.8317;Float;False;NewNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;223;-599.3095,416.5508;Float;False;220;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;-626.368,528.7842;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;54;-302.0764,453.3497;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;236;-83.71747,456.6653;Float;False;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;242;1324.039,459.9366;Float;False;2744.931;803.0454;Comment;25;253;158;130;235;73;182;76;133;107;159;162;160;214;74;213;215;207;57;60;58;127;59;237;256;257;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1374.039,641.3355;Float;False;Property;_BaseCellOffset;Base Cell Offset;3;0;Create;True;0;0;False;0;0;-0.24;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;237;1374.247,533.2394;Float;False;236;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1676.114,631.0218;Float;False;Property;_BaseCellSharpness;Base Cell Sharpness;2;0;Create;True;0;0;False;0;0.01;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;127;1631.055,790.9418;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;1657.487,534.5102;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;233;1355.396,-187.3015;Float;False;828.4254;361.0605;Comment;5;115;118;117;119;225;Indirect Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;215;1944.082,816.0775;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;1956.552,537.3538;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;207;1626.013,899.7625;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;225;1405.396,-42.92128;Float;False;220;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;74;2114.005,542.3831;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;2134.626,851.7131;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;2155.415,983.4974;Float;False;Property;_ShadowContribution;Shadow Contribution;5;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;115;1688.455,-41.32622;Float;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;214;2324.502,854.6982;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;1659.391,58.75889;Float;False;Property;_IndirectDiffuseContribution;Indirect Diffuse Contribution;4;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;2351.156,541.2684;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;1785.008,-137.3016;Float;False;Constant;_Float4;Float 4;20;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;117;1999.821,-60.9863;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;107;2626.905,541.7966;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;73;2975.755,1079.272;Float;False;Property;_BaseTint;Base Tint;1;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;159;2707.211,856.7739;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;76;2937.31,875.5428;Float;True;Property;_BaseColorRGBOutlineWidthA;Base Color (RGB) Outline Width (A);0;1;[NoScaleOffset];Create;True;0;0;False;0;None;c0ef6662780210a40899b40e07ed6153;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;3048.637,509.9366;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;256;3276.777,970.2915;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;3050.939,734.3177;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;235;3423.945,954.0895;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;3307.784,623.0619;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3608.84,749.2598;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;219;3993.704,1541.79;Float;False;1039.617;429.9737;Comment;8;259;200;258;83;185;245;254;192;Custom Outline;1,0.6029412,0.7097364,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;257;3389.934,1086.04;Float;False;OutlineCustomWidth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;253;3801.573,744.8395;Float;False;BaseColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;192;4054.217,1683.832;Float;False;Property;_OutlineTint;Outline Tint;17;0;Create;True;0;0;False;0;0.5294118,0.5294118,0.5294118,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;254;4306.977,1596.032;Float;False;253;BaseColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;259;4345.463,1881.319;Float;False;257;OutlineCustomWidth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;4326.404,1785.057;Float;False;Property;_OutlineWidth;Outline Width;18;0;Create;True;0;0;False;0;0.02;0.0873;0.0001;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;245;4357.897,1686.073;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;240;-635.8909,943.4141;Float;False;717.6841;295.7439;Comment;4;229;228;230;227;Light Falloff;0.9947262,1,0.6176471,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;251;648.5698,-1229.416;Float;False;2234.221;738.9581;Comment;18;184;180;249;246;181;177;81;172;175;61;222;239;62;195;174;173;171;261;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;248;1892.391,-1558.685;Float;False;287;165;Comment;1;189;Spec/Smooth;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;243;1727.822,1561.833;Float;False;1926.522;520.1537;Comment;17;98;244;231;193;96;250;94;103;93;92;238;91;90;88;89;221;86;Rim Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;247;1687.587,-2100.743;Float;False;1008.755;365.3326;Comment;5;224;106;188;186;187;Indirect Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;258;4668.784,1790.126;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;4617.75,1638.073;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.3382353,0.3382353,0.3382353;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;88;2129.955,1708.719;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;106;2161.653,-1963.519;Float;False;World;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;94;2881.555,1712.936;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;252;4657.833,562.008;Float;False;253;BaseColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;98;2890.947,1874.34;Float;False;Property;_RimColor;Rim Color;14;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;227;-177.207,1056.042;Float;False;LightColorFalloff;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OutlineNode;83;4783.315,1639.851;Float;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;249;2211.476,-1055.947;Float;False;HighlightColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;2713.791,-1059.016;Float;False;5;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;193;3470.637,1634.762;Float;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;1777.822,1662.402;Float;False;220;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;3165.683,1641.71;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;186;2188.282,-2050.743;Float;False;Constant;_Float5;Float 5;20;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;250;3159.324,1739.908;Float;False;249;HighlightColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;231;3153.879,1829.875;Float;False;227;LightColorFalloff;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;246;2204.838,-950.8322;Float;False;227;LightColorFalloff;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;187;2512.345,-1997.898;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;263;2277.378,-1363.768;Float;False;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;4527.494,743.6974;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;172;2108.795,-795.9062;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;2072.556,1825.936;Float;False;Property;_RimOffset;Rim Offset;16;0;Create;True;0;0;False;0;0.6;0.469;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;103;2870.778,1633.693;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;244;3154.779,1926.118;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;175;2324.68,-792.1935;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;177;1329.485,-1179.416;Float;False;Property;_HighlightTint;Highlight Tint;7;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;1659.207,-1057.273;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;261;2021.558,-885.707;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;81;1260.552,-1012.602;Float;True;Property;_Highlight;Highlight;6;1;[NoScaleOffset];Create;True;0;0;False;0;None;c0ef6662780210a40899b40e07ed6153;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;86;1824.853,1773.611;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;62;1043.79,-679.1187;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;189;1942.391,-1508.685;Float;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;2353.555,1712.936;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;1003.499,-845.3307;Float;False;236;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;2094.48,-1850.412;Float;False;Property;_IndirectSpecularContribution;Indirect Specular Contribution;10;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;1799.042,-648.0834;Float;False;Property;_HighlightCellSharpness;Highlight Cell Sharpness;9;0;Create;True;0;0;False;0;0.01;0.01;0.001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;61;698.5698,-733.562;Float;False;Blinn-Phong Half Vector;-1;;3;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;238;2644.923,1611.833;Float;False;236;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;195;1271.122,-784.4333;Float;False;Property;_StaticHighLights;Static HighLights;11;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;222;762.1332,-605.4583;Float;False;220;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;180;1898.907,-1053.456;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-351.8918,1059.575;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;173;1405.781,-660.3005;Float;False;Property;_HighlightCellOffset;Highlight Cell Offset;8;0;Create;True;0;0;False;0;-0.95;-0.5;-1;-0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;93;2689.555,1712.936;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;171;1760.558,-788.2134;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;2116.472,-654.7482;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;1737.587,-1980.103;Float;False;220;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;229;-585.8909,1129.158;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;228;-540.1871,993.4141;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;92;2577.555,1840.936;Float;False;Property;_RimPower;Rim Power;15;0;Create;True;0;0;False;0;0.4;0.32;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;2513.555,1712.936;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;268;4955.406,845.8063;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;264;5183.611,952.4802;Float;False;True;2;Float;ASEMaterialInspector;0;3;ASESampleShaders/CustomOutlineToon;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;Hidden/InternalErrorShader;0;0;Standard;2;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;0;3;True;True;True;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;266;5309.991,551.4377;Float;False;False;2;Float;ASEMaterialInspector;0;1;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;265;5309.991,551.4377;Float;False;False;2;Float;ASEMaterialInspector;0;1;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
WireConnection;82;5;109;0
WireConnection;52;0;82;0
WireConnection;170;0;52;0
WireConnection;220;0;170;0
WireConnection;54;0;223;0
WireConnection;54;1;53;0
WireConnection;236;0;54;0
WireConnection;58;0;237;0
WireConnection;58;1;59;0
WireConnection;215;0;127;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;74;0;57;0
WireConnection;213;0;215;0
WireConnection;213;1;207;2
WireConnection;115;0;225;0
WireConnection;214;0;213;0
WireConnection;160;0;74;0
WireConnection;160;1;127;0
WireConnection;117;0;119;0
WireConnection;117;1;115;0
WireConnection;117;2;118;0
WireConnection;159;0;214;0
WireConnection;159;1;160;0
WireConnection;159;2;162;0
WireConnection;182;0;117;0
WireConnection;182;1;107;2
WireConnection;182;2;214;0
WireConnection;256;0;76;0
WireConnection;256;1;73;0
WireConnection;133;0;107;1
WireConnection;133;1;159;0
WireConnection;235;0;256;0
WireConnection;130;0;182;0
WireConnection;130;1;133;0
WireConnection;158;0;130;0
WireConnection;158;1;235;0
WireConnection;257;0;76;4
WireConnection;253;0;158;0
WireConnection;245;0;192;0
WireConnection;258;0;200;0
WireConnection;258;1;259;0
WireConnection;185;0;254;0
WireConnection;185;1;245;0
WireConnection;88;0;221;0
WireConnection;88;1;86;0
WireConnection;106;0;224;0
WireConnection;106;1;189;0
WireConnection;94;0;93;0
WireConnection;94;1;92;0
WireConnection;227;0;230;0
WireConnection;83;0;185;0
WireConnection;83;1;258;0
WireConnection;249;0;180;0
WireConnection;181;0;187;0
WireConnection;181;1;249;0
WireConnection;181;2;246;0
WireConnection;181;3;263;0
WireConnection;181;4;175;0
WireConnection;193;0;96;0
WireConnection;193;1;250;0
WireConnection;193;2;231;0
WireConnection;193;3;244;0
WireConnection;96;0;103;0
WireConnection;96;1;94;0
WireConnection;187;0;186;0
WireConnection;187;1;106;0
WireConnection;187;2;188;0
WireConnection;263;0;189;0
WireConnection;163;0;181;0
WireConnection;163;1;253;0
WireConnection;163;2;193;0
WireConnection;172;0;171;0
WireConnection;172;1;260;0
WireConnection;103;0;238;0
WireConnection;244;0;98;0
WireConnection;175;0;172;0
WireConnection;184;0;177;0
WireConnection;184;1;81;0
WireConnection;261;0;189;0
WireConnection;62;0;61;0
WireConnection;62;1;222;0
WireConnection;189;0;184;0
WireConnection;90;0;88;0
WireConnection;90;1;89;0
WireConnection;195;1;62;0
WireConnection;195;0;239;0
WireConnection;180;0;184;0
WireConnection;230;0;228;1
WireConnection;230;1;229;0
WireConnection;93;0;91;0
WireConnection;171;0;195;0
WireConnection;171;1;173;0
WireConnection;260;0;261;0
WireConnection;260;1;174;0
WireConnection;91;0;90;0
WireConnection;268;0;163;0
WireConnection;268;1;252;0
WireConnection;264;0;268;0
WireConnection;264;3;83;0
ASEEND*/
//CHKSM=6A9D46B8FCBA552F103A48E79AF1B261C5811C40
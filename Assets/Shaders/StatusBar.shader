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
		_ClipThreshold("ClipThreshold", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
    }

    SubShader
    {
		

        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Back
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
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
            #define _AlphaClip 1


            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"

			sampler2D _ToonLines;
			float4 _ToonLines_ST;
			sampler2D _Insignia;
			half _Health;
			float _Shield;
			half _Visibility;
			sampler2D _TextureSample0;
			float4 _TextureSample0_ST;
			half _ClipThreshold;

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float4 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct GraphVertexOutput
            {
                float4 position : POSITION;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			
            GraphVertexOutput vert (GraphVertexInput v)
            {
                GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue =  float3( 0, 0, 0 ) ;
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
				float2 uv_ToonLines = IN.ase_texcoord.xy * _ToonLines_ST.xy + _ToonLines_ST.zw;
				float2 uv010 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 tex2DNode4 = tex2D( _Insignia, uv010 );
				float4 temp_output_21_0 = (( tex2DNode4.b > 0.99 ) ? (( tex2DNode4.g < 0.01 ) ? (( tex2DNode4.r < 0.01 ) ? tex2DNode4 :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) );
				float4 temp_output_5_0 = (( tex2DNode4.r > 0.99 ) ? (( tex2DNode4.b < 0.01 ) ? (( tex2DNode4.g < 0.01 ) ? tex2DNode4 :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) ) :  float4( 0,0,0,0 ) );
				float4 temp_cast_0 = 0;
				
				float2 uv_TextureSample0 = IN.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 temp_cast_2 = (-0.5).xxxx;
				
		        float3 Color = ( tex2D( _ToonLines, uv_ToonLines ) * (( ( temp_output_21_0 + temp_output_5_0 ) == temp_cast_0 ) ? tex2DNode4 :  ( (( uv010.x < _Health ) ? temp_output_5_0 :  float4( 0,0,0,0 ) ) + (( uv010.x < _Shield ) ? temp_output_21_0 :  float4( 0,0,0,0 ) ) ) ) ).rgb;
		        float Alpha = ( 1.0 - ( ( 1.0 - _Visibility ) * ( tex2D( _TextureSample0, uv_TextureSample0 ) - temp_cast_2 ) ) ).r;
		        float AlphaClipThreshold = _ClipThreshold;
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
            #define _AlphaClip 1


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			half _Visibility;
			sampler2D _TextureSample0;
			float4 _TextureSample0_ST;
			half _ClipThreshold;

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float4 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
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
                
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue =  float3(0,0,0) ;
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
        		float2 uv_TextureSample0 = IN.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
        		float4 temp_cast_0 = (-0.5).xxxx;
        		

				float Alpha = ( 1.0 - ( ( 1.0 - _Visibility ) * ( tex2D( _TextureSample0, uv_TextureSample0 ) - temp_cast_0 ) ) ).r;
				float AlphaClipThreshold = _ClipThreshold;
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
            #define _AlphaClip 1


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			half _Visibility;
			sampler2D _TextureSample0;
			float4 _TextureSample0_ST;
			half _ClipThreshold;

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			
			VertexOutput vert( GraphVertexInput v  )
			{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.ase_texcoord.xy = v.ase_texcoord.xy;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord.zw = 0;
					float3 vertexValue =  float3(0,0,0) ;	
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
				float2 uv_TextureSample0 = IN.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 temp_cast_0 = (-0.5).xxxx;
				

				float Alpha = ( 1.0 - ( ( 1.0 - _Visibility ) * ( tex2D( _TextureSample0, uv_TextureSample0 ) - temp_cast_0 ) ) ).r;
				float AlphaClipThreshold = _ClipThreshold;

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
Version=16900
436;196;1484;748;-149.0054;864.6309;1;True;True
Node;AmplifyShaderEditor.SamplerNode;35;752.3634,-261.1178;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;30f384bc1d4410c4c9f953a6c08a3b28;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;1006.759,83.01083;Half;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;774.4674,-369.9956;Half;False;Property;_Visibility;Visibility;4;1;[PerRendererData];Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;54;963.6434,-492.6058;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;1283.367,17.82306;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;18;313.4566,-203.3081;Float;False;293.6;290.4;Health Slider;1;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;1366.173,-138.9799;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;13;-421.5623,-89.43976;Float;False;698.6;207;Is Red?;3;5;17;14;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;419.3759,522.4061;Float;False;293.6;290.4;Shield SLider;2;23;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-406.5013,199.991;Float;False;703.4;210.2;IsBlue;3;19;20;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;29;1035.866,210.8021;Float;False;403.2502;244.8244;If Blue is 0;1;30;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCCompareEqual;30;1202.753,272.0941;Float;False;4;0;COLOR;0,0,0,0;False;1;INT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;180.0483,-371.5872;Half;False;Property;_Health;Health;1;0;Create;True;0;0;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;37;1594.185,-174.3345;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;44;1347.742,574.1816;Float;True;Property;_ToonLines;Toon Lines;5;0;Create;True;0;0;False;0;None;e5d7e169bc52d1f4497b3d2a65240ce7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;32;952.751,-34.36201;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareGreater;21;73.93495,246.8147;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;14;-376.5019,-44.50895;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;12;379.7512,-66.87463;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-819.5015,89.09106;Half;False;Constant;_MinThreshold;MinThreshold;2;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;19;-375.2036,247.852;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1632.209,213.3864;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;53;1451.179,108.9078;Half;False;Property;_ClipThreshold;ClipThreshold;6;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;5;56.63674,-42.3462;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-842.2064,206.2857;Float;True;Property;_Insignia;Insignia;0;1;[PerRendererData];Create;True;0;0;False;0;None;217b5f3648a4834448b4d08015fe60c4;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-638.1622,9.660296;Half;False;Constant;_MaxThreshold;MaxThreshold;1;0;Create;True;0;0;False;0;0.99;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;652.4319,154.0035;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;437.1764,570.8059;Float;False;Property;_Shield;Shield;2;0;Create;True;0;0;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;17;-153.2597,-46.5525;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1090.75,-47.38965;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareLower;24;475.1762,661.2057;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;20;-159.9615,244.2086;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;31;1035.655,523.6927;Float;False;Constant;_Int1;Int 1;3;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;52;1813.418,-27.14296;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthOnly;True;0;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;50;1813.418,-27.14296;Half;False;True;2;Half;ASEMaterialInspector;0;3;SpaceCommander/StatusBar;e2514bdcf5e5399499a9eb24d175b9db;True;Base;0;0;Base;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;Hidden/InternalErrorShader;0;0;Standard;2;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;1;0;3;True;True;True;False;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;51;1813.418,-27.14296;Float;False;False;2;Float;ASEMaterialInspector;0;3;Hidden/Templates/LightWeightSRPUnlit;e2514bdcf5e5399499a9eb24d175b9db;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
WireConnection;54;0;34;0
WireConnection;47;0;35;0
WireConnection;47;1;48;0
WireConnection;49;0;54;0
WireConnection;49;1;47;0
WireConnection;30;0;33;0
WireConnection;30;1;31;0
WireConnection;30;2;4;0
WireConnection;30;3;32;0
WireConnection;37;0;49;0
WireConnection;32;0;12;0
WireConnection;32;1;24;0
WireConnection;21;0;4;3
WireConnection;21;1;7;0
WireConnection;21;2;20;0
WireConnection;14;0;4;2
WireConnection;14;1;15;0
WireConnection;14;2;4;0
WireConnection;12;0;10;1
WireConnection;12;1;9;0
WireConnection;12;2;5;0
WireConnection;19;0;4;1
WireConnection;19;1;15;0
WireConnection;19;2;4;0
WireConnection;45;0;44;0
WireConnection;45;1;30;0
WireConnection;5;0;4;1
WireConnection;5;1;7;0
WireConnection;5;2;17;0
WireConnection;4;1;10;0
WireConnection;33;0;21;0
WireConnection;33;1;5;0
WireConnection;17;0;4;3
WireConnection;17;1;15;0
WireConnection;17;2;14;0
WireConnection;24;0;10;1
WireConnection;24;1;23;0
WireConnection;24;2;21;0
WireConnection;20;0;4;2
WireConnection;20;1;15;0
WireConnection;20;2;19;0
WireConnection;50;0;45;0
WireConnection;50;1;37;0
WireConnection;50;2;53;0
ASEEND*/
//CHKSM=9AB450BA8EC0751431042F438C4E032648638D8D
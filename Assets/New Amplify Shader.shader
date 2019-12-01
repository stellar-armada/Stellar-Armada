// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stellar Armada/ShipHologramShader"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_Transparency("Transparency", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _Color;
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;
		uniform half _Transparency;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			half4 temp_output_3_0 = ( _Color * tex2DNode1.r );
			o.Albedo = temp_output_3_0.rgb;
			o.Emission = temp_output_3_0.rgb;
			half temp_output_8_0 = ( _Color.r + _Color.g + _Color.b );
			o.Alpha = ( ( _Transparency * temp_output_8_0 ) + ( temp_output_8_0 * tex2DNode1.r ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17200
233;617;1697;584;1007.5;109;1;True;True
Node;AmplifyShaderEditor.ColorNode;2;-740.5,-90;Half;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;0,0,0,0;0,0.6607184,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-868.5,293;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;True;0;-1;None;a8c818944d534424ea750aec8fa84bb0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-369.5,367;Half;False;Property;_Transparency;Transparency;3;0;Create;True;0;0;False;0;0;0.5;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-337.5,61;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-32.5,321;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-16.5,178;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-38.5,11;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;150.5,225;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;296,9;Half;False;True;2;ASEMaterialInspector;0;0;Standard;Stellar Armada/ShipHologramShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;False;Overlay;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;2;1
WireConnection;8;1;2;2
WireConnection;8;2;2;3
WireConnection;4;0;7;0
WireConnection;4;1;8;0
WireConnection;11;0;8;0
WireConnection;11;1;1;1
WireConnection;3;0;2;0
WireConnection;3;1;1;1
WireConnection;5;0;4;0
WireConnection;5;1;11;0
WireConnection;0;0;3;0
WireConnection;0;2;3;0
WireConnection;0;9;5;0
ASEEND*/
//CHKSM=61DA9BC4588BDBC3434A9AF9DD85D13FCAFC92B4
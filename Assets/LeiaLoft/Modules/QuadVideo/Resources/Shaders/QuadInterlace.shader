Shader "LeiaLoft/QuadInterlace"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]
        _VideoViewWidth ("Video View Width", Float) = 2560
        [HideInInspector]
        _ScaleTile ("ScaleTile", Vector) = (1, 1, 0, 0)
        [HideInInspector]
        _PositionOffsetX ("Video View Position offset", Float) = 0
        [HideInInspector]
        _CalibrationOffsetX ("Calibration offset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent+1000" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _VideoViewWidth;
            float4 _ScaleTile;
            float _PositionOffsetX;
            float _CalibrationOffsetX;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float2 tile_dimension = float2(0.5, 0.5);
                float pixel_x = floor(i.uv.x * _VideoViewWidth) + fmod(_PositionOffsetX + _CalibrationOffsetX + 2, 4.0);
                float viewId = fmod(pixel_x, 4.0);
                float2 tile_origin = float2(
                    fmod(viewId, 2.0) * tile_dimension.x, 
                    floor(viewId / 2.0) * tile_dimension.y);
                float2 uv_in_tile = i.uv * tile_dimension;
                float2 uv = tile_origin + uv_in_tile;

                return tex2D(_MainTex, uv * _ScaleTile.xy + _ScaleTile.zw);
            }
            ENDCG
        }
    }
}

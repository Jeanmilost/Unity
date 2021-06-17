// this shader was originally created by ocias (https://ocias.com/blog/unity-stipple-transparency-shader/)
Shader "Effects/TransparentDither" {
    // properties exposed onto the Unity editor
    Properties {
        _MainTex     ("Texture (RGB)", 2D)          = "white" {}
        _Color       ("Color",         Color)       = (1, 1, 1, 1)
        _Transparency("Transparency",  Range(0, 1)) = 1.0
        _Glossiness  ("Smoothness",    Range(0, 1)) = 0.5
        _Metallic    ("Metallic",      Range(0, 1)) = 0.0
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 150
        CGPROGRAM

        // a surface linked to a surface shader function (surf), with a physically based Standard lighting model,
        // and enabling shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // editor properties local variables
        sampler2D _MainTex;
        fixed4    _Color;
        half      _Transparency;
        half      _Glossiness;
        half      _Metallic;

        // input structure
        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        // surface shader function, may more or less be the equivalent of the OpenGL pixel shader
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // get the texture pixels and apply color modifier
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // configure output
            o.Albedo     = c.rgb;
            o.Metallic   = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha      = c.a;

            // screen-door transparency, discard pixel if below threshold
            float4x4 thresholdMatrix = {
                1.0  / 17.0, 9.0  / 17.0, 3.0  / 17.0, 11.0 / 17.0,
                13.0 / 17.0, 5.0  / 17.0, 15.0 / 17.0, 7.0  / 17.0,
                4.0  / 17.0, 12.0 / 17.0, 2.0  / 17.0, 10.0 / 17.0,
                16.0 / 17.0, 8.0  / 17.0, 14.0 / 17.0, 6.0  / 17.0
            };

            float4x4 _RowAccess = { 1, 0, 0, 0,
                                    0, 1, 0, 0,
                                    0, 0, 1, 0,
                                    0, 0, 0, 1 };
            float2   pos        = IN.screenPos.xy / IN.screenPos.w;

            // pixel position
            pos *= _ScreenParams.xy;
            clip(_Transparency - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
        }

        ENDCG
    }

    Fallback "Standard"
}

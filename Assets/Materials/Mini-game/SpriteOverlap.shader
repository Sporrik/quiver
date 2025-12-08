Shader "Custom/SpriteOverlap"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color   ("Tint", Color) = (1,1,1,1)

        // These match Unity's built-in sprite shaders
        _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        _AlphaTex      ("External Alpha", 2D) = "white" {}
        _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off         // don't write to depth
        ZTest Always       // ignore depth buffer, always draw
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnitySprites.cginc"   // uses Unity's sprite vert/frag

            ENDCG
        }
    }
}

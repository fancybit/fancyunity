Shader "FancyBit/2D/Fire-Sprite-Unlit"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _DisturbTex("Disturb Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _DisturbOffset("Disturb Offset", Vector) = (0,0,0,0)
        _DisturbOffsetScale("Disturb Offset Scale", Float) = 0.1
        _DisturbSizeScale("Disturb Size Scale", Vector) = (1,1,0,0)
        _UVRect("UV Rect", Vector) = (0,0,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
        #pragma vertex SpriteVert
        #pragma fragment SpriteFrag
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_local _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

        #include "UnityCG.cginc"

        #ifdef UNITY_INSTANCING_ENABLED
        UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
        // SpriteRenderer.Color while Non-Batched/Instanced.
        UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
        // this could be smaller but that's how bit each entry is regardless of type
        UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
        UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

        #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
        #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
        #endif // instancing

        CBUFFER_START(UnityPerDrawSprite)
        #ifndef UNITY_INSTANCING_ENABLED
            fixed4 _RendererColor;
            fixed2 _Flip;
        #endif
            float _EnableExternalAlpha;
        CBUFFER_END

        // Material Color.
        fixed4 _Color;

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color : COLOR;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
        {
            return float4(pos.xy * flip, pos.z, 1.0);
        }

        v2f SpriteVert(appdata_t IN)
        {
            v2f OUT;

            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
            OUT.vertex = UnityObjectToClipPos(OUT.vertex);
            OUT.texcoord = IN.texcoord;
            OUT.color = IN.color * _Color * _RendererColor;

            #ifdef PIXELSNAP_ON
            OUT.vertex = UnityPixelSnap(OUT.vertex);
            #endif

            return OUT;
        }

        sampler2D _MainTex;
        sampler2D _AlphaTex;
        sampler2D _DisturbTex;
        float2 _DisturbOffset;
        float _DisturbOffsetScale;
        float2 _DisturbSizeScale;
        float4 _UVRect;

        float2 RestrictRect(float2 uv,float4 rect){
            return clamp(uv,rect.xy,rect.xy+rect.zw);
        }

        fixed4 SampleSpriteTexture(float2 uv)
        {
            float4 disTex = tex2D(_DisturbTex, (uv+_DisturbOffset)*_DisturbSizeScale);
            float2 offsetUV = float2(disTex.r, disTex.g);
            offsetUV = (offsetUV-0.5)*2;
            offsetUV *=_DisturbOffsetScale;
            float2 mainTexUV = RestrictRect(uv + offsetUV,_UVRect);

            fixed4 color = tex2D(_MainTex, mainTexUV);

        #if ETC1_EXTERNAL_ALPHA
            fixed4 alpha = tex2D(_AlphaTex, mainTexUV);
            color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
        #endif

            return color;
        }

        fixed4 SpriteFrag(v2f IN) : SV_Target
        {
            fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
            c.rgb *= c.a;
            return c;
        }
        ENDCG
        }
    }
}
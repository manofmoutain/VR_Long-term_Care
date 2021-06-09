Shader "Unlit/NewUnlitShader"
{
    //顯示出來的面板屬性
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        //不透明
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            //CGPROGRAM  + ENDCG   = region
            CGPROGRAM

            //點-必要
            #pragma vertex vert

            //面-必要
            #pragma fragment frag

            // 霧化(沒有用到的話就直接刪除)
            // #pragma multi_compile_fog

            //圖形的處理-必要，基本上不用動 (如果有加上其他光源，就要放進光源的語法；沒有特別作光源渲染就不用管他)
            #include "UnityCG.cginc"

            //一開始生成的數據
            struct appdata
            {
                //這邊通常放的是POSITION COLOR TEN NORMAL TEXCOORD(UV、位置等數據-數據專用 )
                //
                //模型頂點的數據(POSITION一定都要大寫)
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            //頂點的計算
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            //頂點算完會傳給frag(片段)
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);


                // 霧化處理(基本上不會用到)
                // UNITY_APPLY_FOG(i.fogCoord, col);


                return col;
            }
            ENDCG
        }
    }
}

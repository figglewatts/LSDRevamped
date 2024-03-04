// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LSDR/Sky"
{
	Properties
	{
		_StartPos("StartPos", Range( -1 , 0)) = 0
		_FogColor("FogColor", Color) = (0,0,0,0)
		_SkyColor("SkyColor", Color) = (0,0,0,0)
		_Distribution("Distribution", Range( 0.1 , 1)) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Background" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform float4 _SkyColor;
			uniform float _StartPos;
			uniform float _Distribution;
			uniform float4 _FogColor;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord = v.vertex;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float temp_output_29_0 = ( saturate( ( ( _StartPos + i.ase_texcoord.xyz.y ) / _Distribution ) ) + saturate( ( ( _StartPos + ( i.ase_texcoord.xyz.y * -1.0 ) ) / _Distribution ) ) );
				
				
				finalColor = ( ( _SkyColor * temp_output_29_0 ) + ( ( 1.0 - temp_output_29_0 ) * _FogColor ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16200
63;406;1363;450;2028.227;564.7688;2.551207;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;2;-1040.583,-438.2788;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-1021.884,-516.1198;Float;False;Property;_StartPos;StartPos;0;0;Create;True;0;0;False;0;0;-0.1080533;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-778.2454,-121.4057;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1033.949,-633.048;Float;False;Property;_Distribution;Distribution;3;0;Create;True;0;0;False;0;0;0.2085721;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-585.5464,-580.4908;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-589.3386,-299.5211;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;-358.3141,-301.7948;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-347.9082,-454.7253;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;6;-186.695,-450.4403;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-181.1794,-304.3515;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-3.608483,-444.1239;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;30;297.5091,-184.0461;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;241.4746,-30.53859;Float;False;Property;_FogColor;FogColor;1;0;Create;True;0;0;False;0;0,0,0,0;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;246.8105,-630.0955;Float;False;Property;_SkyColor;SkyColor;2;0;Create;True;0;0;False;0;0,0,0,0;0.1172414,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;501.8484,-493.572;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;499.5683,-239.0835;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;785.0746,-358.0946;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;940.8556,-358.7664;Float;False;True;2;Float;ASEMaterialInspector;0;1;LSDR/Sky;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;True;False;0;False;-1;0;False;-1;True;2;RenderType=Opaque=RenderType;Queue=Background=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;25;0;2;2
WireConnection;5;0;3;0
WireConnection;5;1;2;2
WireConnection;26;0;3;0
WireConnection;26;1;25;0
WireConnection;22;0;26;0
WireConnection;22;1;7;0
WireConnection;8;0;5;0
WireConnection;8;1;7;0
WireConnection;6;0;8;0
WireConnection;28;0;22;0
WireConnection;29;0;6;0
WireConnection;29;1;28;0
WireConnection;30;0;29;0
WireConnection;17;0;4;0
WireConnection;17;1;29;0
WireConnection;15;0;30;0
WireConnection;15;1;1;0
WireConnection;18;0;17;0
WireConnection;18;1;15;0
WireConnection;0;0;18;0
ASEEND*/
//CHKSM=519C024FF825010D7E8F0DCF04687E4C9A036D25
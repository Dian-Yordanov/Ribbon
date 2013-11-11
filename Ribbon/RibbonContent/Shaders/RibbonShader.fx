float4x4 world : World;
float4x4 wvp : WorldViewProjection;

float AmbientIntensity = .25f;
float4 AmbientColor : AMBIENT = float4(0,0,0,1);

float DiffuseIntensity = 1;
float4 DiffuseColor : Diffuse = float4(1,0,0,1);

float3 LightDirection : Direction = float3(0,0,1);

float4 SpecularColor : Specular = float4(1,1,1,1);
float SpecularIntensity : Scalar = 1;

float3 CameraPosition : CameraPosition;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Light : TEXCOORD1;
    float3 Normal : TEXCOORD2;
	float3 CamView : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position,wvp);
    output.Light = LightDirection;
    output.Normal = mul(input.Normal,world);
    output.CamView = CameraPosition - mul(input.Position,world);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 Norm = normalize(input.Normal);
    float3 LightDir = normalize(input.Light);

	float alpha = .5f;//pow(1-distance(float2(0,.5),float2(0,input.TexCoord.y)),5);

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = (DiffuseIntensity * DiffuseColor) * saturate(dot(LightDir,Norm));

	float3 Half = normalize(LightDir + normalize(input.CamView));    
    float specular = (SpecularColor * SpecularIntensity) * pow(saturate(dot(Norm,Half)),25);
		
    
	float4 col = (ambient + diffuse + specular);
	//col.a = alpha;
	return col;
	
	//return float4(input.TexCoord,0,alpha);
}

technique Technique1
{
    pass Pass1
    {
	    VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

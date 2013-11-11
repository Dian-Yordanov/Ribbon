float4x4 world : World;
float4x4 wvp : WorldViewProjection;

float AmbientIntensity = .125f;
float4 AmbientColor : AMBIENT = float4(0,0,0,1);

float DiffuseIntensity = 1;
float4 DiffuseColor : Diffuse = float4(1,0,0,1);

float3 LightDirection : Direction = float3(1,1,0);

float4 SpecularColor : Specular = float4(1,1,1,1);
float SpecularIntensity : Scalar = 1;

float3 CameraPosition : CameraPosition;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Light : TEXCOORD0;
    float3 Normal : TEXCOORD1;
	float3 CamView : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position,wvp);
    output.Light = LightDirection;
    output.Normal = mul(input.Normal,world);
    output.CamView = CameraPosition - mul(input.Position,world);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 Norm = normalize(input.Normal);
    float3 LightDir = normalize(input.Light);

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = (DiffuseIntensity * DiffuseColor) * saturate(dot(LightDir,Norm));

	float3 Half = normalize(LightDir + normalize(input.CamView));    
    float specular = (SpecularColor * SpecularIntensity) * pow(saturate(dot(Norm,Half)),25);
    
	return pow(ambient + diffuse + specular,5);
}

technique Technique1
{
    pass Pass1
    {
	    VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

/*****************************************HEADER*****************************************/
/*Camera Information*/
float4x4 World;
float4x4 View;
float4x4 Project;
/****************************************************************************************/
/*Texture Information*/
texture ModelTexture;
bool PerformTile = false;
float TextureScaleX = 1.0f;
float TextureScaleY = 1.0f;
float TextureOffsetX = 0.0f;
float TextureOffsetY = 0.0f;
int VerticalTexCoord = 1;
int HorizontalTexCoord = 1;
/****************************************************************************************/
/*Ambient Light Information*/
float4 AmbientColor;
float AmbientIntensity;
/****************************************************************************************/
/*Diffuse Light Information*/
float4 DiffuseColor;
float3 DiffuseLightDirection = { 0.0f, 1.0f, 1.0f };
float DiffuseIntensity;
bool DiffuseEnabled = false;
/****************************************************************************************/
/****************************************FUNCTIONS***************************************/
/*Responsible for assigning a texture to our object*/
sampler TextureSampler = sampler_state 
{
    texture = <ModelTexture>;
    magfilter = ANISOTROPIC;
    minfilter = ANISOTROPIC;
    mipfilter= LINEAR;
    MaxAnisotropy = 1;
    AddressU = WRAP;
    AddressV = WRAP;
};
/****************************************************************************************/
/*Param Struct for our Vertex Shader*/
struct VS_INPUT 
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoords : TEXCOORD0;
};
/****************************************************************************************/
/*Return Struct for our Vertex Shader on the output*/
struct VS_OUTPUT
{
	float4 Color : COLOR0;
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
};
/****************************************************************************************/
/*A function called "VertexShader" that uses the Vertex Shader Container VS_OUTPUT and returns it.*/
VS_OUTPUT VertexShader(VS_INPUT Input, VS_OUTPUT Output)
{
	//VS_OUTPUT Output;
	/*Matrix Information*/
	Input.Position = mul(Input.Position, World);
	Input.Position = mul(Input.Position, View);
	Input.Position = mul(Input.Position, Project);
	
	/*Assign VS_OUTPUT Information*/
	Output.Position = Input.Position; //<-- Update our Position based on the transformations
	Output.TexCoords = Input.TexCoords; //<-- Pass on our Texture Cordinates
	
	if(DiffuseEnabled)
	{
		float4 Normal = normalize(mul(Input.Normal, World)); //<-- Calculate the Normal for Lighting
		float3 DiffuseLightDirectionN = normalize(DiffuseLightDirection); //<-- Normalize our Lighting Direction
		float LightIntensity = dot(Normal, DiffuseLightDirectionN); //<-- Dot the value
		Output.Color = saturate(DiffuseColor * DiffuseIntensity * LightIntensity);
	}
	
	/*Return the Output Data*/
	return Output; 
}
/****************************************************************************************/
/*A function using the PixelShader Input struct*/
float4 PixelShader(VS_OUTPUT Output) : COLOR0 
{
	// Calculate our ambient component
	if(DiffuseEnabled)
    AmbientColor = saturate((AmbientIntensity * AmbientColor) + Output.Color);
	
	Output.TexCoords.x *= TextureScaleX * HorizontalTexCoord;
	Output.TexCoords.y *= TextureScaleY * VerticalTexCoord;
	Output.TexCoords.x += TextureOffsetX;
	Output.TexCoords.y += TextureOffsetY;
	Output.Color = tex2D(TextureSampler, Output.TexCoords); //<-- Pull Texture Colors
	if(DiffuseEnabled)
    Output.Color.xyz = Output.Color.xyz * AmbientColor;

	return Output.Color.xyzw; //<-- Finally send out our Color information
};
/****************************************************************************************/
/*Technique or the type of shader to be ran*/
technique DrawModel 
{
	pass P0 
	{
		VertexShader = compile vs_2_0 VertexShader();
		PixelShader  = compile ps_2_0 PixelShader();
	}
}
/******************************************EOF*******************************************/
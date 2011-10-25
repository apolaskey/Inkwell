float4x4 WVPMatrix;
float4x4 World;
float4x4 View;
float4x4 Projection;
float ViewportHeight;
float OrigSize = 5.0f;
texture SpriteTexture;

struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TexCoords : TEXCOORD0;
};
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TexCoords : TEXCOORD0;
	float Size : PSIZE;
};
struct PS_INPUT
{
	float4 Color: COLOR0;
	float4 TexColor: COLOR1;
	float2 TexCoord : TEXCOORD0;    
};
sampler Sampler = sampler_state
{
	texture = <SpriteTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter= LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

VS_OUTPUT VertexShader(VS_INPUT Input)
{
	VS_OUTPUT Output;
	
	Output.Color = Input.Color;
	Output.TexCoords = Input.TexCoords;
	Output.Position = mul(Input.Position, WVPMatrix);
	Output.Size = length( Output.Position ); //Temp just storing a dummy value
	Output.Size = ViewportHeight * OrigSize * sqrt(1.0f / Output.Size);
	return Output;
}
						
float4 PixelShader(PS_INPUT Input): COLOR0
{
	Input.TexColor = tex2D(Sampler, Input.TexCoord);
    Input.Color.xyz = saturate(Input.Color.xyz * Input.TexColor.xyz);
    Input.TexColor.xyz = Input.Color.xyz;
    //float4 TextureColor = tex2D(Sampler, Input.TexCoord);
    //Input.Color.xyz = saturate(Input.Color.xyz * TextureColor.xyz);
    //TextureColor.xyz = Input.Color.xyz;
    return Input.TexColor;
}



technique PointSprites
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader = compile ps_2_0 PixelShader();		
	}
}    

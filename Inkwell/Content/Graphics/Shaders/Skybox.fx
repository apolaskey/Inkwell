float4x4 World;
float4x4 View;
float4x4 Projection;
 
float3 CameraPosition;
float Offset = 0.0f;
 
Texture SkyBoxTexture; 
samplerCUBE SkyBoxSampler = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 TexCords : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    output.TextureCoordinate = VertexPosition - CameraPosition;
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate));
}
 
technique Skybox
{
    pass P0
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


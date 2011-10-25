texture RenderTarget;

sampler TextureSampler = sampler_state
{
	texture = <RenderTarget>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter= LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

struct PS_INPUT
{
	float4 Color : COLOR0;
	float2 TexCoords : TEXCOORD0;
};

float4 Grayscale(PS_INPUT Input) : COLOR0
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, Input.TexCoords);
    
    // Convert it to greyscale. The constants 0.3, 0.59, and 0.11 are because
    // the human eye is more sensitive to green light, and less to blue.
    float greyscale = dot(tex.rgb, float3(0.3, 0.59, 0.11));
    
    // The input color alpha controls saturation level.
    tex.rgb = lerp(greyscale, tex.rgb, Input.Color.a * 4);
    
    return tex;
}

technique Grayscale
{
    pass Pass1
    {
        // TODO: set renderstates here.
        VertexShader = null;
        PixelShader = compile ps_2_0 Grayscale();
    }
}

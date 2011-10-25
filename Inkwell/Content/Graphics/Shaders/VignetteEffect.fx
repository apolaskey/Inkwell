sampler2D input : register(s0);

float VignetteRadius : register(C0);
 
float4 main(float2 uv : TEXCOORD) : COLOR 
{
    float4 color = tex2D( input , uv.xy);
 
    /*Find the Distance to the Center of the Screen*/
    float2 dist = 0.5 - uv;
     
    // Vignette effect Calc
    color.rgb *= (0.6 + (-VignetteRadius * 0.08) - dot(dist, dist)) * 1.80;
 
    return color;
}

technique Vignette
{
    pass P0
    {
        PixelShader = compile ps_2_0 main();
    }
}

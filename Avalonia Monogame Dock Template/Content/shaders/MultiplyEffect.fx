sampler2D TextureSampler : register(s0);

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = tex2D(TextureSampler, texCoord);
    float4 blendColor = float4(1, 1, 1, 1); // Môžeš ho zmeniť na vlastný blendovací odtieň
    return color;
}

technique Multiply
{
    pass P0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}

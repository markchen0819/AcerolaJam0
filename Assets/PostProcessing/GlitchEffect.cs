using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(GlitchRenderer), PostProcessEvent.AfterStack, "Custom/GlitchEffect")]
public sealed class GlitchEffect : PostProcessEffectSettings
{
    [Range(1f, 1024f), Tooltip("Block size")]
    public FloatParameter blockSize = new FloatParameter { value = 20.0f };
    [Range(-5.0f, 5.0f), Tooltip("Displacement Amount")]
    public FloatParameter displacementAmount = new FloatParameter { value = 0.1f };
}

public sealed class GlitchRenderer : PostProcessEffectRenderer<GlitchEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/GlitchShader"));
        if (settings.blockSize != null)
        {
            sheet.properties.SetFloat("_BlockSize", settings.blockSize.value);
            sheet.properties.SetFloat("_DisplacementAmount", settings.displacementAmount.value);
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

// https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Writing-Custom-Effects.html
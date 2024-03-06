using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PixelRenderer), PostProcessEvent.AfterStack, "Custom/PixelEffect")]
public sealed class PixelEffect : PostProcessEffectSettings
{
    [Range(1f, 1024f), Tooltip("Pixelation amount")]
    public FloatParameter pixelAmount = new FloatParameter { value = 100f };
}

public sealed class PixelRenderer : PostProcessEffectRenderer<PixelEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/PixelShader"));
        if (settings.pixelAmount != null)
        {
            sheet.properties.SetFloat("_PixelationAmount", settings.pixelAmount.value);
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

// https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Writing-Custom-Effects.html
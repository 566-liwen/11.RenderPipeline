using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class RenderPipelineInstance : RenderPipeline
{
    protected ExampleRenderPipelineAsset _setting;
    protected CommandBuffer _commandbuffer;

    private List<RenderTexture> _GBuffers = new List<RenderTexture>();
    private RenderTargetIdentifier[] _GBufferRTIs;
    private int[] _GBufferNameIDs = {
            ShaderConstants.GBuffer0,
            ShaderConstants.GBuffer1,
            ShaderConstants.GBuffer2,
            ShaderConstants.GBuffer3,
        };
    private RenderTextureFormat[] _GBufferFormats = {
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.ARGB32
        };

    private RenderTexture _depthTexture;
    private const string LightModeId = "Deferred";
    private const string LightModeId2 = "Deferred2";
    private RenderObjectPass _opaquePass = new RenderObjectPass(false, LightModeId, false);
    private RenderObjectPass _opaquePass2 = new RenderObjectPass(false, LightModeId2, false);
    private DeferredLightingPass _deferredLightingPass = new DeferredLightingPass();

    public RenderPipelineInstance(ExampleRenderPipelineAsset setting)
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
        _setting = setting;
        _commandbuffer = new CommandBuffer();
        _commandbuffer.name = "RP";
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        // pipeline begin
        foreach (var camera in cameras)
        {
            RenderPerCamera(context, camera);
        }
        context.Submit();
        // pipeline ended
    }

    private void RenderPerCamera(ScriptableRenderContext context, Camera camera)
    {
        context.SetupCameraProperties(camera);
        camera.TryGetCullingParameters(out var cullingParams);
        var cullingResults = context.Cull(ref cullingParams);
        this.OnPostCameraCulling(context, camera, ref cullingResults);
    }

    protected void OnPostCameraCulling(ScriptableRenderContext context, Camera camera, ref CullingResults cullingResults)
    {
        //var cameraDesc = Utils.GetCameraRenderDescription(camera, _setting);
        this.ConfigMRT(context, camera);
        //_opaquePass.Execute(context, camera, ref cullingResults);
        _opaquePass2.Execute(context, camera, ref cullingResults);
        _commandbuffer.Clear();
        _commandbuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        context.ExecuteCommandBuffer(_commandbuffer);
        _deferredLightingPass.Execute(context);
    }

    private void ConfigMRT(ScriptableRenderContext context, Camera camera)
    {
        this.AcquireGBuffersIfNot(context, camera);
        _commandbuffer.Clear();
        //todo: _depthTexture
        RenderTextureDescriptor depthDesc = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight, RenderTextureFormat.Depth, 32, 1);
        RenderTexture _depthTexture = RenderTexture.GetTemporary(depthDesc);
        _depthTexture.Create();
        _commandbuffer.SetRenderTarget(_GBufferRTIs, _depthTexture);
        _commandbuffer.ClearRenderTarget(true, true, camera.backgroundColor);
        context.ExecuteCommandBuffer(_commandbuffer);
    }

    private void AcquireGBuffersIfNot(ScriptableRenderContext context, Camera camera)
    {
        if (_GBuffers.Count > 0)
        {
            var g0 = _GBuffers[0];
            if (g0.width != camera.pixelWidth || g0.height != camera.pixelHeight)
            {
                this.ReleaseGBuffers();
            }
        }
        if (_GBuffers.Count == 0)
        {
            _commandbuffer.Clear();
            _GBufferRTIs = new RenderTargetIdentifier[4];
            for (var i = 0; i < 4; i++)
            {
                RenderTextureDescriptor descriptor = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight, _GBufferFormats[i], 0, 1);
                var rt = RenderTexture.GetTemporary(descriptor);
                rt.filterMode = FilterMode.Bilinear;
                rt.Create();
                _GBuffers.Add(rt);
                _commandbuffer.SetGlobalTexture(_GBufferNameIDs[i], rt);
                _GBufferRTIs[i] = rt;
            }
            context.ExecuteCommandBuffer(_commandbuffer);
        }
    }

    private void ReleaseGBuffers()
    {
        if (_GBuffers.Count > 0)
        {
            foreach (var g in _GBuffers)
            {
                if (g)
                {
                    RenderTexture.ReleaseTemporary(g);
                }
            }
            _GBuffers.Clear();
            _GBufferRTIs = null;
        }
    }


    public static class ShaderConstants
    {
        public static readonly int GBuffer0 = Shader.PropertyToID("_GBuffer0");
        public static readonly int GBuffer1 = Shader.PropertyToID("_GBuffer1");
        public static readonly int GBuffer2 = Shader.PropertyToID("_GBuffer2");
        public static readonly int GBuffer3 = Shader.PropertyToID("_GBuffer3");

        /*public static readonly int CameraColorTexture = Shader.PropertyToID("_CameraColorTexture");

        public static readonly int CameraDepthTexture = Shader.PropertyToID("_XDepthTexture");
        public static readonly int DeferredDebugMode = Shader.PropertyToID("_DeferredDebugMode");

        public static readonly int TileCullingIntersectAlgroThreshold = Shader.PropertyToID("_TileCullingIntersectAlgroThreshold");*/
    }
}
namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    /// <summary>
    /// Start rendering that will support XR
    ///
    /// This pass is used to enable XR rendering. This type of
    /// XR rendering that will be used will be what is currently
    /// configured in the global XR settings.
    /// </summary>
    public class BeginXRRenderingPass : ScriptableRenderPass
    {
        /// <inheritdoc/>
        public override void Execute(ScriptableRenderer renderer, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            context.StartMultiEye(camera);
        }
    }
}

namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    /// <summary>
    /// End XR rendering
    ///
    /// This pass is used to disable XR rendering. This should
    /// be paired with a BeginXRRendering pass.
    /// </summary>
    public class EndXRRenderingPass : ScriptableRenderPass
    {
        /// <inheritdoc/>
        public override void Execute(ScriptableRenderer renderer, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            context.StopMultiEye(camera);
            context.StereoEndRender(camera);
        }
    }
}

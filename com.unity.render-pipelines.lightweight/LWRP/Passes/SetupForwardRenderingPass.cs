namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    /// <summary>
    /// Set up camera properties for the current pass.
    ///
    /// This pass is used to configure shader uniforms and other unity properties that are required for rendering.
    /// * Setup Camera RenderTarget and Viewport
    /// * VR Camera Setup and SINGLE_PASS_STEREO props
    /// * Setup camera view, proj and their inv matrices.
    /// * Setup properties: _WorldSpaceCameraPos, _ProjectionParams, _ScreenParams, _ZBufferParams, unity_OrthoParams
    /// * Setup camera world clip planes props
    /// * setup HDR keyword
    /// * Setup global time properties (_Time, _SinTime, _CosTime)
    /// </summary>
    public class SetupForwardRenderingPass : ScriptableRenderPass
    {
        /// <inheritdoc/>
        public override void Execute(ScriptableRenderer renderer, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            context.SetupCameraProperties(renderingData.cameraData.camera, renderingData.cameraData.isStereoEnabled);
        }
    }
}

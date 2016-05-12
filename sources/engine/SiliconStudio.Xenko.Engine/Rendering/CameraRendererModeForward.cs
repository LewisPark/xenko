// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System.ComponentModel;
using SiliconStudio.Core;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering.Shadows;

namespace SiliconStudio.Xenko.Rendering
{
    /// <summary>
    /// A forward rendering mode.
    /// </summary>
    [DataContract("CameraRendererModeForward")]
    [Display("Forward")]
    public class CameraRendererModeForward : CameraRenderModeBase
    {
        private MeshPipelinePlugin meshPipelinePlugin;

        // TODO This should be exposed to the user at some point
        private bool enableDepthAsShaderResource = true;

        [DataMemberIgnore] public RenderStage MainRenderStage { get; set; }
        [DataMemberIgnore] public RenderStage TransparentRenderStage { get; set; }
        //[DataMemberIgnore] public RenderStage GBufferRenderStage { get; set; }
        [DataMemberIgnore] public RenderStage ShadowMapRenderStage { get; set; }

        [DefaultValue(true)]
        [DataMemberIgnore]
        public bool Shadows { get; set; } = true;

        //public bool GBuffer { get; set; } = false;

        protected override void InitializeCore()
        {
            base.InitializeCore();

            // Create mandatory render stages that don't exist yet
            if (MainRenderStage == null)
                MainRenderStage = RenderSystem.GetOrCreateRenderStage("Main", "Main", new RenderOutputDescription(GraphicsDevice.Presenter.BackBuffer.ViewFormat, GraphicsDevice.Presenter.DepthStencilBuffer.ViewFormat));
            if (TransparentRenderStage == null)
                TransparentRenderStage = RenderSystem.GetOrCreateRenderStage("Transparent", "Main", new RenderOutputDescription(GraphicsDevice.Presenter.BackBuffer.ViewFormat, GraphicsDevice.Presenter.DepthStencilBuffer.ViewFormat));
           
            // Setup stage RenderOutputDescription (since we have the render frame bound)
            var output = Context.Tags.Get(RenderFrame.Current);
            MainRenderStage.Output = output.GetRenderOutputDescription();
            TransparentRenderStage.Output = output.GetRenderOutputDescription();

            // Setup proper sort modes
            MainRenderStage.SortMode = new StateChangeSortMode();
            TransparentRenderStage.SortMode = new BackToFrontSortMode();

            // Create optional render stages that don't exist yet
            //if (GBufferRenderStage == null)
            //    GBufferRenderStage = RenderSystem.GetOrCreateRenderStage("GBuffer", "GBuffer", new RenderOutputDescription(PixelFormat.R11G11B10_Float, GraphicsDevice.Presenter.DepthStencilBuffer.ViewFormat));
            if (Shadows)
            {
                if (ShadowMapRenderStage == null)
                {
                    ShadowMapRenderStage = RenderSystem.GetOrCreateRenderStage("ShadowMapCaster", "ShadowMapCaster", new RenderOutputDescription(PixelFormat.None, PixelFormat.D32_Float));
                    ShadowMapRenderStage.SortMode = new FrontToBackSortMode();
                }

                RenderSystem.PipelinePlugins.InstantiatePlugin<ShadowPipelinePlugin>();
                meshPipelinePlugin = RenderSystem.PipelinePlugins.InstantiatePlugin<MeshPipelinePlugin>();
            }

            MainRenderView.RenderStages.Add(MainRenderStage);
            MainRenderView.RenderStages.Add(TransparentRenderStage);
        }

        protected override void DrawCore(RenderDrawContext context)
        {
            var currentViewport = context.CommandList.Viewport;

            // GBuffer
            //if (GBuffer)
            //{
            //    context.PushRenderTargets();
            //
            //    var gbuffer = PushScopedResource(Context.Allocator.GetTemporaryTexture2D((int)currentViewport.Width, (int)currentViewport.Height, GBufferRenderStage.Output.RenderTargetFormat0));
            //    context.CommandList.Clear(gbuffer, Color4.Black);
            //    context.CommandList.SetDepthAndRenderTarget(context.CommandList.DepthStencilBuffer, gbuffer);
            //    RenderSystem.Draw(context, mainRenderView, GBufferRenderStage);
            //
            //    context.PopRenderTargets();
            //}

            // Shadow maps
            var shadowMapRenderer = meshPipelinePlugin?.ForwardLightingRenderFeature?.ShadowMapRenderer;
            if (Shadows && shadowMapRenderer != null)
            {
                // Clear atlases
                shadowMapRenderer.PrepareAtlasAsRenderTargets(context.CommandList);

                context.PushRenderTargets();

                // Draw all shadow views generated for the current view
                foreach (var renderView in RenderSystem.Views)
                {
                    var shadowmapRenderView = renderView as ShadowMapRenderView;
                    if (shadowmapRenderView != null && shadowmapRenderView.RenderView == MainRenderView)
                    {
                        var shadowMapRectangle = shadowmapRenderView.Rectangle;
                        shadowmapRenderView.ShadowMapTexture.Atlas.RenderFrame.Activate(context);
                        shadowmapRenderView.ShadowMapTexture.Atlas.MarkClearNeeded();
                        context.CommandList.SetViewport(new Viewport(shadowMapRectangle.X, shadowMapRectangle.Y, shadowMapRectangle.Width, shadowMapRectangle.Height));

                        RenderSystem.Draw(context, shadowmapRenderView, ShadowMapRenderStage);
                    }
                }

                context.PopRenderTargets();

                shadowMapRenderer.PrepareAtlasAsShaderResourceViews(context.CommandList);
            }

            // Draw [main view | main stage]
            RenderSystem.Draw(context, MainRenderView, MainRenderStage);

            if (enableDepthAsShaderResource)
            {
                // Resolve Depth as a texture
                var currentRenderFrame = context.RenderContext.Tags.Get(RenderFrame.Current);

                if (currentRenderFrame.DepthBufferResolver == null)
                {
                    currentRenderFrame.DepthBufferResolver = new DepthBufferResolver();
                }

                currentRenderFrame.DepthBufferResolver.Resolve(context, currentRenderFrame.DepthStencil);

                currentRenderFrame.Activate(context, currentRenderFrame.DepthBufferResolver.AsRenderTarget());
            }

            // Draw [main view | transparent stage]
            RenderSystem.Draw(context, MainRenderView, TransparentRenderStage);

            if (enableDepthAsShaderResource)
            {
                var currentRenderFrame = context.RenderContext.Tags.Get(RenderFrame.Current);

                // Release
                currentRenderFrame.DepthBufferResolver?.Reset();
            }
        }
    }
}
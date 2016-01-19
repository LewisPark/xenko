// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Graphics;

namespace SiliconStudio.Xenko.Rendering.Shadows
{
    /// <summary>
    /// An atlas of shadow maps.
    /// </summary>
    public class ShadowMapAtlasTexture
    {
        private readonly GuillotinePacker packer = new GuillotinePacker();

        private bool isRenderTargetCleared;

        public ShadowMapAtlasTexture(Texture texture, int textureId)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));
            Texture = texture;
            packer.Clear(Texture.Width, Texture.Height);
            Width = texture.Width;
            Height = texture.Height;

            RenderFrame = RenderFrame.FromTexture((Texture)null, texture);
            Id = textureId;
        }

        public int Id { get; private set; }

        public readonly int Width;

        public readonly int Height;

        public Type FilterType;

        public readonly Texture Texture;

        public readonly RenderFrame RenderFrame;

        public void Clear()
        {
            packer.Clear();
            isRenderTargetCleared = false;
        }

        public bool Insert(int width, int height, ref Rectangle bestRectangle)
        {
            return packer.Insert(width, height, ref bestRectangle);
        }

        public bool TryInsert(int width, int height, int count, GuillotinePacker.InsertRectangleCallback inserted)
        {
            return packer.TryInsert(width, height, count, inserted);
        }

        public void ClearRenderTarget(RenderContext context)
        {
            if (!isRenderTargetCleared)
            {
                context.GraphicsDevice.Clear(Texture, DepthStencilClearOptions.DepthBuffer);
                isRenderTargetCleared = true;
            }
        }
    }
}
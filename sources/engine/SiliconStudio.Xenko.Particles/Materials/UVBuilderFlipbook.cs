﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Particles.Sorters;
using SiliconStudio.Xenko.Particles.VertexLayouts;

namespace SiliconStudio.Xenko.Particles.Materials
{
    /// <summary>
    /// Animates the texture coordinates in a flipbook fashion, based on the particle's life
    /// The order of the frames is left to right, top to bottom
    /// The flipbook assumes uniform sizes for all frames
    /// </summary>
    [DataContract("UVBuilderFlipbook")]
    [Display("Flipbook")]
    public class UVBuilderFlipbook : UVBuilder
    {
        private UInt32 xDivisions = 4;
        private UInt32 yDivisions = 4;
        private float xStep = 0.25f;
        private float yStep = 0.25f;
        private UInt32 totalFrames = 16;
        private UInt32 startingFrame = 0;
        private UInt32 animationSpeedOverLife = 16;

        /// <summary>
        /// Number of columns (cells per row)
        /// </summary>
        /// <userdoc>
        /// How many columns (divisions along the width/X-axis) should the flipbook have.
        /// </userdoc>
        [DataMember(200)]
        [Display("X divisions")]
        public UInt32 XDivisions
        {
            get { return xDivisions; }
            set
            {
                xDivisions = (value > 0) ? value : 1;
                xStep = (1f / xDivisions);
                totalFrames = xDivisions * yDivisions;
                startingFrame = Math.Min(startingFrame, totalFrames);
            }
        }

        /// <summary>
        /// Number of rows (cells per column)
        /// </summary>
        /// <userdoc>
        /// How many rows (divisions along the height/Y-axis) should the flipbook have.
        /// </userdoc>
        [DataMember(240)]
        [Display("Y divisions")]
        public UInt32 YDivisions
        {
            get { return yDivisions; }
            set
            {
                yDivisions = (value > 0) ? value : 1;
                yStep = (1f / yDivisions);
                totalFrames = xDivisions * yDivisions;
                startingFrame = Math.Min(startingFrame, totalFrames);
            }
        }

        /// <summary>
        /// Position of the starting frame, 0-based indexing
        /// </summary>
        /// <userdoc>
        /// Index of the starting frame in a 0-based indexing. Frames increase to the right first, before going down after the end of a row.
        /// </userdoc>
        [DataMember(280)]
        [Display("Starting frame")]
        public UInt32 StartingFrame
        {
            get { return startingFrame; }
            set
            {
                startingFrame = value;
                startingFrame = Math.Min(startingFrame, totalFrames);
            }
        }

        /// <summary>
        /// Number of frames to change over the particle life
        /// </summary>
        /// <userdoc>
        /// How many frames does the animation have over the particle's lifetime. Speed = X * Y means all frames are played exactly once.
        /// </userdoc>
        [DataMember(320)]
        [Display("Animation speed")]
        public UInt32 AnimationSpeed
        {
            get { return animationSpeedOverLife; }
            set { animationSpeedOverLife = value; }
        }

        /// <inheritdoc />
        public unsafe override void BuildUVCoordinates(ParticleVertexBuilder vertexBuilder, ParticleSorter sorter, AttributeDescription texCoordsDescription)
        {
            var lifeField = sorter.GetField(ParticleFields.RemainingLife);

            if (!lifeField.IsValid())
                return;

            var texAttribute = vertexBuilder.GetAccessor(texCoordsDescription);
            if (texAttribute.Size == 0 && texAttribute.Offset == 0)
            {
                return;
            }

            var texDefault = vertexBuilder.GetAccessor(vertexBuilder.DefaultTexCoords);
            if (texDefault.Size == 0 && texDefault.Offset == 0)
            {
                return;
            }


            foreach (var particle in sorter)
            {
                var normalizedTimeline = 1f - *(float*)(particle[lifeField]);

                var spriteId = startingFrame + (int)(normalizedTimeline * animationSpeedOverLife);

                var uvTransform = new Vector4((spriteId % xDivisions) * xStep, (spriteId / yDivisions) * yStep, xStep, yStep);

                ParticleVertexBuilder.TransformAttributeDelegate<Vector2> transformCoords =
                    (ref Vector2 value) =>
                    {
                        value.X = uvTransform.X + uvTransform.Z * value.X;
                        value.Y = uvTransform.Y + uvTransform.W * value.Y;
                    };

                vertexBuilder.TransformAttributePerParticle(texDefault, texAttribute, transformCoords);

                vertexBuilder.NextParticle();
            }


            vertexBuilder.RestartBuffer();
        }
    }
}

﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Assets;
using SiliconStudio.Core;

namespace SiliconStudio.Xenko.Assets.Entities
{
    [DataContract("EntityGroupAsset")]
    [AssetDescription(FileExtension, false)]
    //[AssetCompiler(typeof(SceneAssetCompiler))]
    //[ThumbnailCompiler(PreviewerCompilerNames.EntityThumbnailCompilerQualifiedName, true)]
    [Display("Entity")]
    //[AssetFormatVersion(AssetFormatVersion, typeof(Upgrader))]
    public class EntityGroupAsset : EntityGroupAssetBase
    {
        public const int AssetFormatVersion = 0;

        /// <summary>
        /// The default file extension used by the <see cref="EntityGroupAsset"/>.
        /// </summary>
        public const string FileExtension = ".xkentity;.pdxentity";
    }
}
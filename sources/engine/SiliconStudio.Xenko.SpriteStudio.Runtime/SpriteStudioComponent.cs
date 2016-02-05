﻿using System.Collections.Generic;
using SiliconStudio.Core;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Design;
using SiliconStudio.Core.Reflection;
using System.Reflection;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Xenko.Animations;
using SiliconStudio.Xenko.SpriteStudio.Runtime;
using SiliconStudio.Xenko.Updater;

namespace SiliconStudio.Xenko.Engine
{
    [DataContract("SpriteStudioComponent")]
    [Display("Sprite Studio", Expand = ExpandRule.Once)]
    [DefaultEntityComponentProcessor(typeof(SpriteStudioProcessor))]
    [DefaultEntityComponentRenderer(typeof(SpriteStudioRenderer))]
    [DataSerializerGlobal(null, typeof(List<SpriteStudioNodeState>))]
    [ComponentOrder(9900)]
    public sealed class SpriteStudioComponent : EntityComponent
    {
        [DataMember(1)]
        public SpriteStudioSheet Sheet { get; set; }

        [DataMemberIgnore, DataMemberUpdatable]
        public List<SpriteStudioNodeState> Nodes { get; } = new List<SpriteStudioNodeState>();

        [DataMemberIgnore]
        internal List<SpriteStudioNodeState> SortedNodes { get; } = new List<SpriteStudioNodeState>();
    }
}
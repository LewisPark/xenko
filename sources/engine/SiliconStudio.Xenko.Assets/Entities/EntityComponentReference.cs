﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;

using SiliconStudio.Core;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Xenko.Engine;

namespace SiliconStudio.Xenko.Assets.Entities
{
    [DataContract]
    [DataStyle(DataStyle.Compact)]
    [NonIdentifiable]
    public sealed class EntityComponentReference : IEntityComponentReference
    {
        // TODO: implement a serializer and pass these fields readonly (and their related properties)

        public EntityComponentReference()
        {
        }

        public EntityComponentReference(EntityComponent entityComponent)
        {
            this.Entity = new EntityReference() { Id = entityComponent.Entity.Id };
            this.Id = IdentifiableHelper.GetId(entityComponent);
            this.Value = entityComponent;
        }

        [DataMember(10)]
        public EntityReference Entity { get; set; }

        [DataMember(20)]
        public Guid Id { get; set; }

        [DataMemberIgnore]
        public EntityComponent Value { get; set; }

        [DataMemberIgnore]
        public Type ComponentType { get; set; }

        public static EntityComponentReference New(EntityComponent entityComponent)
        {
            return new EntityComponentReference(entityComponent);
        }
    }
}

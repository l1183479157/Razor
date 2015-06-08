// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Internal.Web.Utils;

namespace Microsoft.AspNet.Razor.TagHelpers
{
    /// <summary>
    /// An <see cref="IEqualityComparer{TagHelperUsageDescriptor}"/> used to check equality between
    /// two <see cref="TagHelperUsageDescriptor"/>s.
    /// </summary>
    public class TagHelperUsageDescriptorComparer : IEqualityComparer<TagHelperUsageDescriptor>
    {
        /// <summary>
        /// A default instance of the <see cref="TagHelperUsageDescriptorComparer"/>.
        /// </summary>
        public static readonly TagHelperUsageDescriptorComparer Default = new TagHelperUsageDescriptorComparer();

        /// <summary>
        /// Initializes a new <see cref="TagHelperUsageDescriptorComparer"/> instance.
        /// </summary>
        protected TagHelperUsageDescriptorComparer()
        {
        }

        /// <inheritdoc />
        /// <remarks>
        /// Determines equality based on <see cref="TagHelperUsageDescriptor.Summary"/> and
        /// <see cref="TagHelperUsageDescriptor.Remarks"/>
        /// </remarks>
        public bool Equals(TagHelperUsageDescriptor descriptorX, TagHelperUsageDescriptor descriptorY)
        {
            if (descriptorX == descriptorY)
            {
                return true;
            }

            return descriptorX != null &&
                descriptorY != null &&
                string.Equals(descriptorX.Summary, descriptorY.Summary, StringComparison.Ordinal) &&
                string.Equals(descriptorX.Remarks, descriptorY.Remarks, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public int GetHashCode(TagHelperUsageDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return 0;
            }

            return HashCodeCombiner
                .Start()
                .Add(descriptor.Summary, StringComparer.Ordinal)
                .Add(descriptor.Remarks, StringComparer.Ordinal)
                .CombinedHash;
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.Razor.Runtime.TagHelpers
{
    public class Valid_PlainTagHelper : TagHelper
    {
    }

    public class Valid_InheritedTagHelper : Valid_PlainTagHelper
    {
    }

    public class SingleAttributeTagHelper : TagHelper
    {
        public int IntAttribute { get; set; }
    }

    public class MissingAccessorTagHelper : TagHelper
    {
        public string ValidAttribute { get; set; }
        public string InvalidNoGetAttribute { set { } }
        public string InvalidNoSetAttribute { get { return string.Empty; } }
    }

    public class NonPublicAccessorTagHelper : TagHelper
    {
        public string ValidAttribute { get; set; }
        public string InvalidPrivateSetAttribute { get; private set; }
        public string InvalidPrivateGetAttribute { private get; set; }
        protected string InvalidProtectedAttribute { get; set; }
        internal string InvalidInternalAttribute { get; set; }
        protected internal string InvalidProtectedInternalAttribute { get; set; }
    }

    /// <summary>
    /// The summary for the type.
    /// </summary>
    /// <remarks>
    /// The remarks for the type.
    /// </remarks>
    public class DocumentedTagHelper : TagHelper
    {
        public static readonly string XmlTestFileLocation =
            Directory.GetCurrentDirectory() + "/TestFiles/TagHelperDocumentation.xml";

        /// <summary>
        /// The summary for a property.
        /// </summary>
        public string SummaryProperty { get; set; }

        /// <remarks>
        /// The remarks for a property.
        /// </remarks>
        public int RemarksProperty { get; set; }

        /// <summary>
        /// The summary for a property with remarks.
        /// </summary>
        /// <remarks>
        /// The remarks for a property with summary.
        /// </remarks>
        public IDictionary<string, bool> RemarksAndSummaryProperty { get; set; }

        public bool UndocumentedProperty { get; set; }
    }
}
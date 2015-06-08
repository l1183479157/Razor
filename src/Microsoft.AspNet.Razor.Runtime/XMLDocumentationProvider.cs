// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if !DNXCORE50
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Razor.Runtime
{
    /// <summary>
    /// Enables retrieval of summary and remarks XML documentation from an XML documentation file.
    /// </summary>
    public class XmlDocumentationProvider
    {
        private readonly IEnumerable<XElement> _members;

        /// <summary>
        /// Instantiates a new instance of the <see cref="XmlDocumentationProvider"/>.
        /// </summary>
        /// <param name="xmlFileLocation">Path to the XML documentation file to read.</param>
        public XmlDocumentationProvider(string xmlFileLocation)
        {
            // XML file processing is defined by: https://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx
            var xmlDocumentation = XDocument.Load(xmlFileLocation);
            var documentationRootMembers = xmlDocumentation.Root.Element("members");
            _members = documentationRootMembers.Elements("member");
        }

        /// <summary>
        /// Retrieves the <c>&lt;summary&gt;</c> documentation for the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id to lookup.</param>
        /// <returns><c>&lt;summary&gt;</c> documentation for the given <paramref name="id"/>.</returns>
        public string GetSummary(string id)
        {
            var associatedMemeber = GetMember(id);

            return associatedMemeber?.Element("summary")?.Value.Trim();
        }

        /// <summary>
        /// Retrieves the <c>&lt;remarks&gt;</c> documentation for the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id to lookup.</param>
        /// <returns><c>&lt;remarks&gt;</c> documentation for the given <paramref name="id"/>.</returns>
        public string GetRemarks(string id)
        {
            var associatedMemeber = GetMember(id);

            return associatedMemeber?.Element("remarks")?.Value.Trim();
        }

        /// <summary>
        /// Generates the <see cref="string"/> identifier for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the identifier for.</param>
        /// <returns>The <see cref="string"/> identifier for the given <paramref name="type"/>.</returns>
        public static string GetId([NotNull] Type type)
        {
            return $"T:{GetUnnestedFullName(type)}";
        }

        /// <summary>
        /// Generates the <see cref="string"/> identifier for the given <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to get the identifier for.</param>
        /// <returns>The <see cref="string"/> identifier for the given <paramref name="propertyInfo"/>.</returns>
        public static string GetId([NotNull] PropertyInfo propertyInfo)
        {
            var declaringTypeInfo = propertyInfo.DeclaringType;
            return $"P:{GetUnnestedFullName(declaringTypeInfo)}.{propertyInfo.Name}";
        }

        private XElement GetMember(string id)
        {
            var associatedMemeber = _members
                .FirstOrDefault(element =>
                    string.Equals(element.Attribute("name").Value, id, StringComparison.Ordinal));

            return associatedMemeber;
        }

        private static string GetUnnestedFullName(Type type)
        {
            var fullName = type.FullName;
            if (type.DeclaringType != null)
            {
                // Nested types have '+' to indicate nesting in their full name.
                fullName = fullName.Replace('+', '.');
            }

            return fullName;
        }
    }
}
#endif
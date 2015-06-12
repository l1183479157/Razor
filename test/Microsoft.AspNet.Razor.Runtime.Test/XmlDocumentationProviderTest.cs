// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if !DNXCORE50
using System;
using System.Reflection;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Xunit;

namespace Microsoft.AspNet.Razor.Runtime
{
    public class XmlDocumentationProviderTest
    {
        private static readonly TypeInfo DocumentedTagHelperTypeInfo = typeof(DocumentedTagHelper).GetTypeInfo();
        private static readonly PropertyInfo DocumentedTagHelperSummaryPropertyInfo =
            DocumentedTagHelperTypeInfo.GetProperty(nameof(DocumentedTagHelper.SummaryProperty));
        private static readonly PropertyInfo DocumentedTagHelperRemarksPropertyInfo =
            DocumentedTagHelperTypeInfo.GetProperty(nameof(DocumentedTagHelper.RemarksProperty));
        private static readonly PropertyInfo DocumentedTagHelperRemarksSummaryPropertyInfo =
            DocumentedTagHelperTypeInfo.GetProperty(nameof(DocumentedTagHelper.RemarksAndSummaryProperty));

        [Fact]
        public void CanReadXml()
        {
            // Act. Ensuring that reading the Xml file doesn't throw.
            var xmlDocumentationProvider = new XmlDocumentationProvider(DocumentedTagHelper.XmlTestFileLocation);
        }

        public static TheoryData GetSummaryData
        {
            get
            {
                // id, expectedSummary
                return new TheoryData<string, string>
                {
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperTypeInfo),
                        "The summary for the type."
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperSummaryPropertyInfo),
                        "The summary for a property."
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperRemarksPropertyInfo),
                        null
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperRemarksSummaryPropertyInfo),
                        "The summary for a property with remarks."
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetSummaryData))]
        public void GetSummary(string id, string expectedSummary)
        {
            // Arrange
            var xmlDocumentationProvider = new XmlDocumentationProvider(DocumentedTagHelper.XmlTestFileLocation);

            // Act
            var summary = xmlDocumentationProvider.GetSummary(id);

            // Assert
            Assert.Equal(expectedSummary, summary, StringComparer.Ordinal);
        }

        public static TheoryData GetRemarksData
        {
            get
            {
                // id, expectedRemarks
                return new TheoryData<string, string>
                {
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperTypeInfo),
                        "The remarks for the type."
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperSummaryPropertyInfo),
                        null
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperRemarksPropertyInfo),
                        "The remarks for a property."
                    },
                    {
                        XmlDocumentationProvider.GetId(DocumentedTagHelperRemarksSummaryPropertyInfo),
                        "The remarks for a property with summary."
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetRemarksData))]
        public void GetRemarks(string id, string expectedRemarks)
        {
            // Arrange
            var xmlDocumentationProvider = new XmlDocumentationProvider(DocumentedTagHelper.XmlTestFileLocation);

            // Act
            var remarks = xmlDocumentationProvider.GetRemarks(id);

            // Assert
            Assert.Equal(expectedRemarks, remarks, StringComparer.Ordinal);
        }

        [Fact]
        public void GetId_TypeInfo()
        {
            // Arrange
            var expectedId = "T:" + typeof(DocumentedTagHelper).FullName;

            // Act
            var id = XmlDocumentationProvider.GetId(DocumentedTagHelperTypeInfo);

            // Assert
            Assert.Equal(expectedId, id, StringComparer.Ordinal);
        }

        [Fact]
        public void GetId_PropertyInfo()
        {
            // Arrange
            var expectedId = $"P:{typeof(DocumentedTagHelper).FullName}." +
                nameof(DocumentedTagHelper.RemarksAndSummaryProperty);

            // Act
            var id = XmlDocumentationProvider.GetId(DocumentedTagHelperRemarksSummaryPropertyInfo);

            // Assert
            Assert.Equal(expectedId, id, StringComparer.Ordinal);
        }

        [Fact]
        public void GetId_TypeInfo_NestedClass()
        {
            // Arrange
            var typeInfo = typeof(NestedTestClass).GetTypeInfo();
            var expectedId = $"T:{typeof(XmlDocumentationProviderTest).FullName}.{nameof(NestedTestClass)}";

            // Act
            var id = XmlDocumentationProvider.GetId(typeInfo);

            // Assert
            Assert.Equal(expectedId, id, StringComparer.Ordinal);
        }

        [Fact]
        public void GetId_PropertyInfo_NestedClass()
        {
            // Arrange
            var propertyInfo = typeof(NestedTestClass).GetTypeInfo().GetProperty(nameof(NestedTestClass.TestProperty));
            var expectedId = $"P:{typeof(XmlDocumentationProviderTest).FullName}.{nameof(NestedTestClass)}." +
                nameof(NestedTestClass.TestProperty);

            // Act
            var id = XmlDocumentationProvider.GetId(propertyInfo);

            // Assert
            Assert.Equal(expectedId, id, StringComparer.Ordinal);
        }

        private class NestedTestClass
        {
            public int TestProperty { get; set; }
        }
    }
}
#endif
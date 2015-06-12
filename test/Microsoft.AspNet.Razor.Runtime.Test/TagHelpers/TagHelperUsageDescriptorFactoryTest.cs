// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if !DNXCORE50
using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.AspNet.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Razor.Runtime.TagHelpers
{
    public class TagHelperUsageDescriptorFactoryTest
    {
        // The test assembly doesn't really exist. This is used to look up corresponding XML for the fake assembly
        // which is based on the DocumentedTagHelper type.
        public static readonly string DocumentedTestAssemblyLocation =
            Path.ChangeExtension(DocumentedTagHelper.XmlTestFileLocation, ".dll");
        public static readonly string DocumentedTestAssemblyCodeBaseLocation =
            "file:///" + DocumentedTestAssemblyLocation;

        public static TheoryData CreateDescriptor_TypeData
        {
            get
            {
                var defaultLocation = DocumentedTestAssemblyLocation;
                var defaultCodeBase = DocumentedTestAssemblyCodeBaseLocation;
                var nonExistentLocation = defaultLocation.Replace("TestFiles", "TestFile");
                var nonExistentCodeBase = defaultCodeBase.Replace("TestFiles", "TestFile");
                var invalidLocation = defaultLocation + '\0';
                var invalidCodeBase = defaultCodeBase + '\0';

                // tagHelperType, expectedUsageDescriptor
                return new TheoryData<Type, TagHelperUsageDescriptor>
                {
                    { CreateDocumentationTagHelperTestType(location: null, codeBase: null), null },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, codeBase: null),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for the type.",
                            remarks: "The remarks for the type.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: defaultCodeBase),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for the type.",
                            remarks: "The remarks for the type.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, defaultCodeBase),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for the type.",
                            remarks: "The remarks for the type.")
                    },
                    { CreateTestType<SingleAttributeTagHelper>(defaultLocation, defaultCodeBase), null },
                    { CreateDocumentationTagHelperTestType(nonExistentLocation, codeBase: null), null },
                    { CreateDocumentationTagHelperTestType(location: null, codeBase: nonExistentCodeBase), null },
                    { CreateTestType<SingleAttributeTagHelper>(invalidLocation, codeBase: null), null },
                    { CreateDocumentationTagHelperTestType(location: null, codeBase: invalidCodeBase), null },
                };
            }
        }

        public static TheoryData CreateDescriptor_FallbackTypeData
        {
            get
            {
                // Faking localized dlls only to force the TagHelperUsageDescriptorFactory to search for localized XML.
                var defaultENGBLocation = DocumentedTestAssemblyLocation.Replace(".dll", "-en-GB.dll");
                var defaultENLocation = DocumentedTestAssemblyLocation.Replace(".dll", "-en.dll");

                // tagHelperType, expectedUsageDescriptor
                return new TheoryData<Type, TagHelperUsageDescriptor>
                {
                    {
                        CreateDocumentationTagHelperTestType(defaultENGBLocation, codeBase: null),
                        new TagHelperUsageDescriptor(
                            summary: "en-GB: The summary for the type.",
                            remarks: "en-GB: The remarks for the type.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENLocation, codeBase: null),
                        new TagHelperUsageDescriptor(
                            summary: "en: The summary for the type.",
                            remarks: "en: The remarks for the type.")
                    }
                };
            }
        }

        [Theory]
        [ReplaceCulture]
        [MemberData(nameof(CreateDescriptor_TypeData))]
        [MemberData(nameof(CreateDescriptor_FallbackTypeData))]
        public void CreateDescriptor_Type(Type tagHelperType, TagHelperUsageDescriptor expectedUsageDescriptor)
        {
            // Act
            var usageDescriptor = TagHelperUsageDescriptorFactory.CreateDescriptor(tagHelperType);

            // Assert
            Assert.Equal(expectedUsageDescriptor, usageDescriptor, TagHelperUsageDescriptorComparer.Default);
        }

        public static TheoryData CreateDescriptor_PropertyData
        {
            get
            {
                var defaultLocation = DocumentedTestAssemblyLocation;
                var defaultCodeBase = DocumentedTestAssemblyCodeBaseLocation;
                var nonExistentLocation = defaultLocation.Replace("TestFiles", "TestFile");
                var nonExistentCodeBase = defaultCodeBase.Replace("TestFiles", "TestFile");
                var invalidLocation = defaultLocation + '\0';
                var invalidCodeBase = defaultCodeBase + '\0';

                // tagHelperType, propertyName, expectedUsageDescriptor
                return new TheoryData<Type, string, TagHelperUsageDescriptor>
                {
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: null),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, defaultCodeBase),
                        nameof(DocumentedTagHelper.UndocumentedProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, codeBase: null),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        new TagHelperUsageDescriptor(summary: "The summary for a property.", remarks: null)
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        new TagHelperUsageDescriptor(summary: null, remarks: "The remarks for a property.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for a property with remarks.",
                            remarks: "The remarks for a property with summary.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: defaultCodeBase),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        new TagHelperUsageDescriptor(summary: "The summary for a property.", remarks: null)
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: defaultCodeBase),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        new TagHelperUsageDescriptor(summary: null, remarks: "The remarks for a property.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: defaultCodeBase),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for a property with remarks.",
                            remarks: "The remarks for a property with summary.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, defaultCodeBase),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        new TagHelperUsageDescriptor(summary: "The summary for a property.", remarks: null)
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, defaultCodeBase),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        new TagHelperUsageDescriptor(summary: null, remarks: "The remarks for a property.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultLocation, defaultCodeBase),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        new TagHelperUsageDescriptor(
                            summary: "The summary for a property with remarks.",
                            remarks: "The remarks for a property with summary.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(nonExistentLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: nonExistentCodeBase),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(invalidLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        null
                    },
                    {
                        CreateDocumentationTagHelperTestType(location: null, codeBase: invalidCodeBase),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        null
                    }
                };
            }
        }

        public static TheoryData CreateDescriptor_FallbackPropertyData
        {
            get
            {
                // Faking localized dlls only to force the TagHelperUsageDescriptorFactory to search for localized XML.
                var defaultENGBLocation = DocumentedTestAssemblyLocation.Replace(".dll", "-en-GB.dll");
                var defaultENLocation = DocumentedTestAssemblyLocation.Replace(".dll", "-en.dll");

                // tagHelperType, propertyName, expectedUsageDescriptor
                return new TheoryData<Type, string, TagHelperUsageDescriptor>
                {
                    {
                        CreateDocumentationTagHelperTestType(defaultENGBLocation, codeBase: null),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        new TagHelperUsageDescriptor(summary: "en-GB: The summary for a property.", remarks: null)
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENGBLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        new TagHelperUsageDescriptor(summary: null, remarks: "en-GB: The remarks for a property.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENGBLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        new TagHelperUsageDescriptor(
                            summary: "en-GB: The summary for a property with remarks.",
                            remarks: "en-GB: The remarks for a property with summary.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENLocation, codeBase: null),
                        nameof(DocumentedTagHelper.SummaryProperty),
                        new TagHelperUsageDescriptor(summary: "en: The summary for a property.", remarks: null)
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksProperty),
                        new TagHelperUsageDescriptor(summary: null, remarks: "en: The remarks for a property.")
                    },
                    {
                        CreateDocumentationTagHelperTestType(defaultENLocation, codeBase: null),
                        nameof(DocumentedTagHelper.RemarksAndSummaryProperty),
                        new TagHelperUsageDescriptor(
                            summary: "en: The summary for a property with remarks.",
                            remarks: "en: The remarks for a property with summary.")
                    },
                };
            }
        }

        [Theory]
        [ReplaceCulture]
        [MemberData(nameof(CreateDescriptor_PropertyData))]
        [MemberData(nameof(CreateDescriptor_FallbackPropertyData))]
        public void CreateDescriptor_Property(
            Type tagHelperType,
            string propertyName,
            TagHelperUsageDescriptor expectedUsageDescriptor)
        {
            // Arrange
            var mockPropertyInfo = new Mock<PropertyInfo>();
            mockPropertyInfo.Setup(propertyInfo => propertyInfo.DeclaringType).Returns(tagHelperType);
            mockPropertyInfo.Setup(propertyInfo => propertyInfo.Name).Returns(propertyName);

            // Act
            var usageDescriptor = TagHelperUsageDescriptorFactory.CreateDescriptor(mockPropertyInfo.Object);

            // Assert
            Assert.Equal(expectedUsageDescriptor, usageDescriptor, TagHelperUsageDescriptorComparer.Default);
        }

        private static Type CreateDocumentationTagHelperTestType(string location, string codeBase)
        {
            return CreateTestType<DocumentedTagHelper>(location, codeBase);
        }

        private static Type CreateTestType<TWrappedType>(string location, string codeBase)
        {
            var wrappedType = typeof(TWrappedType);
            var testAssembly = new TestAssembly(location, codeBase);
            var mockType = new Mock<Type>();
            mockType.Setup(type => type.Assembly).Returns(testAssembly);
            mockType.Setup(type => type.FullName).Returns(wrappedType.FullName);
            mockType.Setup(type => type.DeclaringType).Returns(wrappedType.DeclaringType);

            return mockType.Object;
        }

        private class TestAssembly : Assembly
        {
            public TestAssembly(string location, string codeBase)
            {
                Location = location;
                CodeBase = codeBase;
            }

            public override string Location { get; }

            public override string CodeBase { get; }
        }
    }
}
#endif
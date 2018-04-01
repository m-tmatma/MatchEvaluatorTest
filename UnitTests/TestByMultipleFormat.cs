﻿//-----------------------------------------------------------------------
// <copyright file="TestByMultipleFormat.cs" company="Masaru Tsuchiyama">
//     Copyright (c) Masaru Tsuchiyama. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace ReplaceGuid
{
    using System;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// unit test for ReplaceWithNewGuid
    /// </summary>
    [TestFixture]
    public class TestByMultipleFormat
    {
        /// <summary>
        /// GUID generator class to make unit-testing easier
        /// </summary>
        private GuidGenerater guidGenerator;

        /// <summary>
        /// enum of GUID Format
        /// </summary>
        /// <see href="https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx">Guid.ToString Method (String)</see>
        private enum Format
        {
            /// <summary>
            /// enum definition for hyphen separated GUID
            /// "D": 00000000-0000-0000-0000-000000000000
            /// </summary>
            TypeD,

            /// <summary>
            /// enum definition for hyphen separated GUID
            /// "B": {00000000-0000-0000-0000-000000000000}
            /// </summary>
            TypeB,

            /// <summary>
            /// enum definition for hyphen separated GUID
            /// "P": (00000000-0000-0000-0000-000000000000)
            /// </summary>
            TypeP,

            /// <summary>
            /// enum definition for raw 32-digit GUID
            /// "N": 00000000000000000000000000000000
            /// </summary>
            TypeN,

            /// <summary>
            /// enum definition for GUID structure
            /// "X": {0x00000000, 0x0000, 0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
            /// </summary>
            TypeX,

            /// <summary>
            /// enum definition for DEFINE_GUID
            /// DEFINE_GUID(&lt;&lt;name&gt;&gt;, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
            /// </summary>
            TypeDEFINE_GUID,

            /// <summary>
            /// enum definition for IMPLEMENT_OLECREATE
            /// IMPLEMENT_OLECREATE(&lt;&lt;class&gt;&gt;, &lt;&lt;external_name&gt;&gt;, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
            /// </summary>
            TypeOLECREATE,
        }

        /// <summary>
        /// setup method to be called to initialize unit tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.guidGenerator = new GuidGenerater();
        }

        /// <summary>
        /// cleanup method to be called to cleanup unit tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.guidGenerator = null;
        }

        /// <summary>
        /// delegate for formatting GUID string
        /// </summary>
        /// <param name="guid">GUID to be formatted</param>
        /// <param name="destFormat">format type</param>
        delegate void FormatGuid(StringBuilder builder, Guid guid, Format destFormat);

        /// <summary>
        /// multiple GUID pattern test.
        /// source GUIDs are generated by Guid.NewGuid()
        /// </summary>
        /// <param name="count">loop count</param>
        /// <see href="https://github.com/nunit/docs/wiki/TestCase-Attribute">TestCase Attribute</see>
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(50)]
        [TestCase(100)]
        public void TestGuidRandomSourceByRandomFormat(int count)
        {
            var builderInput = new StringBuilder();
            var builderResult = new StringBuilder();

            Array values = Enum.GetValues(typeof(Format));
            var random = new Random();

            // GUID generator for source data
            var srcGuidGenerator = new GuidGenerater();

            // create source data
            for (int i = 0; i < count; i++)
            {
                // choose enum 'Format' randomly
                var format = (Format)values.GetValue(random.Next(values.Length));

                var separator = string.Format("------ {0} ------", i);
                Console.WriteLine(i.ToString() + ": " + format.ToString());

                var srcGuid = Guid.NewGuid();
                var dstGuid = srcGuidGenerator.NewGuid();

                FormatGuid formatGuid = delegate(StringBuilder builder, Guid guid, Format destFormat)
                {
                    builder.Append(separator);
                    builder.Append(Environment.NewLine);
                    builder.Append(FormatGuidString(guid, destFormat));
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                };

                // create input data
                formatGuid(builderInput, srcGuid, format);

                // create expected data
                formatGuid(builderResult, dstGuid, format);
            }

            var input = builderInput.ToString();
            var expected = builderResult.ToString();

            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(input);

            Console.WriteLine("input: ");
            Console.WriteLine(input);

            Console.WriteLine("output: ");
            Console.WriteLine(output);

            Console.WriteLine("expected: ");
            Console.WriteLine(expected);

            Assert.That(output, Is.EqualTo(expected));
        }

        /// <summary>
        /// return GUID as string
        /// </summary>
        /// <param name="guid">a GUID to be formated</param>
        /// <param name="index">formt index</param>
        /// <returns>formatted GUID string</returns>
        /// <see href="https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx">Guid.ToString Method (String)</see>
        private static string FormatGuidString(Guid guid, Format format)
        {
            switch (format)
            {
                case Format.TypeN:
                    return guid.ToString("N");
                case Format.TypeD:
                    return guid.ToString("D");
                case Format.TypeB:
                    return guid.ToString("B");
                case Format.TypeP:
                    return guid.ToString("P");
                case Format.TypeX:
                    return guid.ToString("X");
                case Format.TypeOLECREATE:
                    return FormatGuidAsImplementOleCreate(guid);
                case Format.TypeDEFINE_GUID:
                    return FormatGuidAsDefineGuid(guid);
            }
            return string.Empty;
        }

        /// <summary>
        /// IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x00000000,0x0000,0x0000,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
        /// </summary>
        /// <param name="guid">a GUID to be formated</param>
        /// <param name="defineGuid">IMPLEMENT_OLECREATE tag</param>
        /// <param name="variableName1">class tag</param>
        /// <param name="variableName2">external_name tag</param>
        /// <returns>formatted GUID string</returns>
        private static string FormatGuidAsImplementOleCreate(Guid guid, string macroName = "IMPLEMENT_OLECREATE", string variableName1 = "class", string variableName2 = "external_name")
        {
            var builder = new StringBuilder();
            builder.Append(macroName);
            builder.Append("(");
            builder.Append(variableName1);
            builder.Append(",");
            builder.Append(variableName2);
            builder.Append(",");
            builder.Append(FormatGuidAsRawValues(guid));
            builder.Append(");");
            return builder.ToString();
        }

        /// <summary>
        /// DEFINE_GUID(name, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
        /// </summary>
        /// <param name="guid">a GUID to be formated</param>
        /// <param name="macroName">DEFINE_GUID tag</param>
        /// <param name="variableName">variable tag</param>
        /// <returns>formatted GUID string</returns>
        private static string FormatGuidAsDefineGuid(Guid guid, string macroName = "DEFINE_GUID", string variableName = "name")
        {
            var builder = new StringBuilder();
            builder.Append(macroName);
            builder.Append("(");
            builder.Append(variableName);
            builder.Append(",");
            builder.Append(FormatGuidAsRawValues(guid));
            builder.Append(");");
            return builder.ToString();
        }

        /// <summary>
        /// 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private static string FormatGuidAsRawValues(Guid guid)
        {
            var bytes = guid.ToByteArray();
            var builder = new StringBuilder();
            int i = 0;

            int guid_1st = ((int)bytes[3] << 24) | ((int)bytes[2] << 16) | ((int)bytes[1] << 8) | bytes[0];
            short guid_2nd = (short)(((int)bytes[5] << 8) | bytes[4]);
            short guid_3rd = (short)(((int)bytes[7] << 8) | bytes[6]);

            // 0x00000000
            builder.Append("0x");
            builder.Append(guid_1st.ToString("x8"));
            builder.Append(", ");

            // 0x0000
            builder.Append("0x");
            builder.Append(guid_2nd.ToString("x4"));
            builder.Append(", ");

            // 0x0000
            builder.Append("0x");
            builder.Append(guid_3rd.ToString("x4"));
            builder.Append(", ");

            // 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            for (i = 0; i < 8; i++)
            {
                builder.Append("0x");
                builder.Append(bytes[8 + i].ToString("x2"));
                if (i != 7)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="UnitTests.cs" company="Masaru Tsuchiyama">
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
    public class UnitTests
    {
        /// <summary>
        /// input data for test1
        /// </summary>
        private const string Input1 = @"[Guid(""00000000-0000-0000-0000-000000000000"")]";

        /// <summary>
        /// expected output data for test1
        /// </summary>
        private const string Result1 = @"[Guid(""01020304-0506-0708-090a-0b0c0d0e0f10"")]";

        /// <summary>
        /// input data for test2
        /// </summary>
        private const string Input2 = @"""00000000-0000-0000-0000-000000000000""";

        /// <summary>
        /// expected output data for test2
        /// </summary>
        private const string Result2 = @"""01020304-0506-0708-090a-0b0c0d0e0f10""";

        /// <summary>
        /// input data for test3
        /// </summary>
        private const string Input3 = @"{00000000-0000-0000-0000-000000000000}";

        /// <summary>
        /// expected output data for test3
        /// </summary>
        private const string Result3 = @"{01020304-0506-0708-090a-0b0c0d0e0f10}";

        /// <summary>
        /// GUID generator class to make unit-testing easier
        /// </summary>
        private GuidGenerater guidGenerator;

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
        /// unit test method
        /// </summary>
        [Test]
        public void TestGuidWithBrackets()
        {
            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(Input1);
            Assert.That(output, Is.EqualTo(Result1));

            var output2 = replaceWithGuid.ReplaceSameGuidToSameGuid(Input2);
            Assert.That(output2, Is.EqualTo(Result2));

            var output3 = replaceWithGuid.ReplaceSameGuidToSameGuid(Input3);
            Assert.That(output3, Is.EqualTo(Result3));
        }
    }
}

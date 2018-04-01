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

    [TestFixture]
    public class UnitTests
    {
        const string input1 = @"[Guid(""00000000-0000-0000-0000-000000000000"")]";
        const string result1 = @"[Guid(""01020304-0506-0708-090a-0b0c0d0e0f10"")]";
        const string input2 = @"""00000000-0000-0000-0000-000000000000""";
        const string result2 = @"""01020304-0506-0708-090a-0b0c0d0e0f10""";
        const string input3 = @"{00000000-0000-0000-0000-000000000000}";
        const string result3 = @"{01020304-0506-0708-090a-0b0c0d0e0f10}";
        private GuidGenerater guidGenerator;

        [SetUp]
        public void SetUp()
        {
            this.guidGenerator = new GuidGenerater();
        }

        [TearDown]
        public void TearDown()
        {
            this.guidGenerator = null;
        }

        [Test]
        public void TestGuidWithBrackets()
        {
            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(input1);
            Assert.That(output, Is.EqualTo(result1));

            var output2 = replaceWithGuid.ReplaceSameGuidToSameGuid(input2);
            Assert.That(output2, Is.EqualTo(result2));

            var output3 = replaceWithGuid.ReplaceSameGuidToSameGuid(input3);
            Assert.That(output3, Is.EqualTo(result3));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ReplaceGuid
{
    /// <summary>
    /// Tester Class for providing custom GUID generator 
    /// </summary>
    internal class GuidGenerater
    {
        private int Counter;

        public GuidGenerater()
        {
            this.Counter = 0;
        }

        /// <summary>
        /// delegate for ReplaceSameGuidToSameGuid
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Guid NewGuid()
        {
            this.Counter++;

            var builder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                byte ch = (byte)((i + this.Counter) & 0xff);
                builder.Append(ch.ToString("x2"));
            }

            var guidString = builder.ToString();
            Console.WriteLine(guidString);
            return new Guid(guidString);
        }
    }

    [TestFixture]
    public class UnitTests
    {
        const string input1 = @"[Guid(""00000000-0000-0000-0000-000000000000"")]";
        const string result1 = @"[Guid(""01020304-0506-0708-090a-0b0c0d0e0f10"")]";
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
        public void TestGuidWithBraces()
        {
            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(input1);
            Assert.That(output, Is.EqualTo(result1));
        }
    }
}

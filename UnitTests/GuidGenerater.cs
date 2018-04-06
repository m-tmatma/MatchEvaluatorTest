//-----------------------------------------------------------------------
// <copyright file="GuidGenerater.cs" company="Masaru Tsuchiyama">
//     Copyright (c) Masaru Tsuchiyama. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Unittest
{
    using System;
    using System.Text;

    /// <summary>
    /// Tester Class for providing custom GUID generator 
    /// </summary>
    internal class GuidGenerater
    {
        /// <summary>
        /// seed to create a new GUID.
        /// </summary>
        private int counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidGenerater" /> class.
        /// </summary>
        public GuidGenerater()
        {
            this.counter = 0;
        }

        /// <summary>
        /// delegate for ReplaceSameGuidToSameGuid
        /// </summary>
        /// <returns>new GUID</returns>
        public Guid NewGuid()
        {
            this.counter++;

            var builder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                byte ch = (byte)((i + this.counter) & 0xff);
                builder.Append(ch.ToString("x2"));
            }

            var guidString = builder.ToString();
            //Console.WriteLine(guidString);
            return new Guid(guidString);
        }
    }
}

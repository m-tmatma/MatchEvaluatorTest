//-----------------------------------------------------------------------
// <copyright file="GuidGenerater.cs" company="Masaru Tsuchiyama">
//     Copyright (c) Masaru Tsuchiyama. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace ReplaceGuid
{
    using System;
    using System.Text;
    using NUnit.Framework;

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
}

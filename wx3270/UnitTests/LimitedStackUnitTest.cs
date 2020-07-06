// <copyright file="LimitedStackUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="LimitedStack{T}"/> class.
    /// </summary>
    [TestClass]
    public class LimitedStackUnitTest
    {
        /// <summary>
        /// Test basic success cases.
        /// </summary>
        [TestMethod]
        public void LimitedStackSuccess()
        {
            var s = new LimitedStack<int>(3);

            // Push 4 elements on the stack.
            s.Push(1);
            s.Push(2);
            s.Push(3);
            s.Push(4);

            // The count should stay at 3.
            Assert.AreEqual(3, s.Count);

            // Only the last three elements should be there.
            Assert.AreEqual(4, s.Pop());
            Assert.AreEqual(3, s.Pop());
            Assert.AreEqual(2, s.Pop());
        }

        /// <summary>
        /// Test constructor exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LimitedStackException1()
        {
            var s = new LimitedStack<int>(0);
        }

        /// <summary>
        /// Test constructor exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LimitedStackException2()
        {
            var s = new LimitedStack<int>(-9);
        }
    }
}

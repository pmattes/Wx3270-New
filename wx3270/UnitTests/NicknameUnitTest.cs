// <copyright file="NicknameUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="Nickname"/> class.
    /// </summary>
    [TestClass]
    public class NicknameUnitTest
    {
        /// <summary>
        /// Test the <see cref="Nickname.ValidNickname"/> method.
        /// </summary>
        [TestMethod]
        public void ValidNickname()
        {
            // Bad names.
            Assert.IsFalse(Nickname.ValidNickname("con"));
            Assert.IsFalse(Nickname.ValidNickname("pRn"));
            Assert.IsFalse(Nickname.ValidNickname("AUX"));
            Assert.IsFalse(Nickname.ValidNickname("nul"));
            Assert.IsFalse(Nickname.ValidNickname("com3"));
            Assert.IsFalse(Nickname.ValidNickname("lpt7"));
            Assert.IsFalse(Nickname.ValidNickname("a\b"));
            Assert.IsFalse(Nickname.ValidNickname("a/b"));
            Assert.IsFalse(Nickname.ValidNickname("<b"));
            Assert.IsFalse(Nickname.ValidNickname(">b"));
            Assert.IsFalse(Nickname.ValidNickname("C:"));
            Assert.IsFalse(Nickname.ValidNickname("\"a"));
            Assert.IsFalse(Nickname.ValidNickname("able|baker"));
            Assert.IsFalse(Nickname.ValidNickname("zoo*"));
            Assert.IsFalse(Nickname.ValidNickname("zoo?"));
            Assert.IsFalse(Nickname.ValidNickname("foo\0xxx"));
            Assert.IsFalse(Nickname.ValidNickname("foo\u001Fxxx"));

            // Good names that might look bad.
            Assert.IsTrue(Nickname.ValidNickname("con1"));
            Assert.IsTrue(Nickname.ValidNickname("com"));
            Assert.IsTrue(Nickname.ValidNickname("a b"));
        }

        // TODO: Test CreateNickname.
    }
}

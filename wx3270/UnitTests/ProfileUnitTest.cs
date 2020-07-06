// <copyright file="ProfileUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="Nickname"/> class.
    /// </summary>
    [TestClass]
    public class ProfileUnitTest
    {
        /// <summary>
        /// Test the <see cref="Nickname.ValidNickname"/> method.
        /// </summary>
        [TestMethod]
        public void ProfileEqualsTest()
        {
            var profile1 = new Profile();
            var profile2 = new Profile();
            Assert.IsTrue(profile1.Equals(profile2));

            profile1.Version.Major = 999;
            Assert.IsFalse(profile1.Equals(profile2));
        }
    }
}

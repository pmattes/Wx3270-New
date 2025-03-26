// <copyright file="FakeRegistryUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="FakeRegistry"/> class.
    /// </summary>
    [TestClass]
    public class FakeRegistryUnitTest
    {
        /// <summary>
        /// Test initialization.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // Wipe out the database from any previous run.
            File.Delete(FakeRegistry.GetPath("."));
        }

        /// <summary>
        /// Test basic success.
        /// </summary>
        [TestMethod]
        public void FakeRegistrySuccess()
        {
            const string subKeyName = "foo";
            const string valueName = "fred";
            const string value = "some value";

            var r = SimplifiedRegistryFactory.Get(fake: true);
            using (var key = r.CurrentUserCreateSubKey(subKeyName, "."))
            {
                var gotValue = key.GetValue(valueName);
                Assert.IsNull(gotValue);

                key.SetValue(valueName, value);
                gotValue = key.GetValue(valueName);
                Assert.IsNotNull(gotValue);
                Assert.IsInstanceOfType(gotValue, typeof(string));
                Assert.AreEqual(value, (string)gotValue);
            }
        }

        /// <summary>
        /// Write the same key twice, with different values.
        /// </summary>
        [TestMethod]
        public void FakeRegistryChange()
        {
            const string subKeyName = "foo";
            const string valueName = "fred";
            const string value = "some value";
            const string value2 = "some other value";

            var r = SimplifiedRegistryFactory.Get(fake: true);
            using (var key = r.CurrentUserCreateSubKey(subKeyName, "."))
            {
                key.SetValue(valueName, value);
                key.SetValue(valueName, value2);
                var gotValue = key.GetValue(valueName);
                Assert.IsNotNull(gotValue);
                Assert.IsInstanceOfType(gotValue, typeof(string));
                Assert.AreEqual(value2, (string)gotValue);
            }
        }

        /// <summary>
        /// Write multiple values to multiple keys.
        /// </summary>
        [TestMethod]
        public void FakeRegistryMultiple()
        {
            const string subKeyName1 = "foo1";
            const string subKeyName2 = "foo2";
            const string valueName1 = "fred1";
            const string valueName2 = "fred2";
            const string value1 = "some value";
            const string value2 = "some other value";
            var names = new[] { subKeyName1, subKeyName2 };

            // Write them out.
            foreach (var name in names)
            {
                var r = SimplifiedRegistryFactory.Get(fake: true);
                using (var key = r.CurrentUserCreateSubKey(name, "."))
                {
                    key.SetValue(valueName1, name + " " + value1);
                    key.SetValue(valueName2, name + " " + value2);
                }
            }

            // Read them back.
            foreach (var name in names)
            {
                var r = SimplifiedRegistryFactory.Get(fake: true);
                using (var key = r.CurrentUserCreateSubKey(name, "."))
                {
                    var gotValue1 = key.GetValue(valueName1);
                    Assert.IsNotNull(gotValue1);
                    Assert.IsInstanceOfType(gotValue1, typeof(string));
                    Assert.AreEqual(name + " " + value1, (string)gotValue1);

                    var gotValue2 = key.GetValue(valueName2);
                    Assert.IsNotNull(gotValue2);
                    Assert.IsInstanceOfType(gotValue2, typeof(string));
                    Assert.AreEqual(name + " " + value2, (string)gotValue2);

                    // Make sure GetValueNames works.
                    var valueNames = key.GetValueNames();
                    Assert.AreEqual(2, valueNames.Count());
                    var expected = new[] { valueName1, valueName2 };
                    Assert.IsTrue(expected.All(v => valueNames.Contains(v)));
                }
            }
        }

        /// <summary>
        /// Tests DeleteValue.
        /// </summary>
        [TestMethod]
        public void FakeRegistryDeleteValue()
        {
            const string subKeyName = "foo";
            const string valueName = "fred";
            const string value = "some value";

            var r = SimplifiedRegistryFactory.Get(fake: true);
            using (var key = r.CurrentUserCreateSubKey(subKeyName, "."))
            {
                var gotValue = key.GetValue(valueName);
                Assert.IsNull(gotValue);

                key.SetValue(valueName, value);
                gotValue = key.GetValue(valueName);
                Assert.IsNotNull(gotValue);
                Assert.IsInstanceOfType(gotValue, typeof(string));
                Assert.AreEqual(value, (string)gotValue);

                key.DeleteValue(valueName);
                gotValue = key.GetValue(valueName);
                Assert.IsNull(gotValue);
            }
        }
    }
}

// <copyright file="ImportUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Wx3270.Contracts;

    /// <summary>
    /// Unit test for the <see cref="Wc3270Import"/> class.
    /// </summary>
    [TestClass]
    public class ImportUnitTest
    {
        /// <summary>
        /// Test the <see cref="Wc3270Import.Parse"/> method.
        /// </summary>
        [TestMethod]
        public void ParseSucceed()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse("wc3270.foo: bar");
        }

        /// <summary>
        /// Test the <see cref="Wc3270Import.Parse"/> method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ParseFail()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse("foo");
        }

        /// <summary>
        /// Trivial test of the <see cref="Wc3270Import.Digest"/> method.
        /// </summary>
        [TestMethod]
        public void DigestTrivial()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse("wc3270.model: 5");
            var profile = import.Digest();
            Assert.AreEqual(5, profile.Model);
        }

        /// <summary>
        /// Test <see cref="Wc3270Import.Digest"/> to make sure that only the last mention of a
        /// resource is validated.
        /// </summary>
        [TestMethod]
        public void DigestLastValidate()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse("wc3270.model: blatz");
            import.Parse("wc3270.model: 5");
            var profile = import.Digest();
            Assert.AreEqual(5, profile.Model);
        }

        /// <summary>
        /// Test <see cref="Wc3270Import.Digest"/> to make sure that only the last mention of a
        /// resource is consumed.
        /// </summary>
        [TestMethod]
        public void DigestLastConsume()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse("wc3270.model: 2");
            import.Parse("wc3270.model: 5");
            import.Parse("wc3270.model: 3");
            var profile = import.Digest();
            Assert.AreEqual(3, profile.Model);
        }

        /// <summary>
        /// Tests individual attributes.
        /// </summary>
        [TestMethod]
        public void TestSingles()
        {
            var tests = new Tuple<string, string, Action<Profile>>[]
            {
                new Tuple<string, string, Action<Profile>>(B3270.Setting.AltCursor, "true", (profile) => Assert.AreEqual(CursorType.Underscore, profile.CursorType, B3270.Setting.AltCursor)),
                new Tuple<string, string, Action<Profile>>(B3270.Setting.AlwaysInsert, "true", (profile) => Assert.AreEqual(true, profile.AlwaysInsert, B3270.Setting.AlwaysInsert)),
                new Tuple<string, string, Action<Profile>>(Wc3270.Resource.BellMode, "none", (profile) => Assert.AreEqual(false, profile.AudibleBell, Wc3270.Resource.BellMode)),
                new Tuple<string, string, Action<Profile>>(Wc3270.Resource.BellMode, "beep", (profile) => Assert.AreEqual(true, profile.AudibleBell, Wc3270.Resource.BellMode)),
                new Tuple<string, string, Action<Profile>>(Wc3270.Resource.Charset, "page1", (profile) => Assert.AreEqual("page1", profile.HostCodePage, Wc3270.Resource.Charset)),
                new Tuple<string, string, Action<Profile>>(B3270.Setting.CodePage, "page1", (profile) => Assert.AreEqual("page1", profile.HostCodePage, B3270.Setting.CodePage)),
                new Tuple<string, string, Action<Profile>>(B3270.Setting.Crosshair, "true", (profile) => Assert.AreEqual(true, profile.CrosshairCursor, B3270.Setting.Crosshair)),
                new Tuple<string, string, Action<Profile>>(
                    Wc3270.Resource.Macros,
                    "a: Foo()\\nb: Bar()",
                    (profile) => Assert.IsTrue(new[] { new MacroEntry { Name = "a", Macro = "Foo()" }, new MacroEntry { Name = "b", Macro = "Bar()" } }.SequenceEqual(profile.Macros), Wc3270.Resource.Macros)),
                new Tuple<string, string, Action<Profile>>(Wc3270.Resource.PrinterCodepage, "123", (profile) => Assert.AreEqual("123", profile.PrinterCodePage, Wc3270.Resource.PrinterCodepage)),
                new Tuple<string, string, Action<Profile>>(B3270.Setting.PrinterName, "fred", (profile) => Assert.AreEqual("fred", profile.Printer, B3270.Setting.PrinterName)),
                new Tuple<string, string, Action<Profile>>(Wc3270.Resource.Title, "hello", (profile) => Assert.AreEqual("hello", profile.WindowTitle, Wc3270.Resource.Title)),
            };

            var import = new Wc3270Import(new FakeBackEndDb());
            foreach (var test in tests)
            {
                import.Parse(B3270.ResourceFormat.Value(test.Item1, test.Item2));
                var profile = import.Digest();
                test.Item3(profile);
            }
        }

        /// <summary>
        /// Tests reverse video.
        /// </summary>
        [TestMethod]
        public void TestReverseVideo()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(Wc3270.Resource.ConsoleColorForHostColorNeutralBlack, "15"));
            import.Parse(B3270.ResourceFormat.Value(Wc3270.Resource.ConsoleColorForHostColorNeutralWhite, "0"));
            var profile = import.Digest();
            Assert.AreEqual(Settings.BlackOnWhiteScheme, profile.Colors.HostColors);
        }

        /// <summary>
        /// Tests the model number.
        /// </summary>
        [TestMethod]
        public void TestModel()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.Model, "3278-3"));
            var profile = import.Digest();
            Assert.AreEqual(3, profile.Model);
            Assert.AreEqual(false, profile.ColorMode);
            Assert.AreEqual(true, profile.ExtendedMode);

            import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.Model, "5"));
            profile = import.Digest();
            Assert.AreEqual(5, profile.Model);
            Assert.AreEqual(true, profile.ColorMode);
            Assert.AreEqual(true, profile.ExtendedMode);
        }

        /// <summary>
        /// Tests oversize.
        /// </summary>
        [TestMethod]
        public void TestOversize()
        {
            // Copied properly.
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.Oversize, "100x120"));
            var profile = import.Digest();
            Assert.IsTrue(new Profile.OversizeClass { Columns = 100, Rows = 120 }.Equals(profile.Oversize));

            // Too small.
            import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.Oversize, "10x10"));
            profile = import.Digest();
            Assert.IsTrue(new Profile.OversizeClass().Equals(profile.Oversize));
        }

        /// <summary>
        /// Tests PrinterLu and VerifyHostCert, and the logic that always processes the hostname first.
        /// </summary>
        [TestMethod]
        public void TestPrinterLuAndOrdering()
        {
            // Digest resources with PrinterLu before Hostname.
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.PrinterLu, "fred"));
            import.Parse(B3270.ResourceFormat.Value(B3270.Setting.VerifyHostCert, "false"));
            import.Parse(B3270.ResourceFormat.Value(Wc3270.Resource.Hostname, "localhost:2001"));
            var profile = import.Digest();

            // Verify that PrinterLu and VerifyCert were applied to the profile.
            Assert.AreEqual(1, profile.Hosts.Count());
            var host = profile.Hosts.First();
            Assert.AreEqual("localhost", host.Host);
            Assert.AreEqual("fred", host.PrinterSessionLu);
            Assert.IsTrue(host.Prefixes.Contains(B3270.Prefix.NoVerifyCert));
        }

        /// <summary>
        /// Tests unsupported resources.
        /// </summary>
        [TestMethod]
        public void TestUnsupported()
        {
            var import = new Wc3270Import(new FakeBackEndDb());
            import.Parse(B3270.ResourceFormat.Value("foo", "bar"));
            import.Parse(B3270.ResourceFormat.Value("baz", "biff"));
            _ = import.Digest(out _, out IEnumerable<string> unmatched, fromFile: false);
            var expectedUnmatched = new[] { "foo", "baz" };
            Assert.AreEqual(expectedUnmatched.Count(), unmatched.Count());
            Assert.IsTrue(expectedUnmatched.All(s => unmatched.Contains(s)));
        }

        /// <summary>
        ///  Fake code page database class.
        /// </summary>
        private class FakeCodePageDb : ICodePageDb
        {
            /// <summary>
            /// The fake code pages.
            /// </summary>
            private static readonly string[] FakePages = new[] { "page1", "page2" };

            /// <inheritdoc/>
            public IEnumerable<string> All => FakePages;

            /// <inheritdoc/>
            public void AddDone(Action action)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the canonical name for a code page alias.
            /// </summary>
            /// <param name="alias">Code page alias.</param>
            /// <returns>Canonical name.</returns>
            public string CanonicalName(string alias)
            {
                return alias;
            }

            public int Index(string codePage)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Fake model database class.
        /// </summary>
        private class FakeModelsDb : IModelsDb
        {
            /// <inheritdoc/>
            public IReadOnlyDictionary<int, ModelDimensions> Models => throw new NotImplementedException();

            /// <inheritdoc/>
            public void AddDone(Action action)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public int? DefaultColumns(int model)
            {
                return 80;
            }

            /// <inheritdoc/>
            public int? DefaultRows(int model)
            {
                return 24;
            }
        }

        /// <summary>
        /// Mock pfefixes database.
        /// </summary>
        private class FakePrefixDb : IHostPrefixDb
        {
            /// <inheritdoc/>
            public string Prefixes => "LY";
        }

        /// <summary>
        /// Fake back end database.
        /// </summary>
        private class FakeBackEndDb : IBackEndDb
        {
            /// <inheritdoc/>
            public ICodePageDb CodePageDb { get; } = new FakeCodePageDb();

            /// <inheritdoc/>
            public IModelsDb ModelsDb { get; } = new FakeModelsDb();

            /// <inheritdoc/>
            public IProxiesDb ProxiesDb => null;

            /// <inheritdoc/>
            public IHostPrefixDb HostPrefixDb { get; } = new FakePrefixDb();
        }
    }
}

// <copyright file="ImportUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
            var import = new Wc3270Import(new FakeCodePageDb(), new FakeModelsDb());
            import.Parse("wc3270.foo: bar");
        }

        /// <summary>
        /// Test the <see cref="Wc3270Import.Parse"/> method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ParseFail()
        {
            var import = new Wc3270Import(new FakeCodePageDb(), new FakeModelsDb());
            import.Parse("foo");
        }

        /// <summary>
        /// Trivial test of the <see cref="Wc3270Import.Digest"/> method.
        /// </summary>
        [TestMethod]
        public void DigestTrivial()
        {
            var import = new Wc3270Import(new FakeCodePageDb(), new FakeModelsDb());
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
            var import = new Wc3270Import(new FakeCodePageDb(), new FakeModelsDb());
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
            var import = new Wc3270Import(new FakeCodePageDb(), new FakeModelsDb());
            import.Parse("wc3270.model: 2");
            import.Parse("wc3270.model: 5");
            import.Parse("wc3270.model: 3");
            var profile = import.Digest();
            Assert.AreEqual(3, profile.Model);
        }

        /// <summary>
        ///  Fake code page database class.
        /// </summary>
        private class FakeCodePageDb : ICodePageDb
        {
            /// <inheritdoc/>
            public IEnumerable<string> All => throw new NotImplementedException();

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
                return null;
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
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public int? DefaultRows(int model)
            {
                throw new NotImplementedException();
            }
        }
    }
}

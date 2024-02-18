// <copyright file="OversizeUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="Oversize"/> class.
    /// </summary>
    [TestClass]
    public class OversizeUnitTest
    {
        /// <summary>
        /// Tests oversize parsing.
        /// </summary>
        [TestMethod]
        public void OversizeTest()
        {
            var testCases = new[]
            {
                // Success.
                new TestInstance("132x50", true, new Oversize { Rows = 50, Columns = 132 }),
                new TestInstance("132x124", true, new Oversize { Rows = 124, Columns = 132 }),

                // Failure.
                new TestInstance("132x125", false),
                new TestInstance("132x", false),
                new TestInstance("x125", false),
                new TestInstance("foo", false),
                new TestInstance("bar 132x50 baz", false),
                new TestInstance("0x100", false),
                new TestInstance("100x0", false),
                new TestInstance("1234567890123456789012345678x1", false),
                new TestInstance("1x1234567890123456789012345678", false),
                new TestInstance("123X50", false),
            };

            foreach (var t in testCases)
            {
                var success = Oversize.TryParse(t.Candidate, out Oversize o);
                Assert.AreEqual(t.Success, success, $"Expected {t.Success} success for {t.Candidate}");
                if (success)
                {
                    Assert.IsNotNull(o);
                    Assert.AreEqual(t.Oversize.Rows, o.Rows, $"Expected {t.Oversize.Rows} for {t.Candidate}");
                    Assert.AreEqual(t.Oversize.Columns, o.Columns, $"Expected {t.Oversize.Columns} for {t.Candidate}");
                }
                else
                {
                    Assert.IsNull(o);
                }
            }
        }

        /// <summary>
        /// Expected test results.
        /// </summary>
        private class TestInstance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestInstance"/> class.
            /// </summary>
            /// <param name="candidate">Candidate string.</param>
            /// <param name="success">Expected success.</param>
            /// <param name="oversize">Expected oversize.</param>
            public TestInstance(string candidate, bool success, Oversize oversize = null)
            {
                this.Candidate = candidate;
                this.Success = success;
                this.Oversize = oversize;
            }

            /// <summary>
            /// Gets or sets the candidate string.
            /// </summary>
            public string Candidate { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the parse should succeed.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Gets or sets the expected oversize, if successful.
            /// </summary>
            public Oversize Oversize { get; set; }
        }
    }
}

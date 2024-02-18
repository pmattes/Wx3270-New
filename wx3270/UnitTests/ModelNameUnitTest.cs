// <copyright file="ModelNameUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="ModelName"/> class.
    /// </summary>
    [TestClass]
    public class ModelNameUnitTest
    {
        /// <summary>
        /// Tests model name parsing.
        /// </summary>
        [TestMethod]
        public void ModelNameTest()
        {
            var testCases = new[]
            {
                // Basic 3279.
                new TestInstance("2", true, new ModelName { ModelNumber = 2, Color = true, Extended = true }),
                new TestInstance("3", true, new ModelName { ModelNumber = 3, Color = true, Extended = true }),
                new TestInstance("4", true, new ModelName { ModelNumber = 4, Color = true, Extended = true }),
                new TestInstance("5", true, new ModelName { ModelNumber = 5, Color = true, Extended = true }),
                new TestInstance("3279-2-E", true, new ModelName { ModelNumber = 2, Color = true, Extended = true }),
                new TestInstance("IBM-3279-2-E", true, new ModelName { ModelNumber = 2, Color = true, Extended = true }),

                // 3278.
                new TestInstance("3278-2-E", true, new ModelName { ModelNumber = 2, Color = false, Extended = true }),

                // Not extended.
                new TestInstance("3278-2", true, new ModelName { ModelNumber = 2, Color = false, Extended = false }),

                // Lower case.
                new TestInstance("ibm-3279-2-e", true, new ModelName { ModelNumber = 2, Color = true, Extended = true }),

                // Failure cases.
                new TestInstance("3271-2", false),
                new TestInstance("3279-6", false),
                new TestInstance("3279-2-", false),
                new TestInstance("3279", false),
                new TestInstance("IBM-3279", false),
                new TestInstance("IBM", false),
                new TestInstance("foo", false),
                new TestInstance("blah IBM-3279-2-E blah", false),
            };

            foreach (var t in testCases)
            {
                var success = ModelName.TryParse(t.Candidate, out ModelName m);
                Assert.AreEqual(t.Success, success, $"Expected {t.Success} success for {t.Candidate}");
                if (success)
                {
                    Assert.IsNotNull(m);
                    Assert.AreEqual(t.ModelName.ModelNumber, m.ModelNumber, $"Expected {t.ModelName.ModelNumber} for {t.Candidate}");
                    Assert.AreEqual(t.ModelName.Color, m.Color, $"Expected {t.ModelName.Color} for {t.Candidate}");
                    Assert.AreEqual(t.ModelName.Extended, m.Extended, $"Expected {t.ModelName.Extended} for {t.Candidate}");
                }
                else
                {
                    Assert.IsNull(m);
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
            /// <param name="modelName">Expected model name.</param>
            public TestInstance(string candidate, bool success, ModelName modelName = null)
            {
                this.Candidate = candidate;
                this.Success = success;
                this.ModelName = modelName;
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
            /// Gets or sets the expected model name, if successful.
            /// </summary>
            public ModelName ModelName { get; set; }
        }
    }
}

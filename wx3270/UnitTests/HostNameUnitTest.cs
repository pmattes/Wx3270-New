// <copyright file="HostNameUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="HostName"/> class.
    /// </summary>
    [TestClass]
    public class HostNameUnitTest
    {
        /// <summary>
        /// Tests simple host name parsing.
        /// </summary>
        [TestMethod]
        public void HostNameTest()
        {
            var expected = new[]
            {
                // Basic unquoted.
                new Expected("foo", true, null, null, "foo", null, null),
                new Expected("foo:22", true, null, null, "foo", "22", null),
                new Expected("foo:22=fred", true, null, null, "foo", "22", "fred"),
                new Expected("foo=fred", true, null, null, "foo", null, "fred"),
                new Expected("xx@foo", true, null, new List<string> { "xx" }, "foo", null, null),
                new Expected("xx,yy@foo", true, null, new List<string> { "xx", "yy" }, "foo", null, null),
                new Expected("a:foo", true, new List<char> { 'a' }, null, "foo", null, null),
                new Expected("a:b:foo", true, new List<char> { 'a', 'b' }, null, "foo", null, null),

                // Fun with quoted host names.
                new Expected("[foo]", true, null, null, "[foo]", null, null),
                new Expected("xx@[foo]", true, null, new List<string> { "xx" }, "[foo]", null, null),
                new Expected("[1:2:3:4::]", true, null, null, "[1:2:3:4::]", null, null),
                new Expected("xx@[foo]:22", true, null, new List<string> { "xx" }, "[foo]", "22", null),
                new Expected("[xx@foo:22]", false),
                new Expected("[1:2:3:4]", false),
                new Expected("[xx],yy@foo", false),

                // Fun with white space.
                new Expected(" foo", true, null, null, "foo", null, null),
                new Expected(" foo ", true, null, null, "foo", null, null),

                // Duplicates.
                new Expected("foo:22:23", false),
                new Expected("xx@yy@foo", false),
                new Expected("foo=a=b", false),

                // Empty parts.
                new Expected("@foo", false),
                new Expected("foo:", false),
                new Expected("foo=", false),
                new Expected("xx,@", false),
                new Expected(",xx@", false),
                new Expected("xx,@", false),
                new Expected(":23", false),
                new Expected("=23", false),
                new Expected("xx@:23", false),
                new Expected("foo:=fred", false),

                // White space.
                new Expected(" ", false),
                new Expected("a b ", false),

                // Out of order.
                new Expected("a=b@c", false),
                new Expected("az:b@c", false),
                new Expected("az=b:c", false),

                // Messed-up quotes.
                new Expected("[abc", false),
                new Expected("a[b]c", false),
                new Expected("[abc[]", false),
                new Expected("[]abc", false),
                new Expected("[foo][abc]", false),
                new Expected("[foo]@[abc]", false),
                new Expected("[fred]@abc", false),
                new Expected("[a:]fred", false),
                new Expected("fred=[22]", false),
                new Expected("a:[b]:c:fred", false),
                new Expected("fred:[22]", false),
                new Expected("fred=[baz]", false),
                new Expected("[]", false),
                new Expected("[", false),
                new Expected("ab]d", false),
            };

            foreach (var e in expected)
            {
                var result = HostName.TryParse(e.Item1, out List<char> prefixes, out List<string> lus, out string host, out string port, out string accept);
                Assert.AreEqual(e.Item2, result, $"Expected result for {e.Item1}");
                if (result)
                {
                    Assert.IsTrue(UnorderedEqual(e.Item3, prefixes), $"Expected prefixes for {e.Item1}");
                    Assert.IsTrue(UnorderedEqual(e.Item4, lus), $"Expected LUs for {e.Item1}");
                    Assert.AreEqual(e.Item5, host, $"Expected host for {e.Item1}");
                    Assert.AreEqual(e.Item6, port, $"Expected port for {e.Item1}");
                    Assert.AreEqual(e.Item7, accept, $"Expected accept for {e.Item1}");
                }
                else
                {
                    Assert.IsNull(prefixes);
                    Assert.IsNull(lus);
                    Assert.IsNull(host);
                    Assert.IsNull(port);
                    Assert.IsNull(accept);
                }
            }
        }

        /// <summary>
        /// Compares two unordered enumerables.
        /// </summary>
        /// <typeparam name="T">Type of arguments.</typeparam>
        /// <param name="expected">Expected values.</param>
        /// <param name="got">Comparison values.</param>
        /// <returns>True if equal.</returns>
        private static bool UnorderedEqual<T>(IEnumerable<T> expected, IEnumerable<T> got)
        {
            if ((expected == null) != (got == null))
            {
                return false;
            }

            if (expected == null)
            {
                return true;
            }

            var expectedOrdered = expected.OrderBy(x => x).ToList();
            var gotOrdered = got.OrderBy(x => x).ToList();
            return expectedOrdered.SequenceEqual(gotOrdered);
        }

        /// <summary>
        /// Expected test results.
        /// </summary>
        private class Expected : Tuple<string, bool, List<char>, List<string>, string, string, string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Expected"/> class.
            /// </summary>
            /// <param name="text">Subject text.</param>
            /// <param name="result">Expected result.</param>
            /// <param name="prefixes">Expected prefixes.</param>
            /// <param name="lus">Expected LUs.</param>
            /// <param name="host">Expected host name.</param>
            /// <param name="port">Expected port.</param>
            /// <param name="accept">Expected accept name.</param>
            public Expected(string text, bool result, List<char> prefixes = null, List<string> lus = null, string host = null, string port = null, string accept = null)
                : base(text, result, prefixes, lus, host, port, accept)
            {
            }
        }
    }
}

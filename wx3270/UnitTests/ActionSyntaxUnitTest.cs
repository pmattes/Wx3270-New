// <copyright file="ActionSyntaxUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="ActionSyntax"/> class.
    /// </summary>
    [TestClass]
    public class ActionSyntaxUnitTest
    {
        /// <summary>
        /// Test basic success cases.
        /// </summary>
        [TestMethod]
        public void ActionBasicSuccess()
        {
            int column;
            int index = 0;
            string errorText;
            Assert.IsTrue(ActionSyntax.CheckLine("a()", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a( )", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(1)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(1,2)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a( 1,2)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(1, 2)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(1,2)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(1 ,2) b(3,4)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\", there)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello there\")", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello)\")", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a-b1_23zzy(1,2,3)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("_23zzy(1,2,3)", out column, ref index, out errorText));
        }

        /// <summary>
        /// Test backslash success cases.
        /// </summary>
        [TestMethod]
        public void ActionBackslash()
        {
            int column;
            int index = 0;
            string errorText;
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\\\"there\")", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello there\\\\\")", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello there\\\\\\\")", out column, ref index, out errorText));
        }

        /// <summary>
        /// Test unusual success cases.
        /// </summary>
        [TestMethod]
        public void ActionOddball()
        {
            int column;
            int index = 0;
            string errorText;
            Assert.IsTrue(ActionSyntax.CheckLine("a(12,abc\"123)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine(@"a(C:\Users\Fred\Foo)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine(@"a(\,1,2)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine(@"a(,,,)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine(@"a(,1,)", out column, ref index, out errorText));
            Assert.IsTrue(ActionSyntax.CheckLine(" a(,1,) ", out column, ref index, out errorText));
        }

        /// <summary>
        /// Test various failure cases.
        /// </summary>
        [TestMethod]
        public void ActionFail()
        {
            int column;
            int index = 0;
            string errorText;
            Assert.IsFalse(ActionSyntax.CheckLine("&x", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x&()", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x(", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x(1,", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x(\"1", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("x ()", out column, ref index, out errorText));
            Assert.IsFalse(ActionSyntax.CheckLine("-x()", out column, ref index, out errorText));

            // This one is impossible to express in Xt translation table syntax:
            // Two backslashes followed by a double quote inside of a quoted string (\\") terminate the string,
            // (they indicate a backslash at the end of the string), so there is no way to embed that sequence
            // with anything following it.
            Assert.IsFalse(ActionSyntax.CheckLine("a(\"hello\\\\\"there\")", out column, ref index, out errorText));
        }

        /// <summary>
        /// Test argument isolation.
        /// </summary>
        [TestMethod]
        public void ArgumentParsing()
        {
            int column;
            int index = 0;
            string errorText;
            string[] args;

            // Simple unquoted arguments.
            Assert.IsTrue(ActionSyntax.CheckLine("a(1,2,3)", out column, ref index, out errorText, out args));
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual("1", args[0]);
            Assert.AreEqual("2", args[1]);
            Assert.AreEqual("3", args[2]);

            // Simple quoted arguments.
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"a\",\"b\",\"c\")", out column, ref index, out errorText, out args));
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual("a", args[0]);
            Assert.AreEqual("b", args[1]);
            Assert.AreEqual("c", args[2]);

            // More interesting unquoted argument (quotes and backslashes).
            Assert.IsTrue(ActionSyntax.CheckLine("a(a\"b\\c)", out column, ref index, out errorText, out args));
            Assert.AreEqual(1, args.Length);
            Assert.AreEqual("a\"b\\c", args[0]);

            // More interesting quoted argument (escaped double quote).
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\\\"there\")", out column, ref index, out errorText, out args));
            Assert.AreEqual(1, args.Length);
            Assert.AreEqual("hello\"there", args[0]);

            // More interesting quoted argument (escaped ordinary character).
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\\" + "pthere\")", out column, ref index, out errorText, out args));
            Assert.AreEqual(1, args.Length);
            Assert.AreEqual("hello\\" + "pthere", args[0]);

            // More interesting quoted argument (backslash at end).
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\\\\\")", out column, ref index, out errorText, out args));
            Assert.AreEqual(1, args.Length);
            Assert.AreEqual("hello\\", args[0]);

            // More interesting quoted argument (triple backslash in middle).
            Assert.IsTrue(ActionSyntax.CheckLine("a(\"hello\\\\\\there\")", out column, ref index, out errorText, out args));
            Assert.AreEqual(1, args.Length);
            Assert.AreEqual("hello\\\\\\there", args[0]);

            // Empty argument (middle).
            Assert.IsTrue(ActionSyntax.CheckLine("a(1,,3)", out column, ref index, out errorText, out args));
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual("1", args[0]);
            Assert.AreEqual(string.Empty, args[1]);
            Assert.AreEqual("3", args[2]);

            // Empty argument (before).
            Assert.IsTrue(ActionSyntax.CheckLine("a(,2)", out column, ref index, out errorText, out args));
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(string.Empty, args[0]);
            Assert.AreEqual("2", args[1]);

            // Empty argument (end).
            Assert.IsTrue(ActionSyntax.CheckLine("a(,)", out column, ref index, out errorText, out args));
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(string.Empty, args[0]);
            Assert.AreEqual(string.Empty, args[1]);

            // More empty arguments.
            Assert.IsTrue(ActionSyntax.CheckLine("a(,,)", out column, ref index, out errorText, out args));
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual(string.Empty, args[0]);
            Assert.AreEqual(string.Empty, args[1]);
            Assert.AreEqual(string.Empty, args[2]);
        }

        /// <summary>
        /// Test the <see cref="ActionSyntax.FormatForRun(string)"/> method.
        /// </summary>
        [TestMethod]
        public void FormatForRun()
        {
            Assert.AreEqual(
                "a() b()",
                ActionSyntax.FormatForRun("a()" + Environment.NewLine + "# Comment" + Environment.NewLine + " " + Environment.NewLine + "  # Another comment" + Environment.NewLine + "   b()   "));
        }
    }
}

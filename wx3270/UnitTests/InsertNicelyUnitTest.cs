// <copyright file="InsertNicelyUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="Oversize"/> class.
    /// </summary>
    [TestClass]
    public class InsertNicelyUnitTest
    {
        /// <summary>
        /// Tests the <see cref="MacroEditor.InsertNicely(string, string, int)"/> method.
        /// </summary>
        [TestMethod]
        public void FirstTest()
        {
            const string FirstLine = "oldLine1";
            const string SecondLine = "oldline2";
            const string ThirdLine = "oldLine3";
            string oldText = FirstLine + Environment.NewLine + SecondLine;
            const string NewText = "newLine";

            // Basic tests.
            // Insert at the beginning.
            var t = MacroEditor.InsertNicely(NewText, QuickState(oldText, 0), out int newCursor);
            Assert.AreEqual(NewText + Environment.NewLine + oldText + Environment.NewLine, t);
            Assert.AreEqual(NewText.Length, newCursor);

            // Insert midway through the first line, which means at the beginning.
            t = MacroEditor.InsertNicely(NewText, QuickState(oldText, 1), out newCursor);
            Assert.AreEqual(NewText + Environment.NewLine + oldText + Environment.NewLine, t);
            Assert.AreEqual(NewText.Length, newCursor);

            // Insert at the end.
            t = MacroEditor.InsertNicely(NewText, QuickState(oldText, oldText.Length), out newCursor);
            Assert.AreEqual(oldText + Environment.NewLine + NewText + Environment.NewLine, t);
            Assert.AreEqual(-1, newCursor);

            // Insert with the cursor at the beginning of the second line.
            // The insertion should be between the lines.
            t = MacroEditor.InsertNicely(NewText, QuickState(oldText, FirstLine.Length + Environment.NewLine.Length), out newCursor);
            Assert.AreEqual(FirstLine + Environment.NewLine + NewText + Environment.NewLine + SecondLine + Environment.NewLine, t);
            Assert.AreEqual(FirstLine.Length + Environment.NewLine.Length + NewText.Length, newCursor);

            // Insert with the cursor in the middle of the second line.
            // The insertion should also be between the lines.
            t = MacroEditor.InsertNicely(NewText, QuickState(oldText, FirstLine.Length + Environment.NewLine.Length + 1), out newCursor);
            Assert.AreEqual(FirstLine + Environment.NewLine + NewText + Environment.NewLine + SecondLine + Environment.NewLine, t);
            Assert.AreEqual(FirstLine.Length + Environment.NewLine.Length + NewText.Length, newCursor);

            // Insert with the cursor at the end of the first line.
            // The insertion should also be between the lines.
            t = MacroEditor.InsertNicely(NewText, QuickState(oldText, FirstLine.Length), out newCursor);
            Assert.AreEqual(FirstLine + Environment.NewLine + NewText + Environment.NewLine + SecondLine + Environment.NewLine, t);
            Assert.AreEqual(FirstLine.Length + Environment.NewLine.Length + NewText.Length, newCursor);

            // Empty old text.
            t = MacroEditor.InsertNicely(NewText, QuickState(string.Empty, 0), out newCursor);
            Assert.AreEqual(NewText + Environment.NewLine, t);
            Assert.AreEqual(-1, newCursor);

            // No saved state.
            t = MacroEditor.InsertNicely(NewText, null, out newCursor);
            Assert.AreEqual(NewText + Environment.NewLine, t);
            Assert.AreEqual(-1, newCursor);

            // Paste tests.
            // Insert overwriting an entire middle line, even though only one character was selected.
            t = MacroEditor.InsertNicely(NewText, new MacroEditor.EditorState { Text = oldText + Environment.NewLine + ThirdLine, SelectionStart = FirstLine.Length + Environment.NewLine.Length + 2, SelectionLength = 1 }, out newCursor);
            Assert.AreEqual(FirstLine + Environment.NewLine + NewText + Environment.NewLine + ThirdLine + Environment.NewLine, t);
            Assert.AreEqual(FirstLine.Length + Environment.NewLine.Length + NewText.Length, newCursor);

            // Same thing, overwriting the first line.
            t = MacroEditor.InsertNicely(NewText, new MacroEditor.EditorState { Text = oldText, SelectionStart = 2, SelectionLength = 1 }, out newCursor);
            Assert.AreEqual(NewText + Environment.NewLine + SecondLine + Environment.NewLine, t);
            Assert.AreEqual(NewText.Length, newCursor);

            // Same thing overwriting the only line.
            t = MacroEditor.InsertNicely(NewText, new MacroEditor.EditorState { Text = FirstLine, SelectionStart = 2, SelectionLength = 1 }, out newCursor);
            Assert.AreEqual(NewText + Environment.NewLine, t);
            Assert.AreEqual(-1, newCursor);
        }

        /// <summary>
        /// Constructs a <see cref="MacroEditor.EditorState"/> object.
        /// </summary>
        /// <param name="text">Current text.</param>
        /// <param name="cursor">Cursor position.</param>
        /// <returns>Editor state object.</returns>
        private static MacroEditor.EditorState QuickState(string text, int cursor) => new MacroEditor.EditorState
        {
            Text = text,
            SelectionStart = cursor,
            SelectionLength = 0,
        };
    }
}

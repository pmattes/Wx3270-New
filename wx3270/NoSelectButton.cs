// <copyright file="NoSelectButton.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// A button that is not selectable.
    /// </summary>
    public class NoSelectButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSelectButton"/> class.
        /// </summary>
        public NoSelectButton()
            : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}

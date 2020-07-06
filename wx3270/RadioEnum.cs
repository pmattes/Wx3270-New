// <copyright file="RadioEnum.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// A class for mapping between an enumeration and a set of radio buttons.
    /// </summary>
    /// <typeparam name="T">Type of enumeration</typeparam>
    public class RadioEnum<T>
        where T : struct
    {
        /// <summary>
        /// The default value;
        /// </summary>
        private static T defaultValue = default(T);

        /// <summary>
        /// The set of radio buttons.
        /// </summary>
        private HashSet<Tuple<RadioButton, T>> buttons = new HashSet<Tuple<RadioButton, T>>();

        /// <summary>
        /// The current value of the enumeration.
        /// </summary>
        private T currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioEnum{T}"/> class.
        /// </summary>
        /// <param name="container">Container holding the buttons</param>
        public RadioEnum(Control container)
        {
            this.Container = container;

            // Scan the container for the radio buttons.
            foreach (var button in container.Controls.OfType<RadioButton>())
            {
                this.Add(button);
            }
        }

        /// <summary>
        /// Event for value changes.
        /// </summary>
        public event EventHandler Changed = (sender, e) => { };

        /// <summary>
        /// Gets the container.
        /// </summary>
        public Control Container { get; private set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public T Value
        {
            get
            {
                return this.currentValue;
            }

            set
            {
                this.currentValue = value;
                foreach (var b in this.buttons)
                {
                    if (b.Item2.Equals(value))
                    {
                        if (!b.Item1.Checked)
                        {
                            b.Item1.Checked = true;
                            this.Changed(this, null);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add a radio button explicitly.
        /// </summary>
        /// <param name="button">Radio button to add</param>
        public void Add(RadioButton button)
        {
            button.Click += this.OnClick;
            var e = (T)Enum.Parse(typeof(T), (string)button.Tag);
            this.buttons.Add(new Tuple<RadioButton, T>(button, e));
            if (button.Checked)
            {
                this.currentValue = e;
            }
        }

        /// <summary>
        /// Override for string conversion.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return this.currentValue.ToString();
        }

        /// <summary>
        /// A radio button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnClick(object sender, EventArgs e)
        {
            var previousValue = this.currentValue;

            // Start with the default.
            this.currentValue = defaultValue;

            // Scan for a button that is checked.
            foreach (var b in this.buttons)
            {
                if (b.Item1.Checked)
                {
                    this.currentValue = b.Item2;
                    break;
                }
            }

            // If the value changed, signal our change event.
            if (!this.currentValue.Equals(previousValue))
            {
                this.Changed(this, null);
            }
        }
    }
}

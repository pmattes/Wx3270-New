// <copyright file="Wx3270Restrict.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

/// <summary>
/// Wx3270 restrictions utility.
/// </summary>
namespace wx3270Restrict
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using Wx3270;

    /// <summary>
    /// Wx3270 restrictions utility.
    /// </summary>
    /// <remarks>
    /// Needs to be run as administrator to set restrictions.
    /// </remarks>
    public class Wx3270Restrict
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                new Main();

                // Start the event loop.
                Application.Run();
                Application.Exit();
            }

            switch (args[0].ToLowerInvariant())
            {
                case "-show":
                    if (args.Length != 1)
                    {
                        Usage();
                    }

                    ShowRestrictions();
                    break;
                case "-set":
                    if (args.Length != 2)
                    {
                        Usage();
                    }

                    SetRestrictions(args[1]);
                    break;
                case "-clear":
                    if (args.Length != 1)
                    {
                        Usage();
                    }

                    SetRestrictions(Restrictions.None.ToString());
                    break;
                default:
                    Usage($"Invalid option '{args[0]}'");
                    break;
            }
        }

        /// <summary>
        /// Show the current restrictions.
        /// </summary>
        private static void ShowRestrictions()
        {
            var key = Registry.LocalMachine.OpenSubKey(Constants.Misc.RegistryKey);
            try
            {
                var value = (key != null) ? (string)key.GetValue(Constants.Misc.RestrictionsValue) : null;
                if (value != null)
                {
                    if (Enum.TryParse(value, true, out Restrictions r))
                    {
                        Popup(r.ToString(), MessageBoxIcon.Information);
                    }
                    else
                    {
                        Popup($"Invalid restrictions in the registry: '{value}'");
                    }

                    return;
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            Popup(Restrictions.None.ToString(), MessageBoxIcon.Information);
        }

        /// <summary>
        /// Set the restrictions.
        /// </summary>
        /// <param name="value">Value to set</param>
        private static void SetRestrictions(string value)
        {
            if (!Enum.TryParse(value, true, out Restrictions r))
            {
                Popup($"Invalid restrictions '{value}'");
                return;
            }

            RegistryKey key = null;
            try
            {
                try
                {
                    key = Registry.LocalMachine.CreateSubKey(Constants.Misc.RegistryKey);
                }
                catch (Exception e)
                {
                    Popup($"Registry key: {e.Message}");
                    return;
                }

                try
                {
                    key.SetValue(Constants.Misc.RestrictionsValue, r.ToString());
                }
                catch (Exception e)
                {
                    Popup($"Registry key: {e.Message}");
                    return;
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
        }

        /// <summary>
        /// Display a usage message and exit.
        /// </summary>
        /// <param name="reason">Reason for exit</param>
        private static void Usage(string reason = null)
        {
            var output = new List<string>();
            if (reason != null)
            {
                output.Add(reason);
            }

            output.Add("Usage:");
            output.Add("  Wx3270Restrict");
            output.Add("  Wx3270Restrict -show");
            output.Add("  Wx3270Restrict -set restriction{,restriction}");
            output.Add("  Wx3270Restrict -clear");
            output.Add(
                "Restrictions are: " +
                string.Join(", ", Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>().Select(m => m.ToString())));
            Popup(string.Join(Environment.NewLine, output));
            Environment.Exit(1);
        }

        /// <summary>
        /// Pop up an error message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="icon">Optional icon.</param>
        private static void Popup(string message, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBox.Show(message, "wx3270Restrict", MessageBoxButtons.OK, icon);
        }
    }
}

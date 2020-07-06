// <copyright file="Wx3270Restrict.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

/// <summary>
/// Wx3270 restrictions utility.
/// </summary>
namespace wx3270Restrict
{
    using System;
    using System.Linq;

    using Microsoft.Win32;
    using Wx3270;

    /// <summary>
    /// Wx3270 restrictions utility.
    /// </summary>
    /// <remarks>
    /// Needs to be run as administrator to set restrictions.
    /// </remarks>
    class Wx3270Restrict
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
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
        static void ShowRestrictions()
        {
            var key = Registry.LocalMachine.OpenSubKey(Constants.Misc.RegistryKey);
            try
            {
                var value = (key != null) ? (string)key.GetValue(Constants.Misc.RestrictionsValue) : null;
                if (value != null)
                {
                    if (Enum.TryParse(value, true, out Restrictions r))
                    {
                        Console.WriteLine("{0}", r);
                    }
                    else
                    {
                        Console.Error.WriteLine("Invalid restrictions in the registry: '{0}'", value);
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

            Console.WriteLine("{0}", Restrictions.None);
        }

        /// <summary>
        /// Set the restrictions.
        /// </summary>
        /// <param name="value">Value to set</param>
        static void SetRestrictions(string value)
        {
            if (!Enum.TryParse(value, true, out Restrictions r))
            {
                Console.Error.WriteLine("Invalid restrictions '{0}'", value);
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
                    Console.Error.WriteLine("Registry key: {0}", e.Message);
                    return;
                }

                try
                {
                    key.SetValue(Constants.Misc.RestrictionsValue, r.ToString());
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Registry key: {0}", e.Message);
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
        static void Usage(string reason = null)
        {
            if (reason != null)
            {
                Console.WriteLine("{0}", reason);
            }

            Console.WriteLine("Usage: Wx3270Restrict -show");
            Console.WriteLine("       Wx3270Restrict -set restriction{,restriction}");
            Console.WriteLine("       Wx3270Restrict -clear");
            Console.WriteLine(
                "Restrictions are: {0}",
                string.Join(", ", Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>().Select(m => m.ToString())));
            Environment.Exit(1);
        }
    }
}

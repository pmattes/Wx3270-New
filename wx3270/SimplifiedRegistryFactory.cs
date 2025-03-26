// <copyright file="SimplifiedRegistryFactory.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Simplified registry factory.
    /// </summary>
    public static class SimplifiedRegistryFactory
    {
        /// <summary>
        /// Returns a new instance of a particular type of simplified registry.
        /// </summary>
        /// <param name="fake">True to talk to the real registry.</param>
        /// <returns>Simplified registry.</returns>
        public static ISimplifiedRegistry Get(bool fake = false)
        {
            return fake ? (ISimplifiedRegistry)new FakeRegistry() : (ISimplifiedRegistry)new RealRegistry();
        }
    }
}
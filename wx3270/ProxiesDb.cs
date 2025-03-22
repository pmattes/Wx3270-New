// <copyright file="ProxiesDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handler for model indications from the back end.
    /// </summary>
    public class ProxiesDb : BackEndEvent, IProxiesDb
    {
        /// <summary>
        /// The dictionary of proxies.
        /// </summary>
        private readonly Dictionary<string, Proxy> proxies = new Dictionary<string, Proxy>();

        /// <summary>
        /// True if we are inside a proxies block.
        /// </summary>
        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxiesDb"/> class.
        /// </summary>
        public ProxiesDb()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Proxy, this.StartProxy),
                new BackEndEventDef(B3270.Indication.Proxies, this.StartProxies, this.EndProxies),
            };

            // Make sure we always have 'none'.
            this.proxies[Proxy.None] = new Proxy(Proxy.None, null, false);
        }

        /// <summary>
        /// Event that is signaled when the proxies list is ready.
        /// </summary>
        public event Action Done = () => { };

        /// <summary>
        /// Gets the dictionary of models.
        /// </summary>
        public IReadOnlyDictionary<string, Proxy> Proxies => this.proxies;

        /// <summary>
        /// Processes a proxy indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartProxy(string name, AttributeDict attributes)
        {
            if (this.running)
            {
                var proxyName = attributes[B3270.Attribute.Name];
                this.proxies[proxyName] = new Proxy(
                    proxyName,
                    attributes.ContainsKey(B3270.Attribute.Port) ? int.Parse(attributes[B3270.Attribute.Port]) : (int?)null,
                    bool.Parse(attributes[B3270.Attribute.Username]));
            }
        }

        /// <summary>
        /// Processes a proxies start indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartProxies(string name, AttributeDict attributes)
        {
            this.running = true;
        }

        /// <summary>
        /// Processes a proxies end indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        private void EndProxies(string name)
        {
            this.running = false;
            this.Done();
        }
    }
}

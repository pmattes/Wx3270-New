// <copyright file="ProxyParserUnitTest.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="ProxyParser"/> class.
    /// </summary>
    [TestClass]
    public class ProxyParserUnitTest
    {
        /// <summary>
        /// Test basic success cases with a plain address.
        /// </summary>
        [TestMethod]
        public void ProxyParserSuccess()
        {
            var p = new ProxyParser(GetProxiesDb());
            string proxyType, address, username, password, failReason;
            ushort? port;

            Assert.IsTrue(p.TryParse("able:1.2.3.4", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.IsNull(port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:1.2.3.4:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:fred@1.2.3.4:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.AreEqual("fred", username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:fred:pass@1.2.3.4:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.AreEqual("fred", username);
            Assert.AreEqual("pass", password);
            Assert.IsNull(failReason);
        }

        /// <summary>
        /// Test basic success cases with a bracketed address.
        /// </summary>
        [TestMethod]
        public void ProxyParserSuccessBracket()
        {
            var p = new ProxyParser(GetProxiesDb());
            string proxyType, address, username, password, failReason;
            ushort? port;

            Assert.IsTrue(p.TryParse("able:[1.2.3.4]", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.IsNull(port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:[1.2.3.4]:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:fred@[1.2.3.4]:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.AreEqual("fred", username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);

            Assert.IsTrue(p.TryParse("able:fred:pass@[1.2.3.4]:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("1.2.3.4", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.AreEqual("fred", username);
            Assert.AreEqual("pass", password);
            Assert.IsNull(failReason);
        }

        /// <summary>
        /// Test basic success case with a numeric IPv6 address.
        /// </summary>
        [TestMethod]
        public void ProxyParserSuccessIpv6()
        {
            var p = new ProxyParser(GetProxiesDb());
            string proxyType, address, username, password, failReason;
            ushort? port;

            Assert.IsTrue(p.TryParse("able:fred:pass@[::1]:80", out proxyType, out address, out port, out username, out password, out failReason), failReason);
            Assert.AreEqual("able", proxyType);
            Assert.AreEqual("::1", address);
            Assert.AreEqual((ushort?)80, port);
            Assert.AreEqual("fred", username);
            Assert.AreEqual("pass", password);
            Assert.IsNull(failReason);
        }

        /// <summary>
        /// Test success with an empty string.
        /// </summary>
        [TestMethod]
        public void ProxyParserSuccessEmpty()
        {
            var p = new ProxyParser(GetProxiesDb());
            string proxyType, address, username, password, failReason;
            ushort? port;

            Assert.IsTrue(p.TryParse(string.Empty, out proxyType, out address, out port, out username, out password, out failReason, nullIsNone: true), failReason);
            Assert.IsNull(proxyType);
            Assert.IsNull(address);
            Assert.IsNull(port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);
        }

        /// <summary>
        /// Test success with an empty string.
        /// </summary>
        [TestMethod]
        public void ProxyParserSuccessNull()
        {
            var p = new ProxyParser(GetProxiesDb());
            string proxyType, address, username, password, failReason;
            ushort? port;

            Assert.IsTrue(p.TryParse(null, out proxyType, out address, out port, out username, out password, out failReason, nullIsNone: true), failReason);
            Assert.IsNull(proxyType);
            Assert.IsNull(address);
            Assert.IsNull(port);
            Assert.IsNull(username);
            Assert.IsNull(password);
            Assert.IsNull(failReason);
        }

        /// <summary>
        /// Test failure of an empty string.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmpty()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse(string.Empty, out _, out _, out _, out _, out _, out _), "Empty string should fail");
        }

        /// <summary>
        /// Test failure of a string with white space.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailWhiteSpace()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("able: 1.2.3.4", out _, out _, out _, out _, out _, out _), "String with white space should fail");
        }

        /// <summary>
        /// Test failure of no colon.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailNoColon()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("fred", out _, out _, out _, out _, out _, out _), "No colon should fail");
        }

        /// <summary>
        /// Test failure of a bad type.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailBadType()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("charlie:1.2.3.4", out _, out _, out _, out _, out _, out _), "Bad type should fail");
        }

        /// <summary>
        /// Test failure of an empty user name.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyUsername1()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:@1.2.3.4", out _, out _, out _, out _, out _, out _), "Empty username should fail");
        }

        /// <summary>
        /// Test failure of an empty user name with a password.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyUsername2()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker::@1.2.3.4", out _, out _, out _, out _, out _, out _), "Empty username should fail");
        }

        /// <summary>
        /// Test failure of an empty password.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyPassword()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:fred:@1.2.3.4", out _, out _, out _, out _, out _, out _), "Empty password should fail");
        }

        /// <summary>
        /// Test failure of a missing ']'.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailMissingRightBracket()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:[1.2.3.4", out _, out _, out _, out _, out _, out _), "Missing right bracket should fail");
        }

        /// <summary>
        /// Test failure of an empty address.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailMissingEmptyAddress()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:[]", out _, out _, out _, out _, out _, out _), "Empty address should fail");
        }

        /// <summary>
        /// Test failure of garbge after ']'.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailGarbageAfterRightBracket()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:[1.2.3.4]foo", out _, out _, out _, out _, out _, out _), "Garbage after right bracket should fail");
        }

        /// <summary>
        /// Test failure of invalid port.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailInvalidPort()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:[1.2.3.4]:123baz", out _, out _, out _, out _, out _, out _), "Invalid port should fail");
        }

        /// <summary>
        /// Test failure of invalid port.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailInvalidPort2()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:1.2.3.4:123baz", out _, out _, out _, out _, out _, out _), "Invalid port should fail");
        }

        /// <summary>
        /// Test failure of empty address.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyAddress()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker::", out _, out _, out _, out _, out _, out _), "Empty address should fail");
        }

        /// <summary>
        /// Test failure of empty address.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyAddress2()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker::123", out _, out _, out _, out _, out _, out _), "Empty address should fail");
        }

        /// <summary>
        /// Test failure of empty address.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailEmptyAddress3()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:", out _, out _, out _, out _, out _, out _), "Empty address should fail");
        }

        /// <summary>
        /// Test failure of spurious username.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailSpuriousUsername()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("baker:user@1.2.3.4", out _, out _, out _, out _, out _, out _), "Spurious username should fail");
        }

        /// <summary>
        /// Test failure of missing port.
        /// </summary>
        [TestMethod]
        public void ProxyParserFailMissingPort()
        {
            var p = new ProxyParser(GetProxiesDb());
            Assert.IsFalse(p.TryParse("charlie:1.2.3.4", out _, out _, out _, out _, out _, out _), "Missing port should fail");
        }

        /// <summary>
        /// Gets a fake proxies database.
        /// </summary>
        /// <returns>Fake proxies database.</returns>
        private static IProxiesDb GetProxiesDb()
        {
            return new FakeProxiesDb(
                new Dictionary<string, Proxy>
                {
                    { "able", new Proxy("able", 1234, true) },
                    { "baker", new Proxy("baker", 5678, false) },
                    { "charlie", new Proxy("charlie", null, false) },
                });
        }

        /// <summary>
        /// Fake proxies database class.
        /// </summary>
        private class FakeProxiesDb : IProxiesDb
        {
            /// <summary>
            /// The dictionary of proxies.
            /// </summary>
            private readonly Dictionary<string, Proxy> proxies;

            /// <summary>
            /// Initializes a new instance of the <see cref="FakeProxiesDb"/> class.
            /// </summary>
            /// <param name="proxies">Proxy dictionary.</param>
            public FakeProxiesDb(Dictionary<string, Proxy> proxies)
            {
                this.proxies = proxies;
            }

            /// <inheritdoc/>
            public IReadOnlyDictionary<string, Proxy> Proxies => this.proxies;
        }
    }
}

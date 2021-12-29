// <copyright file="OiaDraw.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Wx3270.Contracts;

    /// <summary>
    /// The main screen class.
    /// </summary>
    public partial class MainScreen
    {
        /// <summary>
        /// Gets the keyboard modifier.
        /// </summary>
        public KeyboardModifier Mod { get; private set; }

        /// <summary>
        /// Update the network field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaNetwork(IOiaState oia)
        {
            string disp = this.oia3270Font ? OiaFont.Symbol.Box4 : "4";
            string tip = string.Empty;

            switch (oia.HostNetworkState)
            {
                case HostNetworkState.Blank:
                    disp += this.oia3270Font ? " " : "_";
                    tip = I18n.Get(OiaToolTipName.IoInProgress);
                    break;
                case HostNetworkState.UnderscoreA:
                    disp += this.oia3270Font ? OiaFont.Symbol.UnderA : "A̲";
                    if (oia.ConnectionState == ConnectionState.NotConnected)
                    {
                        tip = I18n.Get(OiaToolTipName.IoNotConnected);
                    }
                    else
                    {
                        tip = I18n.Get(OiaToolTipName.IoConnected);
                    }

                    break;
                case HostNetworkState.UnderscoreB:
                    disp += this.oia3270Font ? OiaFont.Symbol.UnderB : "B̲";
                    tip = I18n.Get(OiaToolTipName.IoConnectedTn3270E);
                    break;
            }

            switch (oia.LocalNetworkState)
            {
                case LocalNetworkState.BoxQuestion:
                    disp += this.oia3270Font ? OiaFont.Symbol.BoxQuestion : "?";
                    break;
                case LocalNetworkState.BoxSolid:
                    disp += this.oia3270Font ? OiaFont.Symbol.BoxSolid : "▮";
                    tip += " " + string.Format(I18n.Get(OiaToolTipName.IoInMode), "3270");
                    break;
                case LocalNetworkState.Nvt:
                    disp += this.oia3270Font ? OiaFont.Symbol.BoxN : "N";
                    tip += " " + string.Format(I18n.Get(OiaToolTipName.IoInMode), "NVT");
                    break;
                case LocalNetworkState.SscpLu:
                    disp += this.oia3270Font ? OiaFont.Symbol.BoxHuman : "S";
                    tip += " " + string.Format(I18n.Get(OiaToolTipName.IoInMode), "SSCP-LU");
                    break;
            }

            this.Oia4AB.Text = disp;
            this.toolTip1.SetToolTip(this.Oia4AB, tip);
        }

        /// <summary>
        /// Update the lock field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaLock(IOiaState oia)
        {
            var text = string.Empty;
            var tooltip = string.Empty;
            Color? color3279 = (this.colorMode && this.colors != null) ? this.colors.HostColors[HostColor.NeutralWhite] : (Color?)null;
            string tag = null;
            switch (oia.Lock)
            {
                case Lock.Deferred:
                    text = this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay;
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + string.Format(I18n.Get(OiaToolTipName.Deferred), 350);
                    break;
                case Lock.Inhibit:
                    text = (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " Inhibit";
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Inhibit);
                    break;
                case Lock.Minus:
                    // XXX: Could also be SSCP-LU operation
                    text = (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " -f";
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Minus);
                    break;
                case Lock.NotConnected:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow;
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " " + I18n.Get(OiaMessage.NotConnected);
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.NotConnected);
                    break;
                case Lock.OperatorError:
                    text = this.OperatorErrorText(oia);
                    tooltip = this.OperatorErrorToolTip(oia);
                    color3279 = (this.colorMode && this.colors != null) ? this.colors.HostColors[HostColor.Red] : (Color?)null;
                    tag = "oerr";
                    break;
                case Lock.Scrolled:
                    text = (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " Scrolled " + oia.ScrollCount;
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Scrolled);
                    break;
                case Lock.SysWait:
                    text = (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " SYSTEM";
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.SysWait);
                    break;
                case Lock.TimeWait:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.ClockLeft + OiaFont.Symbol.ClockRight;
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " " + (VersionSpecific.Win10OrGreater ? VersionSpecific.Clock : "WAIT");
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.TimeWait);
                    break;
                case Lock.Unknown:
                    text = (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " ?";
                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Unknown);
                    break;
                case Lock.Disabled:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.KeyLeft + OiaFont.Symbol.KeyRight;
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " Disabled";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Disabled);
                    break;
                case Lock.Reconnecting:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " " + OiaFont.Symbol.ClockLeft + OiaFont.Symbol.ClockRight;
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " " + (VersionSpecific.Win10OrGreater ? VersionSpecific.Clock : "...");
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Reconnecting);
                    break;
                case Lock.Resolving:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [DNS]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [DNS]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Resolving);
                    break;
                case Lock.TcpPending:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [TCP]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [TCP]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.TcpPending);
                    break;
                case Lock.ProxyPending:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [Proxy]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [Proxy]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.ProxyPending);
                    break;
                case Lock.TlsPending:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [TLS]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [TLS]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.TlsPending);
                    break;
                case Lock.TelnetPending:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [TELNET]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [TELNET]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.TelnetPending);
                    break;
                case Lock.Tn3270EPending:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommBad + OiaFont.Symbol.CommHigh + OiaFont.Symbol.CommJag + OiaFont.Symbol.CommLow +
                            " [TN3270E]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [TN3270E]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Tn3270EPending);
                    break;
                case Lock.Field:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + "[Field]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [Field]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Field);
                    break;
                case Lock.FileTransfer:
                    if (this.oia3270Font)
                    {
                        text = OiaFont.Symbol.X + " " + "[File Transfer]";
                    }
                    else
                    {
                        text = VersionSpecific.DeleteDisplay + " [File Transfer]";
                    }

                    tooltip = I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.FileTransfer);
                    break;
                case Lock.Unlocked:
                    break;
            }

            this.OiaLock.Text = text;
            this.OiaLock.Tag = tag;
            if (color3279.HasValue)
            {
                this.OiaLock.ForeColor = color3279.Value;
            }

            this.toolTip1.SetToolTip(this.OiaLock, tooltip);
        }

        /// <summary>
        /// Update the cursor field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaCursor(IOiaState oia)
        {
            if (oia.CursorEnabled && oia.CursorRow > 0 && oia.CursorColumn > 0)
            {
                Trace.Line(Trace.Type.Draw, $"ChangeOiaCursor enabled {oia.CursorRow}/{oia.CursorColumn}");
                this.oiaCursor.Text = string.Format("{0:D3}/{1:D3}", oia.CursorRow, oia.CursorColumn);
                this.toolTip1.SetToolTip(this.oiaCursor, string.Format(I18n.Get(OiaToolTipName.CursorPos), oia.CursorRow, oia.CursorColumn));
            }
            else
            {
                Trace.Line(Trace.Type.Draw, $"ChangeOiaCursor disabled");
                this.oiaCursor.Text = string.Empty;
                this.toolTip1.SetToolTip(this.oiaCursor, string.Empty);
            }
        }

        /// <summary>
        /// Update the Shift/Alt indication in the OIA.
        /// </summary>
        /// <param name="mod">New keyboard modifiers.</param>
        /// <param name="mask">Modifier mask.</param>
        public void ChangeOiaMod(KeyboardModifier mod, KeyboardModifier mask)
        {
            if ((this.Mod & mask) == (mod & mask))
            {
                return;
            }

            this.Mod = (this.Mod & ~mask) | (mod & mask);

            if (this.oia3270Font)
            {
                this.OiaAltShift.Text = string.Format(
                    "{0}{1}{2}",
                    this.Mod.HasFlag(KeyboardModifier.Alt) ? "A" : " ",
                    this.Mod.HasFlag(KeyboardModifier.Shift) ? OiaFont.Symbol.UpShift : " ",
                    this.Mod.HasFlag(KeyboardModifier.Apl) ? "α" : " ");
            }
            else
            {
                this.OiaAltShift.Text = string.Format(
                    "{0}{1}{2}",
                    this.Mod.HasFlag(KeyboardModifier.Alt) ? "A" : " ",
                    this.Mod.HasFlag(KeyboardModifier.Shift) ? "⇑" : " ",
                    this.Mod.HasFlag(KeyboardModifier.Apl) ? VersionSpecific.AplDisplay : " ");
            }

            var toolTip = new List<string>();
            if (this.Mod.HasFlag(KeyboardModifier.Alt))
            {
                toolTip.Add("Alt");
            }

            if (this.Mod.HasFlag(KeyboardModifier.Shift))
            {
                toolTip.Add("Shift");
            }

            if (this.Mod.HasFlag(KeyboardModifier.Apl))
            {
                toolTip.Add("APL");
            }

            this.toolTip1.SetToolTip(this.OiaAltShift, string.Join(", ", toolTip));
        }

        /// <summary>
        /// Update the insert field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaInsert(IOiaState oia)
        {
            this.OiaInsert.Text = oia.Insert ? (this.oia3270Font ? OiaFont.Symbol.Insert : "^") : string.Empty;
            this.toolTip1.SetToolTip(this.OiaInsert, oia.Insert ? I18n.Get(OiaToolTipName.Insert) : string.Empty);
        }

        /// <summary>
        /// Update the TLS field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaTls(IOiaState oia)
        {
            if (oia.ConnectionState <= ConnectionState.TcpPending)
            {
                this.OiaTLS.Text = string.Empty;
                this.toolTip1.SetToolTip(this.OiaTLS, string.Empty);
                return;
            }

            this.OiaTLS.Text = this.oia3270Font ? OiaFont.Symbol.Lock : VersionSpecific.LockDisplay;
            if (oia.Secure)
            {
                if (oia.Verified)
                {
                    this.OiaTLS.ForeColor = this.ColorMap[HostColor.Green];
                    this.toolTip1.SetToolTip(this.OiaTLS, I18n.Get(OiaToolTipName.Secure));
                }
                else
                {
                    this.OiaTLS.ForeColor = this.ColorMap[HostColor.Yellow];
                    this.toolTip1.SetToolTip(this.OiaTLS, I18n.Get(OiaToolTipName.Unverified));
                }
            }
            else
            {
                this.OiaTLS.ForeColor = this.ColorMap[HostColor.Red];
                this.toolTip1.SetToolTip(this.OiaTLS, I18n.Get(OiaToolTipName.NotSecure));
            }
        }

        /// <summary>
        /// Update the LU field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaLu(IOiaState oia)
        {
            this.oiaLu.Text = oia.Lu;
        }

        /// <summary>
        /// Change the timing field in the OIA.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        public void ChangeOiaTiming(IOiaState oia)
        {
            if (!string.IsNullOrEmpty(oia.Timing))
            {
                var secs = int.Parse(oia.Timing.Split(new[] { '.' })[0]);
                var clock = this.oia3270Font ? OiaFont.Symbol.ClockLeft + OiaFont.Symbol.ClockRight : VersionSpecific.ClockDisplay;
                if (secs < 10)
                {
                    this.oiaTiming.Text = clock + oia.Timing;
                }
                else if (secs < 60)
                {
                    this.oiaTiming.Text = clock + secs;
                }
                else if (secs < 60 * 60)
                {
                    this.oiaTiming.Text = clock + string.Format("{0}m", secs / 60);
                }
                else if (secs < 60 * 60 * 24)
                {
                    this.oiaTiming.Text = clock + string.Format("{0}h", secs / (60 * 60));
                }
                else
                {
                    this.oiaTiming.Text = clock + string.Format("{0}d", secs / (60 * 60 * 24));
                }
            }
            else
            {
                this.oiaTiming.Text = string.Empty;
            }
        }

        /// <summary>
        /// Gets the tool tip text to display for a given operator error state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        /// <returns>Text to display.</returns>
        private string OperatorErrorToolTip(IOiaState oia)
        {
            switch (oia.OperatorError)
            {
                case OperatorError.DBCS:
                    return I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Dbcs);
                case OperatorError.Numeric:
                    return I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Numeric);
                case OperatorError.Overflow:
                    return I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Overflow);
                case OperatorError.Protected:
                    return I18n.Get(OiaToolTipName.KeyboardLocked) + ":" + Environment.NewLine + I18n.Get(OiaToolTipName.Protected);
                default:
                case OperatorError.None:
                case OperatorError.Unknown:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the text to display for a given operator error state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        /// <returns>Text to display.</returns>
        private string OperatorErrorText(IOiaState oia)
        {
            switch (oia.OperatorError)
            {
                case OperatorError.DBCS:
                    return (this.oia3270Font ? OiaFont.Symbol.X : VersionSpecific.DeleteDisplay) + " <S>";
                case OperatorError.Numeric:
                    return (this.oia3270Font ? (OiaFont.Symbol.X + " " + OiaFont.Symbol.Human) : (VersionSpecific.DeleteDisplay + " ")) + "NUM";
                case OperatorError.Overflow:
                    return this.oia3270Font ? (OiaFont.Symbol.X + " " + OiaFont.Symbol.Human + ">") : (VersionSpecific.DeleteDisplay + " " + VersionSpecific.OverflowDisplay);
                case OperatorError.Protected:
                    return this.oia3270Font ?
                        (OiaFont.Symbol.X + " " + OiaFont.Symbol.LeftArrow + OiaFont.Symbol.Human + OiaFont.Symbol.RightArrow) :
                        (VersionSpecific.DeleteDisplay + " " + VersionSpecific.ProtectedDisplay);
                default:
                case OperatorError.None:
                case OperatorError.Unknown:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Change the OIA screen trace count.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        private void ChangeOiaScreenTrace(IOiaState oia)
        {
            var text = string.Empty;
            var tip = string.Empty;
            switch (oia.ScreenTrace)
            {
                case -1:
                    // Off.
                    break;
                case 0:
                    // Pending.
                    text = "-";
                    tip = I18n.Get(OiaToolTipName.ScreenTracePending);
                    break;
                default:
                    // Many screens saved.
                    text = oia.ScreenTrace < 10 ? oia.ScreenTrace.ToString() : "+";
                    tip = string.Format(I18n.Get(oia.ScreenTrace == 1 ? OiaToolTipName.ScreenTraceSaved : OiaToolTipName.ScreenTracesSaved), oia.ScreenTrace);
                    break;
            }

            this.OiaScreentrace.Text = text;
            this.toolTip1.SetToolTip(this.OiaScreentrace, tip);
        }

        /// <summary>
        /// Change the OIA printer session state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        private void ChangeOiaPrinterSession(IOiaState oia)
        {
            this.OiaPrinter.Text = oia.PrinterSession ? (this.oia3270Font ? OiaFont.Symbol.Printer : VersionSpecific.PrinterDisplay) : string.Empty;
            this.toolTip1.SetToolTip(this.OiaPrinter, oia.PrinterSession ? I18n.Get(OiaToolTipName.PrinterSession) : string.Empty);
        }

        /// <summary>
        /// Change the OIA typeahead state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        private void ChangeOiaTypeahead(IOiaState oia)
        {
            this.OiaTypeahead.Text = oia.Typeahead ? "T" : string.Empty;
            this.toolTip1.SetToolTip(this.OiaTypeahead, oia.Typeahead ? I18n.Get(OiaToolTipName.Typeahead) : string.Empty);
        }

        /// <summary>
        /// Change the script state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        private void ChangeScript(IOiaState oia)
        {
            this.OiaScript.Text = oia.Script ? "s" : string.Empty;
            this.toolTip1.SetToolTip(this.OiaScript, oia.Script ? I18n.Get(OiaToolTipName.Script) : string.Empty);
        }

        /// <summary>
        /// Change reverse-input state.
        /// </summary>
        /// <param name="oia">OIA state.</param>
        private void ChangeReverseInput(IOiaState oia)
        {
            this.OiaReverse.Text = oia.ReverseInput ? "R" : string.Empty;
            this.toolTip1.SetToolTip(this.OiaReverse, oia.ReverseInput ? I18n.Get(OiaToolTipName.ReverseInput) : string.Empty);
        }

        /// <summary>
        /// Initialize localized OIA strings.
        /// </summary>
        private void InitOiaLocalization()
        {
            I18n.LocalizeGlobal(OiaToolTipName.IoInProgress, "I/O in progress");
            I18n.LocalizeGlobal(OiaToolTipName.IoNotConnected, "Not connected");
            I18n.LocalizeGlobal(OiaToolTipName.IoConnected, "Connected");
            I18n.LocalizeGlobal(OiaToolTipName.IoConnectedTn3270E, "Connected via TN3270E");
            I18n.LocalizeGlobal(OiaToolTipName.IoInMode, "in {0} mode");

            I18n.LocalizeGlobal(OiaToolTipName.Reconnecting, "Reconnecting");
            I18n.LocalizeGlobal(OiaToolTipName.Resolving, "Resolving host name");
            I18n.LocalizeGlobal(OiaToolTipName.TcpPending, "TCP connection pending");
            I18n.LocalizeGlobal(OiaToolTipName.ProxyPending, "Proxy negotiation pending");
            I18n.LocalizeGlobal(OiaToolTipName.TlsPending, "TLS negotiation pending");
            I18n.LocalizeGlobal(OiaToolTipName.TelnetPending, "TELNET negotiation pending");
            I18n.LocalizeGlobal(OiaToolTipName.Tn3270EPending, "TN3270E negotiation pending");

            I18n.LocalizeGlobal(OiaToolTipName.KeyboardLocked, "Keyboard locked");
            I18n.LocalizeGlobal(OiaToolTipName.Deferred, "{0}ms delay after host unlock request");
            I18n.LocalizeGlobal(OiaToolTipName.Inhibit, "Query Reply sent, no response yet");
            I18n.LocalizeGlobal(OiaToolTipName.Minus, "ATTN sent to host, no response yet");
            I18n.LocalizeGlobal(OiaToolTipName.NotConnected, "No connection to host");
            I18n.LocalizeGlobal(OiaToolTipName.Scrolled, "Screen is scrolled");
            I18n.LocalizeGlobal(OiaToolTipName.SysWait, "Host responded to AID but has not unlocked the keyboard");
            I18n.LocalizeGlobal(OiaToolTipName.TimeWait, "AID sent to host, no response yet");
            I18n.LocalizeGlobal(OiaToolTipName.Unknown, "Cause unknown");
            I18n.LocalizeGlobal(OiaToolTipName.Disabled, "Disabled by script");
            I18n.LocalizeGlobal(OiaToolTipName.Field, "Waiting for the host to format the screen");

            I18n.LocalizeGlobal(OiaToolTipName.Dbcs, "Invalid DBCS operation");
            I18n.LocalizeGlobal(OiaToolTipName.Numeric, "Non-number entered into numeric field");
            I18n.LocalizeGlobal(OiaToolTipName.Overflow, "Character inserted into full field");
            I18n.LocalizeGlobal(OiaToolTipName.Protected, "Character inserted into protected field");

            I18n.LocalizeGlobal(OiaToolTipName.Typeahead, "Typeahead stored");
            I18n.LocalizeGlobal(OiaToolTipName.PrinterSession, "Printer session active");
            I18n.LocalizeGlobal(OiaToolTipName.Insert, "Insert mode is on");
            I18n.LocalizeGlobal(OiaToolTipName.Script, "Script is running");
            I18n.LocalizeGlobal(OiaToolTipName.ReverseInput, "Reverse input mode is on");

            I18n.LocalizeGlobal(OiaToolTipName.ScreenTracePending, "Screen trace pending");
            I18n.LocalizeGlobal(OiaToolTipName.ScreenTraceSaved, "{0} screen trace saved");
            I18n.LocalizeGlobal(OiaToolTipName.ScreenTracesSaved, "{0} screen traces saved");

            I18n.LocalizeGlobal(OiaToolTipName.Secure, "Connection is secure");
            I18n.LocalizeGlobal(OiaToolTipName.Unverified, "Connection is secure, but host is not verified");
            I18n.LocalizeGlobal(OiaToolTipName.NotSecure, "Connection is not secure");

            I18n.LocalizeGlobal(OiaToolTipName.CursorPos, "Cursor is at row {0}, column {1} (1-origin)");

            // Localize other static tool tips.
            this.toolTip1.SetToolTip(this.oiaLu, I18n.Localize(this.oiaLu, I18n.ToolTipName, this.toolTip1.GetToolTip(this.oiaLu)));
            this.toolTip1.SetToolTip(this.oiaTiming, I18n.Localize(this.oiaTiming, I18n.ToolTipName, this.toolTip1.GetToolTip(this.oiaTiming)));

            // Localize the "not connected" message.
            I18n.LocalizeGlobal(OiaMessage.NotConnected, "Not connected");
        }

        /// <summary>
        /// Localized tool tip strings.
        /// </summary>
        private static class OiaToolTipName
        {
            /// <summary>
            /// I/O in progress.
            /// </summary>
            public static readonly string IoInProgress = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "IoInProgress");

            /// <summary>
            /// I/O state not connected.
            /// </summary>
            public static readonly string IoNotConnected = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "IoNotConnected");

            /// <summary>
            /// I/O connected.
            /// </summary>
            public static readonly string IoConnected = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "IoConnected");

            /// <summary>
            /// I/O connected in TN3270E mode.
            /// </summary>
            public static readonly string IoConnectedTn3270E = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "IoConnectedTn3270E");

            /// <summary>
            /// I/O connected in a particular mode.
            /// </summary>
            public static readonly string IoInMode = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "IoInMode");

            /// <summary>
            /// Reconnect in progress.
            /// </summary>
            public static readonly string Reconnecting = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Reconnecting");

            /// <summary>
            /// Name resolution in progress.
            /// </summary>
            public static readonly string Resolving = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Resolving");

            /// <summary>
            /// TCP connection pending.
            /// </summary>
            public static readonly string TcpPending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "TcpPending");

            /// <summary>
            /// Proxy pending.
            /// </summary>
            public static readonly string ProxyPending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "ProxyPending");

            /// <summary>
            /// TLS pending.
            /// </summary>
            public static readonly string TlsPending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "TlsPending");

            /// <summary>
            /// TELNET pending.
            /// </summary>
            public static readonly string TelnetPending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "TelnetPending");

            /// <summary>
            /// TN3270E pending.
            /// </summary>
            public static readonly string Tn3270EPending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Tn3270EPending");

            /// <summary>
            /// Keyboard locked.
            /// </summary>
            public static readonly string KeyboardLocked = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "KeyboardLocked");

            /// <summary>
            /// Keyboard unlock deferred.
            /// </summary>
            public static readonly string Deferred = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Deferred");

            /// <summary>
            /// Keyboard inhibited.
            /// </summary>
            public static readonly string Inhibit = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Inhibit");

            /// <summary>
            /// Keyboard minus.
            /// </summary>
            public static readonly string Minus = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Minus");

            /// <summary>
            /// Keyboard locked, not connected.
            /// </summary>
            public static readonly string NotConnected = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "NotConnected");

            /// <summary>
            /// Screen scrolled.
            /// </summary>
            public static readonly string Scrolled = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Scrolled");

            /// <summary>
            /// System wait.
            /// </summary>
            public static readonly string SysWait = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "SysWait");

            /// <summary>
            /// Time wait.
            /// </summary>
            public static readonly string TimeWait = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "TimeWait");

            /// <summary>
            /// Unknown keyboard lock.
            /// </summary>
            public static readonly string Unknown = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Unknown");

            /// <summary>
            /// Disabled by script.
            /// </summary>
            public static readonly string Disabled = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Disabled");

            /// <summary>
            /// DBCS field error.
            /// </summary>
            public static readonly string Dbcs = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Dbcs");

            /// <summary>
            /// Numeric field error.
            /// </summary>
            public static readonly string Numeric = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Numeric");

            /// <summary>
            /// Field overflow error.
            /// </summary>
            public static readonly string Overflow = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Overflow");

            /// <summary>
            /// Protected field error.
            /// </summary>
            public static readonly string Protected = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Protected");

            /// <summary>
            /// Typeahead stored.
            /// </summary>
            public static readonly string Typeahead = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Typeahead");

            /// <summary>
            /// Active printer session.
            /// </summary>
            public static readonly string PrinterSession = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "PrinterSession");

            /// <summary>
            /// Insert mode.
            /// </summary>
            public static readonly string Insert = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Insert");

            /// <summary>
            /// Script pending.
            /// </summary>
            public static readonly string Script = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Script");

            /// <summary>
            /// Screen trace pending.
            /// </summary>
            public static readonly string ScreenTracePending = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "ScreenTracePending");

            /// <summary>
            /// Screen trace saved.
            /// </summary>
            public static readonly string ScreenTraceSaved = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "ScreenTraceSaved");

            /// <summary>
            /// Multiple screen traces saved.
            /// </summary>
            public static readonly string ScreenTracesSaved = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "ScreenTracesSaved");

            /// <summary>
            /// Reverse input mode.
            /// </summary>
            public static readonly string ReverseInput = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "ReverseInput");

            /// <summary>
            /// Connection is secure.
            /// </summary>
            public static readonly string Secure = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Secure");

            /// <summary>
            /// Secure with unverified host.
            /// </summary>
            public static readonly string Unverified = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Unverified");

            /// <summary>
            /// Not secure.
            /// </summary>
            public static readonly string NotSecure = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "NotSecure");

            /// <summary>
            /// Cursor position.
            /// </summary>
            public static readonly string CursorPos = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "CursorPos");

            /// <summary>
            /// Waiting for input field.
            /// </summary>
            public static readonly string Field = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "Field");

            /// <summary>
            /// Waiting for a file transfer.
            /// </summary>
            public static readonly string FileTransfer = I18n.Combine(nameof(MainScreen), I18n.ToolTipName, "FileTransfer");
        }

        /// <summary>
        /// Miscellaneous messages.
        /// </summary>
        private static class OiaMessage
        {
            /// <summary>
            /// Not connected.
            /// </summary>
            public static readonly string NotConnected = I18n.Combine(nameof(MainScreen), "NotConnected");
        }
    }
}

// <copyright file="AplMode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using I18nBase;

    /// <summary>
    /// APL mode state machine.
    /// </summary>
    public class AplMode
    {
        /// <summary>
        /// Error pop-up title.
        /// </summary>
        private static readonly string ErrorTitle = I18n.Combine(I18n.PopUpTitleName(nameof(AplMode)), "Error");

        /// <summary>
        /// The back end.
        /// </summary>
        private readonly BackEnd backEnd;

        /// <summary>
        /// The last APL mode sent to the back end.
        /// </summary>
        private State state;

        /// <summary>
        /// Initializes a new instance of the <see cref="AplMode"/> class.
        /// </summary>
        /// <param name="backEnd">Back end.</param>
        public AplMode(BackEnd backEnd)
        {
            this.backEnd = backEnd;
        }

        /// <summary>
        /// Possible values for <see cref="state"/>.
        /// </summary>
        private enum State
        {
            Unknown,
            Set,
            Clear,
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(ErrorTitle, "APL Mode Setting Error");
        }

        /// <summary>
        /// Toggle APL mode.
        /// </summary>
        /// <param name="isSet">True if APL mode is now on.</param>
        public void Toggle(bool isSet)
        {
            var newState = isSet ? State.Set : State.Clear;
            if (this.state != newState)
            {
                this.backEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.AplMode, isSet ? B3270.Value.Set : B3270.Value.Clear), ErrorBox.Completion(I18n.Get(ErrorTitle)));
                this.state = newState;
            }
        }
    }
}

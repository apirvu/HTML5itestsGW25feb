/**
 * Copyright (c) 2007-2014, Kaazing Corporation. All rights reserved.
 */

using System;
using Kaazing.Security;

namespace Kaazing.HTML5.Demo
{


    ///
    /// Challenge handler for Basic authentication. See RFC 2617.
    ///
    public class LoginHandlerDemo : LoginHandler
    {
        private HTML5DemoForm parent;
        /// <summary>
        /// constructor
        /// <para>pass in main form for callback</para>
        /// </summary>
        /// <param name="form"></param>
        public LoginHandlerDemo(HTML5DemoForm form)
        {
            this.parent = form;
        }


        #region LoginHandler Members

        PasswordAuthentication LoginHandler.GetCredentials()
        {
            return parent.AuthenticationHandler();
        }

        #endregion
    }
}
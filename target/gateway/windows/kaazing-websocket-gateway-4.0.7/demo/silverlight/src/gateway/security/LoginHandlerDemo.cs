/**
 * Copyright (c) 2007-2012, Kaazing Corporation. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls.Primitives;
using Kaazing.Security;

namespace Kaazing.HTML5.Demo
{
    /**
     * Challenge handler for Basic authentication. See RFC 2617.
     */
    public class LoginHandlerDemo : LoginHandler
    {
        private MainPage parent;
        
        public LoginHandlerDemo(MainPage mainPage)
        {
            parent = mainPage;
        }

        //handle Challenge request from server
        public PasswordAuthentication GetCredentials()
        {
            parent.Dispatcher.BeginInvoke(parent.PopupLoginPage);

            //wait user click OK or Cacel button
            parent.userInputCompleted.WaitOne();
            return parent.Credentials;
        }    
    }
}
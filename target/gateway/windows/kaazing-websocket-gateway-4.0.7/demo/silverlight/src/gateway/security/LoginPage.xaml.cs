/**
 * Copyright (c) 2007-2012, Kaazing Corporation. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Kaazing.Security;

namespace Kaazing.HTML5.Demo
{
    public partial class LoginPage : UserControl
    {
        private MainPage parent;

        public LoginPage(MainPage mainPage)
        {
            parent = mainPage;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            parent.Credentials = new PasswordAuthentication(UsernameText.Text, PasswordText.Password.ToCharArray());
            // Remove the control from the layout. 
            this.Visibility = Visibility.Collapsed;
            parent.userInputCompleted.Set(); //signal Authenticate thread
        }

        private void Cancelutton_Click(object sender, RoutedEventArgs e)
        {
            parent.Credentials = null;
            // Remove the control from the layout. 
            this.Visibility = Visibility.Collapsed;
            parent.userInputCompleted.Set();
        }
    }
}

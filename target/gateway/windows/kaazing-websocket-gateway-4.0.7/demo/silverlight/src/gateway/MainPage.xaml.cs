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
using System.Windows.Browser;

using Kaazing.HTML5;
using Kaazing.Security;
using System.Windows.Controls.Primitives;

namespace Kaazing.HTML5.Demo
{
    /// <summary>
    /// Silverlight HTML5 Demo Main Page
    /// </summary>
    public partial class MainPage : UserControl
    {
        WebSocket webSocket;

        // username and password from Login Popup Page
        public PasswordAuthentication Credentials;
        public System.Threading.AutoResetEvent userInputCompleted = new System.Threading.AutoResetEvent(false);

        /// <summary>
        /// Silverlight HTML5 Demo Main Page
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            Origin sourceOrigin = Origin.GetSourceOrigin();
            String defaultLocation = "";
            if (sourceOrigin.Scheme == "file")
            {
                defaultLocation = "ws://localhost:8001/echo";
            }
            else
            {
                defaultLocation = "ws://" + sourceOrigin.Host + ":8001/echo";
            }

            LocationText.Text = defaultLocation;

            if (Application.Current.InstallState == InstallState.NotInstalled)
            {
                btnInstall.IsEnabled = true;
            }

            //Logger.LoggerCallback = HandleLog;

            // Create handler for authentication
            BasicChallengeHandler basicHandler = ChallengeHandlers.Load<BasicChallengeHandler>(typeof(BasicChallengeHandler));
            basicHandler.LoginHandler = new LoginHandlerDemo(this);
            ChallengeHandlers.Default = basicHandler;

            /// Note you can use DispatchHandler to assign different challengeHandlers for different locations
            /// dispatchHandler = ChallengeHandlers.Load<DispatchChallengeHandler>(typeof(DefaultDispatchChallengeHandler));
            /// ChallengeHandlers.Default = dispatchHandler;

            /// set loginHandler for this location
            /// DefaultBasicChallengeHandler basicHandler = ChallengeHandlers.Load<DefaultBasicChallengeHandler>(typeof(DefaultBasicChallengeHandler));
            /// basicHandler.LoginHandler = new LoginHandlerDemo(this);
            /// dispatchHandler.Register("ws://myserver.com/*", basicHandler);
        }
        
        private void HandleLog(String message)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                Log("LOG: " + message);
            });
        }

        /*
         *  Button click handlers
         */

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Immediately disable the connect button
            ConnectButton.IsEnabled = false;

            webSocket = new WebSocket();
            Log("CONNECT:" + LocationText.Text);

            webSocket.OpenEvent += new OpenEventHandler(OpenHandler);
            webSocket.CloseEvent += new CloseEventHandler(CloseHandler);
            webSocket.MessageEvent += new MessageEventHandler(MessageHandler);

            webSocket.Connect(LocationText.Text);
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Log("DISCONNECT");
            webSocket.Close();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Log("SEND:" + MessageText.Text);
            webSocket.Send(MessageText.Text);
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            btnInstall.IsEnabled = false;

            try
            {
                Application.Current.Install();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("The application is already installed.");
            }
        }

        /*
         * Console output 
         */
        private const int LOG_LIMIT = 50;
        private Queue<string> logLines = new Queue<string>();
        private void Log(string arg)
        {
            logLines.Enqueue(arg);
            if (logLines.Count > LOG_LIMIT)
            {
                logLines.Dequeue();
            }
            string[] o = logLines.ToArray<string>();

            o=o.Reverse<string>().ToArray<string>();

            Output.Text = string.Join("\n", o);
        }

        private void ClearOutput_Click(object sender, RoutedEventArgs e)
        {
            logLines.Clear();
            Output.Text = "";
        }


        /*
         * Event Handlers
         */
        private void OpenHandler(object sender, OpenEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                Log("CONNECTED");

                DisconnectButton.IsEnabled = true;
                SendButton.IsEnabled = true;
            });
        }

        private void CloseHandler(object sender, CloseEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                Log("DISCONNECTED");

                ConnectButton.IsEnabled = true;
                DisconnectButton.IsEnabled = false;
                SendButton.IsEnabled = false;
            });
        }

        private void MessageHandler(object sender, MessageEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                Log("MESSAGE: " + args.Data);
            });
        }

        public void PopupLoginPage()
        {
            Popup p = new Popup();

            // Set the Child property of Popup to an instance of LoginPage. 
            LoginPage login = new LoginPage(this);
            p.Child = login;
            
            // Set where the popup will show up on the screen. 
            p.VerticalOffset = 200;
            p.HorizontalOffset = 200;

            // Open the popup. 
            p.IsOpen = true;
        }
    }
}

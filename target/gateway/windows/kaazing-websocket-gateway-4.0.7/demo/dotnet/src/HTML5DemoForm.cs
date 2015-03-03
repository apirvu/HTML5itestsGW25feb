/**
 * Copyright (c) 2007-2014, Kaazing Corporation. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using Kaazing.HTML5;
using Kaazing.Security;
using System.Threading;

namespace Kaazing.HTML5.Demo
{
    /// <summary>
    /// .Net HTML5 Demo Main Page
    /// </summary>
    public partial class HTML5DemoForm : Form
    {
        private WebSocket webSocket = null;

        public delegate void InvokeDelegate();

        /// <summary>
        /// .Net HTML5 Demo Form
        /// </summary>
        public HTML5DemoForm()
        {

            InitializeComponent();

            String defaultLocation = "ws://localhost:8001/echo";
            LocationText.Text = defaultLocation;

            // TODO: if we decide to get rid of any future Favicon, we can get rid of these lines. Until than, we will keep them
            //ResourceManager resourceManager = new ResourceManager ("com.kaazing.gateway.html5.client.demo.HTML5DemoForm", GetType().Assembly);
            //this.Icon = (Icon)resourceManager.GetObject("Favicon.Ico");

            //Logger.LoggerCallback = HandleLog;

            //setup ChallengeHandler to handler Basic/Application Basic authentications
            BasicChallengeHandler basicHandler = ChallengeHandlers.Load<BasicChallengeHandler>(typeof(BasicChallengeHandler));
            basicHandler.LoginHandler = new LoginHandlerDemo(this);
            ChallengeHandlers.Default = basicHandler;

            ///
            /// create authentication handler to handle authentiacations
            /// use DispatchChallengeHandler to assign different ChallengeHandler to each location
            ///
            /// dispatchHandler = ChallengeHandlers.Load<DispatchChallengeHandler>(typeof(DispatchChallengeHandler));
            /// ChallengeHandlers.Default = dispatchHandler;
            /// LoginHandler loginHandler = new LoginHandlerDemo(this);
            ///
            /// //set a loginHandler for this location
            /// BasicChallengeHandler basicHandler = ChallengeHandlers.Load<BasicChallengeHandler>(typeof(BasicChallengeHandler));
            /// basicHandler.LoginHandler = loginHandler;
            /// dispatchHandler.Register("ws://myserver.com/*", basicHandler);
            ///
            /// //add another handler
            /// BasicChallengeHandler basicHandler2 = ChallengeHandlers.Load<BasicChallengeHandler>(typeof(BasicChallengeHandler));
            /// basicHandler2.LoginHandler = loginHandler2;
            /// dispatchHandler.Register("ws://otherserver.com/*", basicHandler2);
        }

        private void HTML5DemoForm_Load(object sender, EventArgs e)
        {
        }

        private void HandleLog(String message)
        {
            this.BeginInvoke((InvokeDelegate)(() =>
            {
                Log("LOG: " + message);
            }));
        }

        ///
        ///  Button click handlers
        ///
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // Immediately disable the connect button
            ConnectButton.Enabled = false;
            webSocket = new WebSocket();
            Log("CONNECT:" + LocationText.Text);

            webSocket.OpenEvent += new OpenEventHandler(OpenHandler);
            webSocket.CloseEvent += new CloseEventHandler(CloseHandler);
            webSocket.MessageEvent += new MessageEventHandler(MessageHandler);

            webSocket.Connect(LocationText.Text);
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Log("DISCONNECT");
            webSocket.Close();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Log("SEND:" + MessageText.Text);
            webSocket.Send(MessageText.Text);
        }

        ///
        /// Console output
        ///
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

            o = o.Reverse<string>().ToArray<string>();

            Output.Text = string.Join("\r\n", o);
        }

        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            logLines.Clear();
            Output.Text = "";
        }


        /*
         * HTML5 Event Handlers
         */
        ///
        /// Handle server authentication challenge request,
        /// Popup a login window for username/password
        ///
        public PasswordAuthentication AuthenticationHandler()
        {
            PasswordAuthentication credentials = null;
            AutoResetEvent userInputCompleted = new AutoResetEvent(false);
            this.BeginInvoke((InvokeDelegate)(() =>
            {
                LoginDemoForm loginForm = new LoginDemoForm();
                if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    credentials = new PasswordAuthentication(loginForm.Username, loginForm.Password.ToCharArray());
                }
                userInputCompleted.Set();
            }));
            // wait user click 'OK' or 'Cancel' on login window
            userInputCompleted.WaitOne();
            return credentials;
        }

        ///
        /// HTML5 Event Handlers
        ///
        private void OpenHandler(object sender, OpenEventArgs args)
        {
            this.BeginInvoke((InvokeDelegate)(() =>
            {
                Log("CONNECTED");

                DisconnectButton.Enabled = true;
                SendButton.Enabled = true;
            }));
        }

        private void CloseHandler(object sender, CloseEventArgs args)
        {
            this.BeginInvoke((InvokeDelegate)(() =>
            {
                Log("DISCONNECTED");

                ConnectButton.Enabled = true;
                DisconnectButton.Enabled = false;
                SendButton.Enabled = false;
            }));
        }

        private void MessageHandler(object sender, MessageEventArgs args)
        {
            this.BeginInvoke((InvokeDelegate)(() =>
            {
                Log("MESSAGE: " + args.Data);
            }));
        }
    }
}

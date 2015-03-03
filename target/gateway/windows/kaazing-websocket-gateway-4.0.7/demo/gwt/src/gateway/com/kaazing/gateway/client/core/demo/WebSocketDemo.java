/**
 * Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
 */

package com.kaazing.gateway.client.core.demo;

import com.kaazing.gateway.client.ByteBuffer;
import com.kaazing.gateway.client.Charset;
import com.kaazing.gateway.client.WebSocket;
import com.kaazing.gateway.client.WebSocketException;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.TextArea;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.Window;


public class WebSocketDemo implements EntryPoint, WebSocket.OpenHandler,
                                      WebSocket.MessageHandler, WebSocket.CloseHandler
{
    //Kaazing WebSocket implementation for GWT
    WebSocket ws;

    //UI widgets to manipulate the connection
    public TextArea logTextArea ;
    public TextBox msgField;
    public TextBox locField;
    Button openButton;
    Button closeButton;
    Button clearButton;
    Button sendButton;
    Button sendButton2;
    Charset UTF8;
    
    @Override
    public void onModuleLoad() {
       
      /* Top Info */
      HTML topInfo = new HTML("This demo uses the WebSocket API to send text messages to the<BR>"+
                              "Kaazing Gateway Echo service, which echoes back the messages.");
      topInfo.setStyleName("info");
      topInfo.setWidth("600px");

      /* Build Location Panel */
      Label locLabel = new Label("Location");
      locLabel.setStyleName("label");

      locField = new TextBox();
      locField.setWidth("200");
      locField = new TextBox();
      locField.setWidth("200");
      String hostname = Window.Location.getHostName();
      String port = Window.Location.getPort();
      String scheme = Window.Location.getProtocol();
      if (scheme == "http:") {
          scheme = "ws:";
      } 
      else if (scheme == "https:") {
          scheme = "wss:"; 
      }
      locField.setText(scheme + "//" + hostname + ":" + port + "/echo");
      
      HorizontalPanel hLocFieldPanel = new HorizontalPanel();
      hLocFieldPanel.setSpacing(6);
      hLocFieldPanel.add(locLabel);
      hLocFieldPanel.add(locField);

      openButton = new Button("Connect");
      openButton.setSize("80px", "24px");

      closeButton = new Button("Disconnect");
      closeButton.setEnabled(false);
      closeButton.setSize("80px", "24px");

      HorizontalPanel hLocPanel = new HorizontalPanel();
      hLocPanel.add(openButton);
      hLocPanel.add(closeButton);
      hLocPanel.setSpacing(6);

      VerticalPanel locationPanel = new VerticalPanel();
      locationPanel.add(hLocFieldPanel);
      locationPanel.add(hLocPanel);
      
      /* Build Message Panel */
      Label msgLabel = new Label("Message");
      msgLabel.setStyleName("label");

      msgField = new TextBox();
      msgField.setText("Hello, WebSocket!");

      sendButton = new Button("Send Text");
      sendButton.setSize("100px", "24px");
      sendButton.setEnabled(false);

      sendButton2 = new Button("Send Binary");
      sendButton2.setSize("100px", "24px");
      sendButton2.setEnabled(false);

      HTML messageInfo = new HTML("Send messages to the Gateway");
      messageInfo.setStyleName("info");
      messageInfo.setWidth("600px");

      HorizontalPanel messagePanel = new HorizontalPanel();
      messagePanel.add(msgLabel);
      messagePanel.add(msgField);
      messagePanel.add(sendButton);
      messagePanel.add(sendButton2);
      messagePanel.setSpacing(6);
      
      /* Build Log Panel */
      logTextArea = new TextArea();
      logTextArea.setCharacterWidth(60);
      logTextArea.setVisibleLines(12);

      clearButton = new Button("Clear Log");

      HTML logInfo = new HTML("Log messages");
      logInfo.setStyleName("info");
      logInfo.setWidth("600px");

      VerticalPanel logPanel = new VerticalPanel();
      logPanel.add(logTextArea);  
      logPanel.add(clearButton);
      logPanel.setSpacing(6);

      /* Add the panels to the RootPanel */
      RootPanel rootPanel = RootPanel.get();
      rootPanel.add(topInfo);
      rootPanel.add(locationPanel);
      rootPanel.add(messageInfo);
      rootPanel.add(messagePanel);
      rootPanel.add(logInfo);
      rootPanel.add(logPanel);

      /* Initialize call back handlers for UI */
      openButton.addClickHandler(new ClickHandler() {
        public void onClick(ClickEvent event) {        
          try {
            show("CONNECTING");
            connect();
          } catch (WebSocketException e) {
	    show("EXCEPTION: "+e.getMessage());
          }
	}
      });

      closeButton.addClickHandler(new ClickHandler() {
        public void onClick(ClickEvent event) {        
          ws.close(); 
          }
        }
      );
      
      sendButton.addClickHandler(new ClickHandler() {
        public void onClick(ClickEvent event) {
          String s = msgField.getText();
          ws.send(s);

          show("SENT TEXT: "+s);
         }
        }
      );
      
      sendButton2.addClickHandler(new ClickHandler() {
  		public void onClick(ClickEvent event) {
  			ws.binaryType("arraybuffer");
              String s = msgField.getText();
              ByteBuffer buf = ByteBuffer.create();
              buf.putString(s, Charset.getUTF8());
              buf.flip();
              show("SENT BINARY: "+ buf);
              ws.send(buf);
           }
         }
      );
      
      clearButton.addClickHandler(new ClickHandler() {
        public void onClick(ClickEvent event) {
            logTextArea.setText("");
          }
        }
      );
    }
    
    public void connect() throws WebSocketException {
      /* Make sure that the gateway-config.xml file has a cross-site-constraint
       * for the source-origin (e.g. either "http://127.0.0.1:8888" or "*")
       * for the echo service accepting on "ws://localhost:8001/echo". */

      // Open the connection and initialize callback handlers
      ws = new WebSocket(locField.getText());
      ws.addOpenHandler(this);
      ws.addMessageHandler(this);
      ws.addCloseHandler(this);
    }

    public void onOpen(WebSocket.OpenEvent ev) {
      show("CONNECTED");

      openButton.setEnabled(false);
      closeButton.setEnabled(true);
      sendButton.setEnabled(true);
      sendButton2.setEnabled(true);
    }

    public void onMessage(WebSocket.MessageEvent ev) {
      // Display the message received back from the server
        String dataType = ev.getDataType();
        if (dataType.contains("string"))
      	  show("RESPONSE TEXT: " + ev.getDataAsString());
        else
            show("RESPONSE BINARY: " +  ev.getDataAsByteBuffer());
    }

    public void onClose(WebSocket.CloseEvent ev) {
      show("DISCONNECTED");

      openButton.setEnabled(true);
      closeButton.setEnabled(false);
      sendButton.setEnabled(false);
      sendButton2.setEnabled(false);
    }
    
    /*
     * Utility method to log messages
     */
    public void show(String msg) {
        if (logTextArea.getText().isEmpty()) {
            logTextArea.setText(msg);
        } else {
            logTextArea.setText(msg+"\n"+logTextArea.getText());
        }
    }
}

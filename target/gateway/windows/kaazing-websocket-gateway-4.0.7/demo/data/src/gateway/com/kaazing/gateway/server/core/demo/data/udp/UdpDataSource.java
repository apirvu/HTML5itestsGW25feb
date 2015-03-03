/**
 * Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
 */

package com.kaazing.gateway.server.core.demo.data.udp;

import java.io.File;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.URI;
import java.security.SecureRandom;
import java.util.Iterator;
import java.util.Properties;
import java.util.Random;
import java.util.ServiceLoader;

import org.apache.log4j.LogManager;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.kaazing.gateway.demo.DemoServiceConfig;
import com.kaazing.gateway.demo.KaazingDOMConfigurator;

public class UdpDataSource {

	private static final String LOG4J_CONFIG_PROPERTY = "LOG4J_CONFIG";

	private static final Logger LOGGER = LoggerFactory.getLogger(UdpDataSource.class);
    private static final ServiceLoader<DemoServiceConfig> loader = ServiceLoader.load(DemoServiceConfig.class);

	/**
	 * Usage: java ... com.kaazing.gateway.server.core.demo.data.udp.UdpDataSource {URLs}
	 *        where URLs is a comma separated list of udp://... URLs
	 * @param args
	 */
	public static void main(String... args) throws Exception {
        Properties demoServiceConfigProps = null;
        if (loader != null) {
            Iterator<DemoServiceConfig> iter = loader.iterator();
            if ((iter != null) && iter.hasNext()) {
                DemoServiceConfig demoServiceConfig = iter.next();
                demoServiceConfigProps = demoServiceConfig.configure();
            }
        }

        if (demoServiceConfigProps == null) {
            demoServiceConfigProps = new Properties();
            demoServiceConfigProps.putAll(System.getProperties());
        }

        // Write to two ports by default to cater for cluster
		String remoteURLsParam = (args.length > 0) ? args[0] : "udp://localhost:50505,udp://localhost:50506";
		String[] remoteURLs = remoteURLsParam.split(",");

		String log4jConfigProperty = demoServiceConfigProps.getProperty(LOG4J_CONFIG_PROPERTY);
		if (log4jConfigProperty != null) {
			File log4jConfigFile = new File(log4jConfigProperty);
			KaazingDOMConfigurator configurator = new KaazingDOMConfigurator(demoServiceConfigProps);
			configurator.doConfigure(log4jConfigFile.toURI().toURL(), LogManager.getLoggerRepository());
		}

		SocketAddress[] remoteAddresses = new SocketAddress[remoteURLs.length];
		for (int i=0; i<remoteURLs.length; i++) {
		    URI remoteURI = URI.create(remoteURLs[i]);
		    remoteAddresses[i] = new InetSocketAddress(remoteURI.getHost(), remoteURI.getPort());
		    System.out.print((i==0 ? "Sending UDP data to " : ",") + remoteURI);
		}
		System.out.println();

		DatagramSocket socket = new DatagramSocket();
		Random random = new SecureRandom();
		while (true) {
			String data = STORIES[random.nextInt(STORIES.length)];
			byte[] buf = data.getBytes();
			for (SocketAddress remoteAddress : remoteAddresses) {
    			DatagramPacket packet = new DatagramPacket(buf, buf.length, remoteAddress);
    			socket.send(packet);
    			LOGGER.debug("Sending UDP data to " + remoteAddress + ": " + data);
			}
			Thread.sleep(2000L);
		}
	}

	private static final String[] STORIES = new String[] {
		"Ranking the top 25 players in baseball",
		"Spacecraft blasts off in search of 'Earths'",
		"Ancient marvels, sun-splashed islands of Greece",
		"Why we're sleeping less",
		"Unemployed place their bets on casino jobs",
		"Crews save homes from wildfire",
		"Spare us, say bowlers to planned tax",
		"Proof of ancient Malaysian civilization found",
		"Papua New Guinea gets first conservation area",
		"Nation's rappers down to last two samples"
	};
}

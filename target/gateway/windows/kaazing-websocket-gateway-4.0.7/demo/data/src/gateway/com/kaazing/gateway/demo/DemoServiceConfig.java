/**
 * Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
 */

package com.kaazing.gateway.demo;

import java.util.Properties;

public interface DemoServiceConfig {
    /**
     * Config the demo services with the correct environment
     * @return the properties that make up the environment for the demo services
     */
    Properties configure();
}

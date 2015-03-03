You can launch a gateway:

- As a cluster of single gateway: mvn clean pre-integration-test -Pcluster.
-- To launch multiple cluster gateways, you may (but not necessarily) need to run the above command 
   with overridden paramaters about:
   1) jmx port (to avoid port conflicts)
   2) cluster connect url (hazelcast)
   3) cluster accept url (hazelcast)
   4) cluster node hostname
   5) cluster node IP address
   6) cluster hostname (the endpoint for client to connect to)
   7) gateway pid file (unless you run each gateway in separate target folders)
   
  Examples:
    * Running single gateway with the default cluster node hostname: node.kaazing.me:
    project-dir>    mvn clean pre-integration-test -Pcluster -Dcluster.hostname.prop=gateway.kaazing.me
    
    * Running two gateways as cluster in single machine of two IP addresses:
    project-copy1>  mvn clean pre-integration-test -Pcluster -Dcluster.hostname.prop=gateway.cluster.local \
                    -Dnode.hostname.prop=node1.cluster.local -Dnode.jmxport.prop=2121 -Dnode.ip.prop=127.0.0.1 \
                    -Dnode.cluster.accept.prop=tcp://10.0.67.139:5351 -Dnode.cluster.connect.prop=tcp://10.0.67.139:5352 \
    project-copy2>  mvn clean pre-integration-test -Pcluster -Dcluster.hostname.prop=gateway.cluster.local \
                    -Dnode.hostname.prop=node2.cluster.local-Dnode.jmxport.prop=2122 -Dnode.ip.prop=127.0.0.1 \
                    -Dnode.cluster.accept.prop=tcp://10.0.67.139:5352 -Dnode.cluster.connect.prop=tcp://10.0.67.139:5351 \

- As a non-cluster single gateway: mvn clean pre-integration-test
(Using old gateway-config-client.itests.xml)

By default, the gateway will run as a non-cluster.
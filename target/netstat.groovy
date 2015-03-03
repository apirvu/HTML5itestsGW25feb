def fetchNetStat = {
    if(shouldSkip()) {
        log.info "Skipping netstat"
        return;
    }

    def cmd = "netstat -an";
    log.info("Executing: "+cmd);

    def text = cmd.execute().text;
    def failed = false;
    text.eachLine { line ->
        if (line.contains("LISTEN")) {
            // We can write this better - but linux/mac use .8000; windows of course uses something different :8000
            if (line.contains(".2020") || line.contains(".8000") || line.contains(".8001") || line.contains(".9000") || line.contains(".9001") ||
                line.contains(".8010") || line.contains(".8011") ||
                line.contains(":2020") || line.contains(":8000") || line.contains(":8001") || line.contains(":9000") || line.contains(":9001") ||
                line.contains(":8010") || line.contains(":8011")) {

                log.info "> ${line}";
                failed = true;
            }
            else {
                log.info "  ${line}";
            }
        }
    }

    if (failed) {
        log.error("netstat reported bound port - see entries marked above");
        fail("netstat reported bound port - see entries marked above");
    }
}

def shouldSkip() {
    try {
        if(project.properties.getProperty("skipNetstat").equals("true")) {
            return true;
        } else {
            return false;
        }
    } catch(Exception e) {
        //may happen if we run this outside maven project or if skipNetstat property is not defined.
        return false;
    }
}

fetchNetStat()

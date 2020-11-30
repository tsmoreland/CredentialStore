module moreland.win32.credentialstore.cli {
    requires java.logging;
    requires moreland.win32.credentialstore;
    
    requires spring.beans;
    requires spring.context;
    requires spring.core;
    requires spring.aop;
    requires spring.expression;

    requires commons.logging;
    opens moreland.win32.credentialstore.cli to spring.core;

    exports moreland.win32.credentialstore.cli;
}

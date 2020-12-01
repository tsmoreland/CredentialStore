module moreland.win32.credentialstore.cli {
    requires java.logging;
    requires transitive moreland.win32.credentialstore;
    
    requires spring.beans;
    requires spring.context;
    requires spring.core;
    requires spring.aop;
    requires spring.expression;

    requires commons.logging;
    opens moreland.win32.credentialstore.cli to spring.core, spring.beans, spring.aop;
    exports moreland.win32.credentialstore.cli.internal to spring.core, spring.context, spring.aop, spring.beans;

    exports moreland.win32.credentialstore.cli;
}

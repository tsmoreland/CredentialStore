module moreland.win32.credentialstore.cli {
    requires moreland.win32.credentialstore;
    
    requires transitive spring.beans;
    requires transitive spring.context;
    requires transitive spring.core;

    exports moreland.win32.credentialstore.cli;
}

package moreland.win32.credentialstore.cli;

import java.io.PrintStream;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Import;
import org.springframework.context.annotation.Scope;

import moreland.win32.credentialstore.ServiceConfiguration;
import moreland.win32.credentialstore.Win32CredentialManager;
import moreland.win32.credentialstore.cli.internal.ConsolePasswordReaderFacade;
import moreland.win32.credentialstore.cli.internal.CredentialExecutor;
import moreland.win32.credentialstore.cli.internal.PasswordReaderFacade;
import moreland.win32.credentialstore.cli.internal.Win32CredentialExecutor;

@Configuration
@SuppressWarnings({"java:S125"})
//@ComponentScan({"moreland.win32.credentialstore"})
@Import(ServiceConfiguration.class) // -- if using explicit import rather than ComponentScan
public class ApplicationConfiguration {
    
    @Autowired
    ServiceConfiguration serviceConfiguration;

    @Bean(name = "credentialExecutor")
    @Scope(value=BeanDefinition.SCOPE_SINGLETON)
    @SuppressWarnings({"java:S106"})
    CredentialExecutor getCredentialExecutor() {
        return new Win32CredentialExecutor(new Win32CredentialManager(), System.out, getPasswordReaderFacade());
    }

    @Bean(name = "passwordReaderFacade")
    @Scope(value=BeanDefinition.SCOPE_SINGLETON)
    PasswordReaderFacade getPasswordReaderFacade() {
        return new ConsolePasswordReaderFacade();
    }

    @Bean(name = "printStream")
    @Scope(value = BeanDefinition.SCOPE_SINGLETON)
    @SuppressWarnings({"java:S106"})
    PrintStream getPrintStream() {
        return System.out;
    }

}

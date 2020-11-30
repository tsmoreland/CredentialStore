package moreland.win32.credentialstore.cli;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Import;

import moreland.win32.credentialstore.ServiceConfiguration;
import moreland.win32.credentialstore.cli.internal.ConsolePasswordReaderFacade;
import moreland.win32.credentialstore.cli.internal.CredentialExecutor;
import moreland.win32.credentialstore.cli.internal.PasswordReaderFacade;
import moreland.win32.credentialstore.cli.internal.Win32CredentialExecutor;

@Configuration
@Import(ServiceConfiguration.class)
public class ApplicationConfiguration {
    
    @Autowired
    ServiceConfiguration serviceConfiguration;

    @Bean(name = "credentialExecutor")
    @SuppressWarnings({"java:S106"})
    CredentialExecutor getCredentialExecutor() {
        return new Win32CredentialExecutor(serviceConfiguration.getCredentialManager(), System.out, getPasswordReaderFacade());
    }

    @Bean(name = "passwordReaderFacade")
    PasswordReaderFacade getPasswordReaderFacade() {
        return new ConsolePasswordReaderFacade();
    }

}

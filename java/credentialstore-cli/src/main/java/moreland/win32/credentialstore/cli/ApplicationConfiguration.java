//
// Copyright © 2020 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
package moreland.win32.credentialstore.cli;

import java.io.PrintStream;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Scope;

import moreland.win32.credentialstore.ServiceConfiguration;

@Configuration
@SuppressWarnings({"java:S125"})
@ComponentScan({"moreland.win32.credentialstore"})
public class ApplicationConfiguration {
    
    @Autowired
    ServiceConfiguration serviceConfiguration;

    /*
     * exmamples of how to handle this without @Service / @Component annotations
     * will remove this comment once fully working
    @Bean(name = "credentialExecutor")
    @Scope(value=BeanDefinition.SCOPE_SINGLETON)
    @SuppressWarnings({"java:S106"})
    CredentialExecutor getCredentialExecutor() {
        return new Win32CredentialExecutor(
            serviceConfiguration.getCredentialManager(),
            System.out, 
            getPasswordReaderFacade());
    }

    @Bean(name = "passwordReaderFacade")
    @Scope(value=BeanDefinition.SCOPE_SINGLETON)
    PasswordReaderFacade getPasswordReaderFacade() {
        return new ConsolePasswordReaderFacade();
    }
    */

    @Bean(name = "printStream")
    @Scope(value = BeanDefinition.SCOPE_SINGLETON)
    @SuppressWarnings({"java:S106"})
    PrintStream getPrintStream() {
        return System.out;
    }

}

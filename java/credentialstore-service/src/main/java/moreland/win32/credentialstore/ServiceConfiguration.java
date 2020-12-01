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
package moreland.win32.credentialstore;

import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Scope;

import moreland.win32.credentialstore.converters.CredentialConverter;
import moreland.win32.credentialstore.converters.Win32CredentialConverter;
import moreland.win32.credentialstore.internal.NativeInteropBridge;
import moreland.win32.credentialstore.internal.Win32NativeInteropBridge;

@Configuration
//@ComponentScan({"moreland.win32.credentialstore"})
@SuppressWarnings({"java:S125"})
public class ServiceConfiguration {
    
    @Bean(name = "credentialManager")
    @Scope(value=BeanDefinition.SCOPE_PROTOTYPE) // no particular need, just leaving it here for sake of reference
    public CredentialManager getCredentialManager() {
        return new Win32CredentialManager(getNativeInteropBridge(), getCredentialConverter());
    }

    @Bean(name = "nativeInteropBean")
    @Scope(value=BeanDefinition.SCOPE_SINGLETON)
    NativeInteropBridge getNativeInteropBridge() {
        return new Win32NativeInteropBridge();
    }

    @Bean(name = "credentialConverter")
    @Scope(value=BeanDefinition.SCOPE_PROTOTYPE)
    CredentialConverter getCredentialConverter() {
        return new Win32CredentialConverter();
    }
}

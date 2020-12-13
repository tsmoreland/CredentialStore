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

import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.AnnotationConfigApplicationContext;

import moreland.win32.credentialstore.cli.internal.CredentialExecutor;

public class Application {

    @SuppressWarnings({"java:S106"})
    public static void main(String[] args) {
        if (args.length < 1) {
            System.out.println("usage: credentialstore.cli <verb> (optional arguments)");
            return;
        }

        var logger = LoggerFactory.getLogger(Application.class);

        try (var context = new AnnotationConfigApplicationContext(ApplicationConfiguration.class)) {
            var executor = context.getBean("credentialExecutor", CredentialExecutor.class);
            var operation = executor.getOperation(args[0]).orElse(arguments -> false);

            var operationArgs = IntStream
                .range(1, args.length)
                .mapToObj(i -> args[i])
                .collect(Collectors.toList());

            if (!operation.process(operationArgs)) {
                logger.warn(String.format("Operation '%s' failed", args[0]));
            }
        } catch (Exception ex) {
            logger.error(String.format("Error occurred building the application context: %s", ex.getMessage()), ex);
        }

    }    
}

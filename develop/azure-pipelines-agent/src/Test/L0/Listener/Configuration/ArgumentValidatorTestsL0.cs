// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.Services.Agent.Listener.Configuration;
using Xunit;

namespace Microsoft.VisualStudio.Services.Agent.Tests.Listener.Configuration
{
    public sealed class ArgumentValidatorTestsL0
    {
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "ArgumentValidator")]
        public void ServerUrlValidator()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Assert.True(Validators.ServerUrlValidator("http://servername"));
                Assert.False(Validators.ServerUrlValidator("Fail"));
                Assert.False(Validators.ServerUrlValidator("ftp://servername"));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "ArgumentValidator")]
        public void AuthSchemeValidator()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Assert.True(Validators.AuthSchemeValidator("pat"));
                Assert.False(Validators.AuthSchemeValidator("Fail"));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "ArgumentValidator")]
        public void NonEmptyValidator()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Assert.True(Validators.NonEmptyValidator("test"));
                Assert.False(Validators.NonEmptyValidator(string.Empty));
            }
        }


        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "ArgumentValidator")]
        [Trait("SkipOn", "darwin")]
        [Trait("SkipOn", "linux")]
        public void WindowsLogonAccountValidator()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Assert.False(Validators.NTAccountValidator(string.Empty));
                Assert.True(Validators.NTAccountValidator("NT AUTHORITY\\LOCAL SERVICE"));
            }
        }
    }
}
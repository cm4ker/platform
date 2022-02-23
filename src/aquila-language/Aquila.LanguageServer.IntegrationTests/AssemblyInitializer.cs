// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquila.LanguageServer.IntegrationTests
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize()]
        public static void AssemblyInitialize(TestContext testContext)
        {
            //BicepDeploymentsInterop.Initialize();
        }
    }
}
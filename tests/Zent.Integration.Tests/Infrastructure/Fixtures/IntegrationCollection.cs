using Xunit;

namespace Zent.Integration.Tests.Infrastructure.Fixtures;

[CollectionDefinition("Integration")]
public class IntegrationCollection : IClassFixture<TestHostFixture>;
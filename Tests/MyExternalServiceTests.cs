using Libs;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class MyExternalServiceTests
    {
        private Mock<HttpMessageHandler> handlerMock;
        private MyExternalService service;

        [SetUp]
        public void Setup()
        {
            this.handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            HttpResponseMessage result = new HttpResponseMessage();

            this.handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns(Task.FromResult(result))
            .Verifiable()
            ;

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://www.code4it.dev/")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            mockHttpClientFactory.Setup(_ => _.CreateClient("ext_service")).Returns(httpClient);

            this.service = new MyExternalService(mockHttpClientFactory.Object);
        }

        [Test]
        public async Task TestQueryString()
        {
            await service.DeleteObject("my-name");

            //Assert
            handlerMock.Verify(r => r.RequestUri.Query.Contains("my-name"));
        }
    }



    public static class MockExtension
    {

        public static void Verify(this Mock<HttpMessageHandler> mock, Func<HttpRequestMessage, bool> match)
        {
            mock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req => match(req)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
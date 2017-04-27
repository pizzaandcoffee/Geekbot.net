using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Geekbot.net.Lib;
using Geekbot.net.Modules;
using Moq;
using RestSharp;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestCat()
        {
            // setup
            var catClient = new Mock<ICatClient>(MockBehavior.Strict);
            var client = new Mock<IRestClient>(MockBehavior.Strict);
            catClient.Setup(cc => cc.Client).Returns(client.Object);
            var response = new Mock<IRestResponse<Cat.CatResponse>>(MockBehavior.Strict);
            var resultData = new Cat.CatResponse {file = "unit-test"};
            response.SetupGet(r => r.Data).Returns(resultData);
            Console.WriteLine(resultData.file);
            var request = new Mock<IRestRequest>(MockBehavior.Strict);
            Func<IRestRequest> requestFunc = () => request.Object;
            client.Setup(c => c.Execute<Cat.CatResponse>(request.Object)).Returns(response.Object);
            Mock<AsyncReplier> asyncReplier = new Mock<AsyncReplier>(MockBehavior.Strict);
            asyncReplier.Setup(ar => ar.ReplyAsyncInt(resultData.file)).Returns(Task.FromResult(true)).Verifiable();

            // execute
            //var cat = new Cat(catClient.Object, requestFunc, asyncReplier.Object);
            //await cat.Say();

            // validate
            //asyncReplier.Verify();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(33, 4561)]
        [InlineData(79, 449702)]
        [InlineData(79, 449702 + 1)]
        public void TestLevel(int expectedIndex, int experience)
        {
            var index = LevelCalc.GetLevelAtExperience(experience);
            index.Should().Be(expectedIndex);
        }

    }
}
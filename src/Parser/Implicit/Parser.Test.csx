using Moq;
using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
using System;
using Parseus.Lexer;
using Xunit;

namespace Parseus.Parser.Implicit.Tests
{
    public class BaseParserTests
    {
        private BaseParser.CancelationToken _token;
        private Mock<IParserContext> _mockContext;
        private BaseParserContext _ctx;

        public BaseParserTests()
        {
            _token = new BaseParser.CancelationToken();
            _mockContext = new Mock<IParserContext>();
            _ctx = new BaseParserContext(_mockContext.Object, _token);
        }

        [Fact]
        public void Repeat_StopsOnFailure()
        {
            int counter = 0;

            _mockContext.SetupSequence(c => c.pos)
                        .Returns(0).Returns(1).Returns(2);
            _mockContext.SetupSequence(c => c.HasMoreTokens())
                        .Returns(true).Returns(false);

            BaseParser.Repeat(_ctx, ctx =>
            {
                counter++;
                if (counter == 2) ctx.state.Flag();
            });

            Assert.Equal(2, counter);
            Assert.False(_ctx.state.Ok);
        }

        [Fact]
        public void Opt_AlwaysResetsState()
        {
            _ctx.state.Flag(); // Make sure it doesn't run if not OK
            BaseParser.Opt(_ctx, ctx => throw new Exception("Should not run"));
            _ctx.state.Reset();
            BaseParser.Opt(_ctx, ctx => ctx.state.Flag());

            Assert.True(_ctx.state.Ok); // Should always reset
        }

        [Fact]
        public void Alt_ResolvesFirstSuccessfulAlternative()
        {
            bool firstCalled = false, secondCalled = false;
            _mockContext.Setup(c => c.pos).Returns(0);

            BaseParser.Alt(_ctx,
                ctx => { firstCalled = true; ctx.state.Flag(); },
                ctx => { secondCalled = true; });

            Assert.True(firstCalled);
            Assert.True(secondCalled);
            Assert.True(_ctx.state.Ok);
        }

        [Fact]
        public void Literal_MatchesExpectedValue()
        {
            var token = new TokenElement("text", "hello", 0,0);
            _mockContext.Setup(c => c.HasMoreTokens()).Returns(true);
            _mockContext.Setup(c => c.PeekToken()).Returns(token);
            _mockContext.Setup(c => c.Consume()).Returns(token);

            BaseParser.Literal(_ctx, "hello", out bool success);

            Assert.True(success);
            Assert.True(_ctx.state.Ok);
        }

        [Fact]
        public void Literal_FailsOnMismatch()
        {
            var token = new Token { Value = "bye" };
            _mockContext.Setup(c => c.HasMoreTokens()).Returns(true);
            _mockContext.Setup(c => c.PeekToken()).Returns(token);

            BaseParser.Literal(_ctx, "hello", out bool success);

            Assert.False(success);
            Assert.False(_ctx.state.Ok);
        }

        [Fact]
        public void Token_MatchesTokenType()
        {
            var token = new Token { Token = "ID", Value = "x" };
            _mockContext.Setup(c => c.HasMoreTokens()).Returns(true);
            _mockContext.Setup(c => c.PeekToken()).Returns(token);
            _mockContext.Setup(c => c.Consume()).Returns(token);

            BaseParser.Token(_ctx, "ID", out string value);

            Assert.Equal("x", value);
            Assert.True(_ctx.state.Ok);
        }

        [Fact]
        public void Node_ParsesWithParser()
        {
            var parser = new BaseParser.Parser<DummyNode>((ctx, node) => { node.Data = "test"; });
            BaseParser.Node(_ctx, parser, out DummyNode node);

            Assert.Equal("test", node.Data);
            Assert.True(_ctx.state.Ok);
        }

        [Fact]
        public void Node_FailsAndResetsPosition()
        {
            var parser = new BaseParser.Parser<DummyNode>((ctx, node) => ctx.state.Flag());
            _mockContext.SetupProperty(c => c.pos, 0);

            BaseParser.Node(_ctx, parser, out DummyNode node);

            Assert.False(_ctx.state.Ok);
            Assert.Equal(0, _mockContext.Object.pos);
        }

        public class DummyNode
        {
            public string Data { get; set; }
        }
    }
}

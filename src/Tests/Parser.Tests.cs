using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
namespace Parseus.Tests;

using Xunit;

using Parseus.Lexer;

using System.Linq;

public class Parser_Tests {
	[Fact]
	public void ParserTest_ParseRepeat() {
		var parser = new TestParser();
		var script = parser.Parse("let x 1 2 3\nlet y 4 5 6");
		
		var varX = script.variableStatements.FirstOrDefault(v => v.identifier == "x")!;
		var varY = script.variableStatements.FirstOrDefault(v => v.identifier == "y")!;
		
		Assert.Equal("x", varX.identifier);
		Assert.Equal("y", varY.identifier);
		Assert.Equal(new List<string>{"1","2","3"}, varX.items);
		Assert.Equal(new List<string>{"4","5","6"}, varY.items);
	}
}
namespace parser;

public static class Program {
    public static void Main(string[] args) {

        string[] src = {
            "TestVal := (15 * 14) + -4;",
            "TestVal2 := \"Hello World!\"",
            "TestBool := (5 == 5)"
        };

        //Generate Chunk/AST

        Chunk prgm = AstGen.GenerateAst(Lexer.LexIt(Lexer.preprocessor(src)));
        Console.WriteLine(prgm.GenerateSrcCode());


        Chunk prgm2 = new(
            new() {
                new AssignStat("TestVal", new MathExp(new MathExp(new Value().setValue(15), Operator.MUL, new Value().setValue(14)).SetParenties(ParenType.round), Operator.ADD, new Value().setValue(-4))),
                new AssignStat("TestVal2", new Value().setValue("Hello World!")),
                new AssignStat("TestBool", new LogicExp(new Value().setValue(5), Operator.EQL, new
                    Value().setValue(5)).SetParenties(ParenType.round))
            });
        Console.WriteLine(prgm2.GenerateSrcCode());
        //
        Console.WriteLine();
    }
}
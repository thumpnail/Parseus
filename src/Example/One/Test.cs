﻿using Parseus.Parser.BasicParsers.Test;
namespace Parseus;

public class Test {
    public static void test_main(string[] args) {
        var iniParser = new TestBaseParser().Parse("""
                                                  hello = (
                                                        one : 1
                                                        two : 2
                                                        tree : 3
                                                        abc : 23
                                                        efefzuzwvefpupvefuefv:26347263817
                                                  )
                                                  """);
    }
}

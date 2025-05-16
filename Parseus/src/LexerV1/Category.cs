<<<<<<< Updated upstream
﻿namespace Parseus.Lexer;

struct Category<T> {
    public T token;
    public bool isSkipable = false;
    public string[] literals;
    public Category(T token, params string[] literals) {
        this.token = token;
        this.literals = literals;
    }
    public Category(T token, bool skipable, params string[] literals) {
        this.token = token;
        this.literals = literals;
        this.isSkipable = skipable;
    }
=======
﻿namespace Parseus.Lexer;

struct Category<T> {
    public T token;
    public bool isSkipable = false;
    public string[] literals;
    public Category(T token, params string[] literals) {
        this.token = token;
        this.literals = literals;
    }
    public Category(T token, bool skipable, params string[] literals) {
        this.token = token;
        this.literals = literals;
        this.isSkipable = skipable;
    }
>>>>>>> Stashed changes
}
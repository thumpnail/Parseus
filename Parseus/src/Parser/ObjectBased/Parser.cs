<<<<<<< Updated upstream
﻿namespace Parseus.Parser.ObjectBased;

public abstract class Parser {
    public interface EbnfNode {
        public bool Parse();
    }
    public class Token: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Literal: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Opt: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Repeat: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Alt: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Node: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
=======
﻿namespace Parseus.Parser.ObjectBased;

public abstract class Parser {
    public interface EbnfNode {
        public bool Parse();
    }
    public class Token: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Literal: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Opt: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Repeat: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Alt: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
    public class Node: EbnfNode {
        public bool Parse() {
            throw new NotImplementedException();
        }
    }
>>>>>>> Stashed changes
}
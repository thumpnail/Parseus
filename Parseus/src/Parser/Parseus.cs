using Sprache;

namespace Parseus.Parser;

public static class Parseus<T> where T : Enum {
	public static ParserContext<T> parserContext;
	public class ParserContext<T> where T : Enum {public ParserContext() {}}

	public class ParserClass<T> where T : Enum {
		public List<RuleClass<T>> rules;
		public ParserClass(params RuleClass<T>[] rules) {
			this.rules = rules.ToList();
		}
	}
	public class RuleClass<T> where T : Enum {
		public string name;
		public IClass<T>[] childs;
		public RuleClass(string name, params IClass<T>[] childs) {
			this.childs = childs;
			this.name = name;
		}
	}

	public interface IClass<T> where T : Enum {
		public static void Parse() {
			throw new NotImplementedException();
		}
	}
	public class RuleRefClass<T> : IClass<T> where T : Enum {
		public string name;
		public RuleRefClass(string name){
			this.name = name;
		}
	}
	public class AltClass<T> : IClass<T> where T : Enum {
		public List<IClass<T>> branches;
		public AltClass(params IClass<T>[] branches) {
			this.branches = branches.ToList();
		}
	}
	public class GroupClass<T> : IClass<T> where T : Enum {
		public List<IClass<T>> childs;
		public GroupClass(params IClass<T>[] childs) {
			this.childs = childs.ToList();
		}
	}
	public class RepeatClass<T> : IClass<T> where T : Enum {
		public List<IClass<T>> childs;
		public RepeatClass(params IClass<T>[] childs) {
			this.childs = childs.ToList();
		}
	}
	public class OptionalClass<T> : IClass<T> where T : Enum {
		public List<IClass<T>> childs;
		public OptionalClass(params IClass<T>[] childs) {
			this.childs = childs.ToList();
		}
	}
	public class TokenClass<T> : IClass<T> where T : Enum {
		public string id;
		public string value;
		public T token;
		public RuleRefClass<T> parsable;
		public TokenClass(string value) {
			this.value = value;
		}
		public TokenClass(T token) {
			this.token = token;
		}
		public TokenClass(string id, T token) {
			this.token = token;
			this.id = id;
		}
		public TokenClass(string id, RuleRefClass<T> parsable, T token) {
			this.parsable = parsable;
			this.token = token;
			this.id = id;
		}
	}
	public static ParserClass<T> Parser(params RuleClass<T>[] rules) {
		return new ParserClass<T>(rules);
	}
	public static RuleClass<T> Rule(string name, params IClass<T>[] childs) {
		return new RuleClass<T>(name, childs);
	}
	public static RuleRefClass<T> RuleRef(string name) {
		return new(name);
	}
	public static AltClass<T> Alt(params IClass<T>[] branches) {
		return new(branches);
	}
	public static GroupClass<T> Branch(params IClass<T>[] branches) {
		return new(branches);
	}
	public static GroupClass<T> Group(params IClass<T>[] childs) {
		return new(childs);
	}
	public static RepeatClass<T> Repeat(params IClass<T>[] childs) {
		return new(childs);
	}
	public static OptionalClass<T> Optional(params IClass<T>[] childs) {
		return new(childs);
	}
	public static TokenClass<T> Token(string value) {
		return new(value);
	}
	public static TokenClass<T> Parse(string id, T token) {
		return new(token);
	}
	public static TokenClass<T> Parse(string id, RuleRefClass<T> parsable, T token) {
		return new(token);
	}

	///////////////////////////////////
	/// AST ///
	///////////////////////////////////
	
	public class Node<T> where T : Enum {
		//default keys
		public string id;
		public string type;
		public T token;
		public string value;

		public List<Node<T>> nodes;
		public Node(string id = null, string type = null, T token = default, string value = null, params Node<T>[] nodes) {
			this.id = id;
			this.type = type;
			this.token = token;
			this.value = value;
			this.nodes = nodes.ToList();
		}
	}
}
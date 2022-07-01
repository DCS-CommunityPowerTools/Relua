using Relua.Deserialization.Expressions;
using System.Collections.Generic;




namespace Relua {

	/// <summary>
	/// Struct representing a Lua operator (unary and/or binary), along with
	/// its precedence and associativity.
	/// </summary>
	public struct OperatorInfo {

		/// <summary>
		/// Dictionary of builtin Lua operators.
		/// </summary>
		public static readonly Dictionary<string, OperatorInfo> LuaOperators = new Dictionary<string, OperatorInfo> {
			["or"] = new OperatorInfo(true, false, "or", 2, false),
			["and"] = new OperatorInfo(true, false, "and", 3, false),
			["<"] = new OperatorInfo(true, false, "<", 4, false),
			["<="] = new OperatorInfo(true, false, "<=", 4, false),
			[">"] = new OperatorInfo(true, false, ">", 4, false),
			[">="] = new OperatorInfo(true, false, ">=", 4, false),
			["~="] = new OperatorInfo(true, false, "~=", 4, false),
			["=="] = new OperatorInfo(true, false, "==", 4, false),
			[".."] = new OperatorInfo(true, false, "..", 5, true),
			["+"] = new OperatorInfo(true, false, "+", 6, false),
			["-"] = new OperatorInfo(true, true, "-", 6, false),
			["*"] = new OperatorInfo(true, false, "*", 7, false),
			["/"] = new OperatorInfo(true, false, "/", 7, false),
			["%"] = new OperatorInfo(true, false, "%", 7, false),
			["not"] = new OperatorInfo(false, true, "not", 8, false),
			["#"] = new OperatorInfo(false, true, "#", 8, false),
			["^"] = new OperatorInfo(true, false, "^", 9, true),
		};


		public static bool BinaryOperatorExists(string key) {
			return LuaOperators.ContainsKey(key) && LuaOperators[key].IsBinary;
		}


		public static OperatorInfo? FromToken(Token tok) {
			if (tok.IsEOF()) {
				return null;
			}

			if (LuaOperators.TryGetValue(tok.Value, out OperatorInfo op)) {
				return op;
			}

			return null;
		}




		public enum OperatorType {
			Binary,
			Unary
		}




		public string TokenValue;
		public int Precedence;
		public bool RightAssociative;
		public bool IsBinary;
		public bool IsUnary;


		public OperatorInfo(bool is_binary, bool is_unary, string value, int precedence, bool right_assoc) {
			this.IsBinary = is_binary;
			this.IsUnary = is_unary;
			this.TokenValue = value;
			this.Precedence = precedence;
			this.RightAssociative = right_assoc;
		}




		public BinaryExpression.OpType? BinaryOpType {
			get {
				switch (this.TokenValue) {
					case "or": return BinaryExpression.OpType.Or;
					case "and": return BinaryExpression.OpType.And;
					case "<": return BinaryExpression.OpType.LessThan;
					case ">": return BinaryExpression.OpType.GreaterThan;
					case "<=": return BinaryExpression.OpType.LessOrEqual;
					case ">=": return BinaryExpression.OpType.GreaterOrEqual;
					case "~=": return BinaryExpression.OpType.NotEqual;
					case "==": return BinaryExpression.OpType.Equal;
					case "..": return BinaryExpression.OpType.Concat;
					case "+": return BinaryExpression.OpType.Add;
					case "-": return BinaryExpression.OpType.Subtract;
					case "*": return BinaryExpression.OpType.Multiply;
					case "/": return BinaryExpression.OpType.Divide;
					case "%": return BinaryExpression.OpType.Modulo;
					case "^": return BinaryExpression.OpType.Power;
					default: return null;
				}
			}
		}


		public UnaryExpression.OpType? UnaryOpType {
			get {
				switch (this.TokenValue) {
					case "-": return UnaryExpression.OpType.Negate;
					case "not": return UnaryExpression.OpType.Invert;
					case "#": return UnaryExpression.OpType.Length;
					default: return null;
				}
			}
		}

	}

}

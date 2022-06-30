using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Literals {

	/// <summary>
	/// String literal expression. Note that `Value` will contain the real value
	/// of the string, with all escape sequences interpreted accordingly. Single
	/// quote string literals are also supported.
	/// 
	/// ```
	/// "Hello, world!"
	/// 'Hello, world!'
	/// ```
	/// </summary>
	public class StringLiteral : Node, IExpression {
		public static void Quote(IndentAwareTextWriter s, string str) {
			s.Write('"');
			for (var i = 0; i < str.Length; i++) {
				var c = str[i];
				if (c == '\n') s.Write("\\n");
				else if (c == '\t') s.Write("\\t");
				else if (c == '\r') s.Write("\\r");
				else if (c == '\a') s.Write("\\a");
				else if (c == '\b') s.Write("\\b");
				else if (c == '\f') s.Write("\\f");
				else if (c == '\v') s.Write("\\v");
				else if (c == '\\') s.Write("\\\\");
				else if (c == '"') s.Write("\\\"");
				else if (!c.IsASCIIPrintable()) s.Write($"\\{((int)c).ToString("D3")}");
				else s.Write(c);
			}
			s.Write('"');
		}

		public string Value;

		public override void Write(IndentAwareTextWriter writer) {
			Quote(writer, Value);
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

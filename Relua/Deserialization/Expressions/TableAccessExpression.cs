using Relua;
using Relua.Deserialization.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Table access assignable expression. Note that this can be used with any
	/// Lua type and not just  tables due to metatables. When writing, an
	/// attempt will be made to use syntax sugar if possible - assuming that
	/// the index is a literal string which is a valid identifier, dot-style
	/// access will be used as opposed to the verbose bracket syntax
	/// (i.e. `a.b` instead of `a["b"]`).
	/// 
	/// ```
	/// table.val
	/// ("abc").x
	/// _G["abc"]
	/// ```
	/// </summary>
	public class TableAccessExpression : Node, IExpression, IAssignable {
		public IExpression Table;
		public IExpression Index;

		private bool GetIdentifierAccessChain(StringBuilder s, bool is_method_access_top_level = false) {
			if (Table is TableAccessExpression) {
				if (!((TableAccessExpression)Table).GetIdentifierAccessChain(s)) return false;
			} else if (Table is VariableExpression) {
				s.Append(((VariableExpression)Table).Name);
			} else return false;

			if (is_method_access_top_level) {
				s.Append(":");
			} else s.Append(".");

			if (Index is StringLiteral) {
				var lit = (StringLiteral)Index;
				if (!lit.Value.IsIdentifier()) return false;

				s.Append(lit.Value);
			} else return false;

			return true;
		}

		public string GetIdentifierAccessChain(bool is_method_access) {
			var s = new StringBuilder();
			if (!GetIdentifierAccessChain(s, is_method_access)) return null;
			return s.ToString();
		}

		public void WriteDotStyle(IndentAwareTextWriter writer, string index) {
			if (Table is StringLiteral) writer.Write("(");
			Table.Write(writer);
			if (Table is StringLiteral) writer.Write(")");
			writer.Write(".");
			writer.Write(index);
		}

		public void WriteGenericStyle(IndentAwareTextWriter writer) {
			if (Table is StringLiteral) writer.Write("(");
			Table.Write(writer);
			if (Table is StringLiteral) writer.Write(")");
			writer.Write("[");
			Index.Write(writer);
			writer.Write("]");
		}

		public override void Write(IndentAwareTextWriter writer) {
			if (Index is StringLiteral && ((StringLiteral)Index).Value.IsIdentifier()) {
				WriteDotStyle(writer, ((StringLiteral)Index).Value);
			} else {
				WriteGenericStyle(writer);
			}
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

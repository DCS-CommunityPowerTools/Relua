using Relua.Deserialization.Literals;
using System.Text;




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
			if (this.Table is TableAccessExpression) {
				if (!((TableAccessExpression)this.Table).GetIdentifierAccessChain(s)) {
					return false;
				}
			} else if (this.Table is VariableExpression) {
				s.Append(((VariableExpression)this.Table).Name);
			} else {
				return false;
			}

			if (is_method_access_top_level) {
				s.Append(":");
			} else {
				s.Append(".");
			}

			if (this.Index is StringLiteral) {
				StringLiteral lit = (StringLiteral)this.Index;
				if (!lit.Value.IsIdentifier()) {
					return false;
				}

				s.Append(lit.Value);
			} else {
				return false;
			}

			return true;
		}


		public string GetIdentifierAccessChain(bool is_method_access) {
			StringBuilder s = new StringBuilder();
			if (!this.GetIdentifierAccessChain(s, is_method_access)) {
				return null;
			}

			return s.ToString();
		}


		public void WriteDotStyle(IndentAwareTextWriter writer, string index) {
			if (this.Table is StringLiteral) {
				writer.Write("(");
			}

			this.Table.Write(writer);
			if (this.Table is StringLiteral) {
				writer.Write(")");
			}

			writer.Write(".");
			writer.Write(index);
		}


		public void WriteGenericStyle(IndentAwareTextWriter writer) {
			if (this.Table is StringLiteral) {
				writer.Write("(");
			}

			this.Table.Write(writer);
			if (this.Table is StringLiteral) {
				writer.Write(")");
			}

			writer.Write("[");
			this.Index.Write(writer);
			writer.Write("]");
		}


		public override void Write(IndentAwareTextWriter writer) {
			if (this.Index is StringLiteral && ((StringLiteral)this.Index).Value.IsIdentifier()) {
				this.WriteDotStyle(writer, ((StringLiteral)this.Index).Value);
			} else {
				this.WriteGenericStyle(writer);
			}
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);
	}

}

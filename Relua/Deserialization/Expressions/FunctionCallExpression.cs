using Relua.Deserialization.Definitions;
using Relua.Deserialization.Literals;
using System.Collections.Generic;




namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Function call expression and statement.
	/// 
	/// If `ForceTruncateReturnValues` is `true`, then this call will be
	/// surrounded with parentheses to make sure that the result of the
	/// expression is only its first return value. `ForceTruncateReturnValues`
	/// will automatically be unset if the `FunctionCall` is augmented with
	/// expressions such as unary operators or table access, because those
	/// operations automatically truncate the function's return values.
	/// 
	/// Method-style function calls will be parsed into this node. The `Function`
	/// field will then be a `TableAccess` node, and the `Arguments` list will
	/// be prepended with the `Table` field of that node.
	/// 
	/// While writing, an attempt will be made to simplify the function call
	/// to a method-style call if possible (`a:b()` syntax). This will only
	/// happen if `Function` is a `TableAccess` node and its index is a string
	/// literal and valid identifier. The `FunctionCall` will also need to have
	/// at least one argument, the first of which must be the `Table` field of
	/// the `TableAccess` node (referential equality). If these conditions are
	/// satisfied, the colon character will be written and the first argument of
	/// the function will be skipped.
	/// 
	/// ```
	/// a("hi")
	/// (function(a) print(a) end)(1, 2, 3)
	/// print"Hello, world!"
	/// do_smth_with_table{shorthand = "syntax"}
	/// obj:method("Hello!")
	/// </summary>
	public class FunctionCallExpression : Node, IExpression, IStatement {

		public IExpression Function;
		public List<IExpression> Arguments = new List<IExpression>();
		public bool ForceTruncateReturnValues = false;


		public void WriteMethodStyle(IndentAwareTextWriter writer, IExpression obj, string method_name) {
			if (obj is FunctionDefinition) {
				writer.Write("(");
			}

			obj.Write(writer);
			if (obj is FunctionDefinition) {
				writer.Write(")");
			}

			writer.Write(":");
			writer.Write(method_name);
			writer.Write("(");
			for (int i = 1; i < this.Arguments.Count; i++) {
				this.Arguments[i].Write(writer);
				if (i < this.Arguments.Count - 1) {
					writer.Write(", ");
				}
			}
			writer.Write(")");
		}


		public void WriteGenericStyle(IndentAwareTextWriter writer) {
			if (this.Function is FunctionDefinition) {
				writer.Write("(");
			}

			this.Function.Write(writer);
			if (this.Function is FunctionDefinition) {
				writer.Write(")");
			}

			writer.Write("(");
			for (int i = 0; i < this.Arguments.Count; i++) {
				this.Arguments[i].Write(writer);
				if (i < this.Arguments.Count - 1) {
					writer.Write(", ");
				}
			}
			writer.Write(")");
		}


		public override void Write(IndentAwareTextWriter writer) {
			if (this.ForceTruncateReturnValues) {
				writer.Write("(");
			}

			if (this.Function is TableAccessExpression && this.Arguments.Count > 0) {
				TableAccessExpression tf = (TableAccessExpression)this.Function;

				if (tf.Table == this.Arguments[0] && tf.Index is StringLiteral && ((StringLiteral)tf.Index).Value.IsIdentifier()) {
					this.WriteMethodStyle(writer, tf.Table, ((StringLiteral)tf.Index).Value);
				} else {
					this.WriteGenericStyle(writer);
				}
			} else {
				this.WriteGenericStyle(writer);
			}

			if (this.ForceTruncateReturnValues) {
				writer.Write(")");
			}
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

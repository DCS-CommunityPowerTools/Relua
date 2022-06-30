using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Unary operation expression.
	/// 
	/// ```
	/// not value
	/// -value
	/// #table
	/// </summary>
	public class UnaryOp : Node, IExpression {

		public enum OpType {
			Negate,
			Invert,
			Length
		}


		public OpType Type;
		public IExpression Expression;


		public UnaryOp() { }


		public UnaryOp(OpType type, IExpression expr) {
			Type = type;
			Expression = expr;
		}


		public static void WriteUnaryOp(OpType type, IndentAwareTextWriter writer) {
			switch (type) {
				case OpType.Negate: writer.Write("-"); break;
				case OpType.Invert: writer.Write("not "); break;
				case OpType.Length: writer.Write("#"); break;
			}
		}


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("(");
			WriteUnaryOp(Type, writer);
			(Expression as Node).Write(writer);
			writer.Write(")");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

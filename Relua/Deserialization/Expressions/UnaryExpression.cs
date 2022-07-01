namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Unary operation expression.
	/// 
	/// ```
	/// not value
	/// -value
	/// #table
	/// </summary>
	public class UnaryExpression : Node, IExpression {

		public enum OpType {
			Negate,
			Invert,
			Length
		}


		public OpType Type;
		public IExpression Expression;


		public UnaryExpression() { }


		public UnaryExpression(OpType type, IExpression expr) {
			this.Type = type;
			this.Expression = expr;
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
			WriteUnaryOp(this.Type, writer);
			(this.Expression as Node).Write(writer);
			writer.Write(")");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

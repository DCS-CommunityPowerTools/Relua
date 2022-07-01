namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Conditional block node. This is neither a standalone statement nor an
	/// expression, but a representation of a single generic `if` condition
	/// (i.e. `if` and `elseif`). See `If` for the actual `if` statement.
	/// </summary>
	public class ConditionalBlockStatement : Node {

		public IExpression Condition;
		public BlockStatement Block;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("if ");
			this.Condition.Write(writer);
			writer.Write(" then");
			writer.IncreaseIndent();
			writer.WriteLine();
			this.Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Repeat statement.
	/// 
	/// ```
	/// repeat
	///     print("TEST")
	/// until test_finished()
	/// ```
	/// </summary>
	public class RepeatStatement : Node, IStatement {

		public IExpression Condition;
		public BlockStatement Block;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("repeat");
			writer.IncreaseIndent();
			writer.WriteLine();
			this.Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("until ");
			this.Condition.Write(writer);
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

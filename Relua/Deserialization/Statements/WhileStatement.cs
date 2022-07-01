namespace Relua.Deserialization.Statements {

	/// <summary>
	/// While statement.
	/// 
	/// ```
	/// while true do
	///     print("spam")
	/// end
	/// ```
	/// </summary>
	public class WhileStatement : Node, IStatement {

		public IExpression Condition;
		public BlockStatement Block;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("while ");
			this.Condition.Write(writer);
			writer.Write(" do");
			writer.IncreaseIndent();
			writer.WriteLine();
			this.Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("end");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

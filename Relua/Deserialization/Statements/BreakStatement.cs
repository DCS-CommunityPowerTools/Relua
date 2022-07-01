namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Break statement.
	/// 
	/// ```
	/// break
	/// ```
	/// </summary>
	public class BreakStatement : Node, IStatement {

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("break");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

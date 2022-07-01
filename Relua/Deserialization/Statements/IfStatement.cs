using System.Collections.Generic;




namespace Relua.Deserialization.Statements {

	/// <summary>
	/// If statement.
	/// 
	/// ```
	/// if true then
	///     print("true")
	/// elseif false then
	///     print("false")
	/// else
	///     print("tralse")
	/// end
	/// </summary>
	public class IfStatement : Node, IStatement {

		public ConditionalBlockStatement MainIf;
		public List<ConditionalBlockStatement> ElseIfs = new List<ConditionalBlockStatement>();
		public BlockStatement Else;


		public override void Write(IndentAwareTextWriter writer) {
			this.MainIf.Write(writer);
			for (int i = 0; i < this.ElseIfs.Count; i++) {
				writer.Write("else");
				this.ElseIfs[i].Write(writer);
			}
			if (this.Else != null) {
				writer.Write("else");
				writer.IncreaseIndent();
				writer.WriteLine();
				this.Else.Write(writer, false);
				writer.DecreaseIndent();
				writer.WriteLine();
			}
			writer.Write("end");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

using System.Collections.Generic;




namespace Relua.Deserialization.Statements {

	/// <summary>
	/// A block statement. Usually used as part of another statement and not
	/// a standalone statement by itself. If `TopLevel` is true, then the `do
	/// end` construct will never be emitted, even if the node is not being
	/// written by another node.
	/// 
	/// ```
	/// do
	///     print("abc")
	/// end
	/// </summary>
	public class BlockStatement : Node, IStatement {

		public List<IStatement> Statements = new List<IStatement>();
		public bool TopLevel;




		public bool IsEmpty => this.Statements.Count == 0;




		public void Write(IndentAwareTextWriter writer, bool alone) {
			if (this.TopLevel && alone) {
				alone = false;
			}

			if (alone) {
				writer.Write("do");
				writer.IncreaseIndent();
				writer.WriteLine();
			}
			for (int i = 0; i < this.Statements.Count; i++) {
				IStatement stat = this.Statements[i];

				stat.Write(writer);
				//if (writer.ForceOneLine && stat.AmbiguousTermination && i != Statements.Count - 1) {
				//    writer.Write(";");
				//}
				if (i < this.Statements.Count - 1) {
					writer.WriteLine();
				}
			}
			if (alone) {
				writer.DecreaseIndent();
				writer.WriteLine();
				writer.Write("end");
			}
		}


		public override void Write(IndentAwareTextWriter writer)
			=> this.Write(writer, true);




		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class Block : Node, IStatement {
		public List<IStatement> Statements = new List<IStatement>();
		public bool TopLevel;

		public bool IsEmpty => Statements.Count == 0;

		public override void Write(IndentAwareTextWriter writer) {
			Write(writer, true);
		}

		public void Write(IndentAwareTextWriter writer, bool alone) {
			if (TopLevel && alone) alone = false;

			if (alone) {
				writer.Write("do");
				writer.IncreaseIndent();
				writer.WriteLine();
			}
			for (var i = 0; i < Statements.Count; i++) {
				var stat = Statements[i];

				stat.Write(writer);
				//if (writer.ForceOneLine && stat.AmbiguousTermination && i != Statements.Count - 1) {
				//    writer.Write(";");
				//}
				if (i < Statements.Count - 1) writer.WriteLine();
			}
			if (alone) {
				writer.DecreaseIndent();
				writer.WriteLine();
				writer.Write("end");
			}
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

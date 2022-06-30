using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class Repeat : Node, IStatement {
		public IExpression Condition;
		public Block Block;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("repeat");
			writer.IncreaseIndent();
			writer.WriteLine();
			Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("until ");
			Condition.Write(writer);
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

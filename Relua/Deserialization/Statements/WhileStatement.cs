using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class While : Node, IStatement {
		public IExpression Condition;
		public Block Block;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("while ");
			Condition.Write(writer);
			writer.Write(" do");
			writer.IncreaseIndent();
			writer.WriteLine();
			Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("end");
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

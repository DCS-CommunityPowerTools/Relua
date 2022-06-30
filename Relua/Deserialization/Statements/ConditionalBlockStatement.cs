using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Conditional block node. This is neither a standalone statement nor an
	/// expression, but a representation of a single generic `if` condition
	/// (i.e. `if` and `elseif`). See `If` for the actual `if` statement.
	/// </summary>
	public class ConditionalBlock : Node {
		public IExpression Condition;
		public Block Block;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("if ");
			Condition.Write(writer);
			writer.Write(" then");
			writer.IncreaseIndent();
			writer.WriteLine();
			Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

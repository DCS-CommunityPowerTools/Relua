using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Return statement.
	/// 
	/// ```
	/// return
	/// ```
	/// </summary>
	public class Return : Node, IStatement {
		public List<IExpression> Expressions = new List<IExpression>();

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("return");
			if (Expressions.Count > 0) writer.Write(" ");
			for (var i = 0; i < Expressions.Count; i++) {
				var expr = Expressions[i];

				expr.Write(writer);
				if (i < Expressions.Count - 1) writer.Write(", ");
			}
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

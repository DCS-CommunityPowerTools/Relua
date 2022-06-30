using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Generic for statement.
	/// 
	/// ```
	/// for k, v in pairs(some_table) do
	///     print(tostring(k) .. " = " .. tostring(v))
	/// end
	/// 
	/// for i, v in ipairs(some_table) do
	///     print(v)
	/// end
	/// </summary>
	public class GenericFor : For {
		public List<string> VariableNames = new List<string>();
		public IExpression Iterator;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("for ");
			for (var i = 0; i < VariableNames.Count; i++) {
				writer.Write(VariableNames[i]);
				if (i < VariableNames.Count - 1) writer.Write(", ");
			}
			writer.Write(" in ");
			Iterator.Write(writer);
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

using Relua.Deserialization.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Numeric for statement. Please note that `Step` is optional in code and
	/// defaults to 1. If the `AutofillNumericForStep` option is enabled in
	/// `Parser.Settings` (which it is by default), then the `Step` field will
	/// never be null. Instead, if the code does not specify it, it will be
	/// created automatically as a `NumberLiteral` of value 1. Without this
	/// option, the field may be null if no step was specified.
	/// 
	/// ```
	/// for i=1,10 do
	///     print(i)
	/// end
	/// ```
	/// </summary>
	public class NumericFor : For {
		public string VariableName;
		public IExpression StartPoint;
		public IExpression EndPoint;
		public IExpression Step;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("for ");
			writer.Write(VariableName);
			writer.Write(" = ");
			StartPoint.Write(writer);
			writer.Write(", ");
			EndPoint.Write(writer);
			if (Step != null && !(Step is NumberLiteral && ((NumberLiteral)Step).Value == 1)) {
				writer.Write(", ");
				Step.Write(writer);
			}
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

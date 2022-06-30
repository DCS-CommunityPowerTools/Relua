using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Literals {


	/// <summary>
	/// Boolean literal expression.
	/// 
	/// ```
	/// true
	/// false
	/// ```
	/// </summary>
	public class BoolLiteral : Node, IExpression {

		public static BoolLiteral TrueInstance = new BoolLiteral { Value = true };
		public static BoolLiteral FalseInstance = new BoolLiteral { Value = false };
		public bool Value;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write(Value ? "true" : "false");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

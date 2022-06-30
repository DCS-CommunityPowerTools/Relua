using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Variable expression.
	/// 
	/// ```
	/// some_var
	/// ```
	/// </summary>
	public class VariableExpression : Node, IExpression, IAssignable {

		public string Name;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write(Name);
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Break statement.
	/// 
	/// ```
	/// break
	/// ```
	/// </summary>
	public class Break : Node, IStatement {
		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("break");
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

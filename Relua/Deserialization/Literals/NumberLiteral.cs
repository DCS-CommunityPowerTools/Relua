using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Literals {

	/// <summary>
	/// Number literal expression. The value is always a `double`. If `HexFormat`
	/// is true, the value will be converted to a long and then written in hex.
	/// 
	/// ```
	/// 123
	/// 0xFF
	/// ```
	/// </summary>
	public class NumberLiteral : Node, IExpression {
		public double Value;
		public bool HexFormat = false;

		public override void Write(IndentAwareTextWriter writer) {
			if (HexFormat) {
				writer.Write("0x");
				writer.Write(((long)Value).ToString("X"));
			} else writer.Write(Value);
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

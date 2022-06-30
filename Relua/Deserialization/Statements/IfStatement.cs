using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// If statement.
	/// 
	/// ```
	/// if true then
	///     print("true")
	/// elseif false then
	///     print("false")
	/// else
	///     print("tralse")
	/// end
	/// </summary>
	public class If : Node, IStatement {
		public ConditionalBlock MainIf;
		public List<ConditionalBlock> ElseIfs = new List<ConditionalBlock>();
		public Block Else;

		public override void Write(IndentAwareTextWriter writer) {
			MainIf.Write(writer);
			for (var i = 0; i < ElseIfs.Count; i++) {
				writer.Write("else");
				ElseIfs[i].Write(writer);
			}
			if (Else != null) {
				writer.Write("else");
				writer.IncreaseIndent();
				writer.WriteLine();
				Else.Write(writer, false);
				writer.DecreaseIndent();
				writer.WriteLine();
			}
			writer.Write("end");
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}

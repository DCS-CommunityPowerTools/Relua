using System.Collections.Generic;




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
	public class GenericForStatement : ForStatement {

		public List<string> VariableNames = new List<string>();
		public IExpression Iterator;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("for ");
			for (int i = 0; i < this.VariableNames.Count; i++) {
				writer.Write(this.VariableNames[i]);
				if (i < this.VariableNames.Count - 1) {
					writer.Write(", ");
				}
			}
			writer.Write(" in ");
			this.Iterator.Write(writer);
			writer.Write(" do");
			writer.IncreaseIndent();
			writer.WriteLine();
			this.Block.Write(writer, false);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write("end");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

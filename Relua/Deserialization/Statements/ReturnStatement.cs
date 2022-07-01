using System.Collections.Generic;




namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Return statement.
	/// 
	/// ```
	/// return
	/// ```
	/// </summary>
	public class ReturnStatement : Node, IStatement {

		public List<IExpression> Expressions = new List<IExpression>();


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("return");
			if (this.Expressions.Count > 0) {
				writer.Write(" ");
			}

			for (int i = 0; i < this.Expressions.Count; i++) {
				IExpression expr = this.Expressions[i];

				expr.Write(writer);
				if (i < this.Expressions.Count - 1) {
					writer.Write(", ");
				}
			}
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

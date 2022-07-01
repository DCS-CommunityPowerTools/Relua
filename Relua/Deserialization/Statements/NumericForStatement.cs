using Relua.Deserialization.Literals;




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
	public class NumericForStatement : ForStatement {

		public string VariableName;
		public IExpression StartPoint;
		public IExpression EndPoint;
		public IExpression Step;


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("for ");
			writer.Write(this.VariableName);
			writer.Write(" = ");
			this.StartPoint.Write(writer);
			writer.Write(", ");
			this.EndPoint.Write(writer);
			if (this.Step != null && !(this.Step is NumberLiteral && ((NumberLiteral)this.Step).Value == 1)) {
				writer.Write(", ");
				this.Step.Write(writer);
			}
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

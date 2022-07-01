namespace Relua.Deserialization.Literals {

	/// <summary>
	/// Nil value literal expression.
	/// 
	/// ```
	/// nil
	/// ```
	/// </summary>
	public class NilLiteral : Node, IExpression {

		public static NilLiteral Instance = new NilLiteral();


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("nil");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

namespace Relua.Deserialization.Literals {

	/// <summary>
	/// Varargs literal expression. Please note that in the case of
	/// `FunctionDefinition`s parameter names are read as strings, not AST nodes.
	/// This node will only appear in actual uses of the value of the varargs in
	/// statements and expressions, and to detect if a `FunctionDefinition`
	/// accepts varargs, you have to use its `AcceptsVarargs` field.
	/// 
	/// ```
	/// ...
	/// ```
	/// </summary>
	public class VarargsLiteral : Node, IExpression {

		public static VarargsLiteral Instance = new VarargsLiteral();


		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("...");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

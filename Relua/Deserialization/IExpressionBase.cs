namespace Relua.Deserialization {

	public interface IExpressionBase {
		void Write(IndentAwareTextWriter writer);
		void Accept(IVisitor visitor);
		string ToString(bool one_line);
	}

}

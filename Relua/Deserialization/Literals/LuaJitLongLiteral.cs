namespace Relua.Deserialization.Literals {

	/// <summary>
	/// Long literal expression (LuaJIT style). If `HexFormat` is true, the
	/// value will always be written in hex. Will never be read if the
	/// `EnableLuaJITLongs` option of `Parser.Settings` is `false` (it's `true`
	/// by default).
	/// 
	/// ```
	/// 123LL
	/// 0x123LL
	/// ```
	/// </summary>
	public class LuaJitLongLiteral : Node, IExpression {

		public long Value;
		public bool HexFormat = false;


		public override void Write(IndentAwareTextWriter writer) {
			if (this.HexFormat) {
				writer.Write("0x");
				writer.Write(this.Value.ToString("X4"));
			} else {
				writer.Write(this.Value);
			}

			writer.Write("LL");
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}

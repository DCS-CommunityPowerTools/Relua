using Relua.Exceptions;




namespace Relua.Deserialization.Exceptions {

	/// <summary>
	/// Exception thrown when the parser runs into invalid syntax.
	/// </summary>
	public class ParserException : ReluaException {

		public ParserException(string msg, Tokenizer.Region region)
						: base($"Failed parsing: {msg} [{region.BoundsToString()}]") {
			this.Line = region.StartLine;
			this.Column = region.StartColumn;
		}

		public ParserException(string msg) : base(msg) { }

	}

}

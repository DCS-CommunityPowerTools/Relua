using Relua.Exceptions;




namespace Relua.Deserialization.Exceptions {

	/// <summary>
	/// Exception thrown when the tokenizer runs into invalid syntax.
	/// </summary>
	public class TokenizerException : ReluaException {

		public TokenizerException(string msg, int line, int @char)
						: base($"Failed tokenizing: {msg} [{line}:{@char}]") {
			this.Line = line;
			this.Column = @char;
		}

		public TokenizerException(string msg, Tokenizer.Region region)
						: base($"Failed tokenizing: {msg} [{region.BoundsToString()}]") {
			this.Line = region.StartLine;
			this.Column = region.StartColumn;
		}

	}

}

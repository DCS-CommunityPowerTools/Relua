namespace Relua {

	public enum TokenType {
		EOF,
		Identifier,
		QuotedString,
		Number,
		Punctuation
	}




	public struct Token {

		public static readonly Token EOF = new Token(TokenType.EOF, null, new Tokenizer.Region());




		public TokenType Type;
		public string Value;
		public Tokenizer.Region Region;




		public Token(TokenType type, string value, Tokenizer.Region reg) {
			this.Type = type;
			this.Value = value;
			this.Region = reg;
		}




		public bool Is(TokenType type, string value) {
			return this.Type == type && this.Value == value;
		}


		public bool IsEOF() {
			return this.Type == TokenType.EOF;
		}


		public bool IsIdentifier(string value) {
			return this.Is(TokenType.Identifier, value);
		}


		public bool IsQuotedString(string value) {
			return this.Is(TokenType.QuotedString, value);
		}


		public bool IsNumber(string value) {
			return this.Is(TokenType.Number, value);
		}


		public bool IsPunctuation(string value) {
			return this.Is(TokenType.Punctuation, value);
		}




		public override string ToString() {
			return $"TOKEN {{type = {this.Type}, value = {this.Value.Inspect()}, region = {this.Region.Inspect()}}}";
		}

	}

}

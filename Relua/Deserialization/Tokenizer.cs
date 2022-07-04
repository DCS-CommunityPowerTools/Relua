using Relua.Deserialization.Exceptions;
using Relua.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace Relua {

	public class Tokenizer {

		public struct Region {

			public Tokenizer Tokenizer;
			public int StartChar;
			public int StartLine;
			public int StartColumn;
			public int EndChar;
			public int EndLine;
			public int EndColumn;




			public void End() {
				this.EndLine = this.Tokenizer.CurrentLine;
				this.EndColumn = this.Tokenizer.CurrentColumn;
				this.EndChar = this.Tokenizer.CurrentIndex;
			}


			public string BoundsToString() {
				if (this.StartChar == this.EndChar) {
					return $"{this.StartLine}:{this.StartChar}";
				}

				return $"{this.StartLine}:{this.StartChar} -> {this.EndLine}:{this.EndChar}";
			}


			public override string ToString() {
				return this.Tokenizer.Data.Substring(this.StartChar, this.EndChar - this.StartChar);
			}

		}




		public static HashSet<char> WHITESPACE = new HashSet<char> { ' ', '\t', '\n', '\r' };
		public static HashSet<char> PUNCTUATION = new HashSet<char> {
			'#', '%', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';',
			'<', '=', '>', '[', ']', '^', '{', '}', '~'
		};

		public static HashSet<string> RESERVED_KEYWORDS = new HashSet<string> {
			"and", "break", "do", "else", "elseif", "end",
			"false", "for", "function", "if", "in", "local",
			"nil", "not", "or", "repeat", "return", "then",
			"true", "until", "while"
		};




		public string Data;
		public int CurrentIndex = 0;

		public int CurrentLine = 1;
		public int CurrentColumn = 1;
		public Parser.Settings ParserSettings;




		public char CurChar {
			get {
				if (this.CurrentIndex >= this.Data.Length) {
					return '\0';
				}

				return this.Data[this.CurrentIndex];
			}
		}


		public bool EOF
			=> this.CurChar == '\0';


		private Token? _CachedPeekToken = null;
		public Token PeekToken {
			get {
				if (this._CachedPeekToken.HasValue) {
					return this._CachedPeekToken.Value;
				}

				return (this._CachedPeekToken = this.NextToken()).Value;
			}
		}




		public Tokenizer(string data, Parser.Settings settings = null) {
			this.ParserSettings = settings ?? new Parser.Settings();
			this.Data = data;
		}




		public void Throw(string msg) {
			throw new TokenizerException(msg, this.CurrentLine, this.CurrentColumn);
		}


		public void Throw(string msg, Region region) {
			throw new TokenizerException(msg, region);
		}




		public char Peek(int n = 1) {
			if (this.CurrentIndex + n >= this.Data.Length) {
				return '\0';
			}

			return this.Data[this.CurrentIndex + n];
		}


		public char Move(int n = 1) {
			this.CurrentIndex += n;
			this.CurrentColumn += 1;
			if (this.CurChar == '\n') {
				this.CurrentLine += 1;
				this.CurrentColumn = 1;
			}
			return this.CurChar;
		}


		public Region StartRegion() {
			return new Region {
				Tokenizer = this,
				StartLine = CurrentLine,
				StartColumn = CurrentColumn,
				StartChar = CurrentIndex
			};
		}


		public string ReadUntil(params char[] c) {
			Region read_region = this.StartRegion();

			while (!this.EOF && !c.Contains(this.Move())) { }

			read_region.End();

			if (this.EOF) {
				this.Throw($"Expected one of: {c.Inspect()}");
			}

			return read_region.ToString();
		}


		public void SkipWhitespace() {
			while (!this.EOF && WHITESPACE.Contains(this.CurChar)) {
				this.Move();
			}
		}


		public string ReadQuoted(out Region reg) {
			if (this.CurChar != '"' && this.CurChar != '\'') {
				this.Throw($"Expected quoted string");
			}

			bool is_single_quote = this.CurChar == '\'';
			this.Move();
			StringBuilder s = new StringBuilder();
			reg = this.StartRegion();
			bool escaped = false;
			while (true) {
				char c = this.CurChar;

				if (escaped) {
					switch (c) {
						case 'n': c = '\n'; break;
						case 't': c = '\t'; break;
						case 'r': c = '\r'; break;
						case 'a': c = '\a'; break;
						case 'b': c = '\b'; break;
						case 'f': c = '\f'; break;
						case 'v': c = '\v'; break;
						case '\\': c = '\\'; break;
						case '"': c = '"'; break;
						case '\'': c = '\''; break;
						default:
							if (IsDigit(c)) {
								int num = c - '0';
								if (IsDigit(this.Peek(1))) {
									num *= 10;
									num += this.Peek(1) - '0';
									this.Move();
								}
								if (IsDigit(this.Peek(1))) {
									num *= 10;
									num += this.Peek(1) - '0';
									this.Move();
								}
								c = (char)num; break;
							}
							this.Throw($"Unknown escape sequence '\\{c}'");
							break;
					}

					s.Append(c);
					this.Move();
					escaped = false;
					continue;
				}

				if (c == '\\') {
					escaped = true;
					this.Move();
					continue;
				} else if ((is_single_quote && c == '\'') || (!is_single_quote && c == '"')) {
					this.Move();
					break;
				}

				if (this.EOF) {
					reg.End();
					this.Throw($"Unterminated quoted string", reg);
				}

				s.Append(c);
				this.Move();
			}

			return s.ToString();
		}


		public void Expect(char[] chars) {
			if (!chars.Contains(this.CurChar)) {
				this.Throw($"Expected one of: {chars.Inspect()}");
			}
		}


		public string ReadIdentifier(out Region reg) {
			reg = this.StartRegion();
			if (this.CurChar != '_' && !(this.CurChar >= 'a' && this.CurChar <= 'z') && !(this.CurChar >= 'A' && this.CurChar <= 'Z')) {
				this.Throw($"Expected identifier start, got {this.CurChar.Inspect()}");
			}
			this.Move();
			while (!this.EOF && (this.CurChar == '_' || (this.CurChar >= 'a' && this.CurChar <= 'z') || (this.CurChar >= 'A' && this.CurChar <= 'Z') || (this.CurChar >= '0' && this.CurChar <= '9'))) {
				this.Move();
			}
			reg.End();
			return reg.ToString();
		}


		public string ReadNumber(out Region reg) {
			reg = this.StartRegion();
			if ((this.CurChar == '+' || this.CurChar == '-') && this.Peek(1) == '.' && !IsDigit(this.Peek(2))) {
				this.Throw($"Expected number, got {new string(new char[] { this.CurChar, this.Peek(1), this.Peek(2) }).Inspect()}...");
			} else if ((this.CurChar == '+' || this.CurChar == '-') && !IsDigit(this.Peek(1))) {
				this.Throw($"Expected number, got {new string(new char[] { this.CurChar, this.Peek(1) }).Inspect()}...");
			} else if (this.CurChar == '.' && !IsDigit(this.Peek(1))) {
				this.Throw($"Expected number, got {new string(new char[] { this.CurChar, this.Peek(1) }).Inspect()}...");
			} else if (!IsDigit(this.CurChar)) {
				this.Throw($"Expected number, got {this.CurChar.Inspect()}");
			}
			if (this.CurChar == '0' && this.Peek(1) == 'x') {
				this.Move(2);
				while (!this.EOF && IsHexDigit(this.CurChar)) {
					this.Move();
				}
				reg.End();
				return reg.ToString();
			}
			this.Move();
			if (this.CurChar == '.') {
				this.Move();
			}

			while (!this.EOF && IsDigit(this.CurChar)) {
				this.Move();
			}

			if (this.CurChar == '.') {
				this.Move();
				while (!this.EOF && IsDigit(this.CurChar)) {
					this.Move();
				}
			}

			reg.End();

			return reg.ToString();
		}


		public string ReadPunctuation(out Region reg) {
			if (!IsPunctuation(this.CurChar)) {
				this.Throw($"Expected punctuation, got {this.CurChar.Inspect()}");
			}

			char c = this.CurChar;
			reg = this.StartRegion();
			char p = this.Peek(1);
			char p2 = this.Peek(2);
			reg.End();
			this.Move();

			// complex punctuation
			if (c == '=' && p == '=') { reg.End(); this.Move(); return "=="; }
			if (c == '<' && p == '=') { reg.End(); this.Move(); return "<="; }
			if (c == '>' && p == '=') { reg.End(); this.Move(); return ">="; }
			if (c == '.' && p == '.' && p2 == '.') { reg.End(); this.Move(); this.Move(); return "..."; }
			if (c == '.' && p == '.') { reg.End(); this.Move(); return ".."; }
			if (c == '~' && p == '=') { reg.End(); this.Move(); return "~="; }

			return c.ToString();
		}


		public static bool IsIdentifierStartSymbol(char c) {
			return c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}


		public static bool IsIdentifierContSymbol(char c) {
			return IsDigit(c) || c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}


		public static bool IsWhitespace(char c) {
			return c == ' ' || c == '\t' || c == '\n' || c == '\r';
		}


		public static bool IsDigit(char c) {
			return c >= '0' && c <= '9';
		}


		public static bool IsHexDigit(char c) {
			return IsDigit(c) || ((c >= 'a') && (c <= 'f')) || ((c >= 'A') && (c <= 'F'));
		}


		public static bool IsPunctuation(char c) {
			return PUNCTUATION.Contains(c);
		}


		public bool SkipMultilineComment() {
			if (this.CurChar == '[') {
				int eq_count = 0;
				this.Move();
				while (this.CurChar == '=') {
					eq_count += 1;
					this.Move();
				}

				if (this.CurChar == '[') {
					this.Move();
					while (!this.EOF) {
						if (this.CurChar == ']') {
							this.Move(1);
							int cur_eq_count = 0;
							while (this.CurChar == '=') {
								cur_eq_count += 1;
								this.Move(1);
							}

							if (cur_eq_count == eq_count && this.CurChar == ']') {
								this.Move();
								break;
							}
						}
						this.Move();
					}
					return true;
				}
			}

			return false;
		}


		public Token NextToken() {
			if (this._CachedPeekToken.HasValue) {
				Token tok = this._CachedPeekToken.Value;
				this._CachedPeekToken = null;
				return tok;
			}

			this.SkipWhitespace();

			char c = this.CurChar;

			if (this.EOF) {
				return Token.EOF;
			}

			while (c == '-' && this.Peek(1) == '-') {
				this.Move(2);

				if (!this.SkipMultilineComment()) {
					while (!this.EOF && this.CurChar != '\n') {
						this.Move(1);
					}
				}

				if (this.EOF) {
					return Token.EOF;
				}

				this.SkipWhitespace();
				c = this.CurChar;
			}

			if (this.EOF) {
				return Token.EOF;
			}

			if (IsDigit(c)) {
				Region reg;
				string val = this.ReadNumber(out reg);
				return new Token(TokenType.Number, val, reg);
			} else if (IsIdentifierStartSymbol(c)) {
				Region reg;
				string val = this.ReadIdentifier(out reg);
				if (RESERVED_KEYWORDS.Contains(val)) {
					return new Token(TokenType.Punctuation, val, reg);
				} else {
					return new Token(TokenType.Identifier, val, reg);
				}
			} else if (c == '"' || c == '\'') {
				Region reg;
				string val = this.ReadQuoted(out reg);
				return new Token(TokenType.QuotedString, val, reg);
			} else if (IsPunctuation(c)) {
				Region reg;
				string val = this.ReadPunctuation(out reg);
				return new Token(TokenType.Punctuation, val, reg);
			} else {
				this.Throw($"Unrecognized character: {c.Inspect()}");
				throw new Exception("unreachable");
			}
		}

	}

}

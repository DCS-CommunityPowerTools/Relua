using Relua.Deserialization.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Exceptions {

	/// <summary>
	/// Exception thrown when the tokenizer runs into invalid syntax.
	/// </summary>
	public class TokenizerException : ReluaException {

		public TokenizerException(string msg, int line, int @char)
						: base($"Failed tokenizing: {msg} [{line}:{@char}]") {
			Line = line;
			Column = @char;
		}

		public TokenizerException(string msg, Tokenizer.Region region)
						: base($"Failed tokenizing: {msg} [{region.BoundsToString()}]") {
			Line = region.StartLine;
			Column = region.StartColumn;
		}

	}

}

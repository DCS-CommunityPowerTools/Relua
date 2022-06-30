using Relua.Deserialization.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Exceptions {

	/// <summary>
	/// Exception thrown when the parser runs into invalid syntax.
	/// </summary>
	public class ParserException : ReluaException {

		public ParserException(string msg, Tokenizer.Region region)
						: base($"Failed parsing: {msg} [{region.BoundsToString()}]") {
			Line = region.StartLine;
			Column = region.StartColumn;
		}

		public ParserException(string msg) : base(msg) { }

	}

}

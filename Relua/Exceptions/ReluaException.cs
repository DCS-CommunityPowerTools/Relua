using System;
namespace Relua.Deserialization.Exceptions {
	/// <summary>
	/// Base class for Relua exceptions. By catching this type, you can
	/// catch both types of exceptions (while tokenizing and while parsing).
	/// </summary>
	public abstract class ReluaException : Exception {
		public int Line;
		public int Column;

		protected ReluaException(string msg) : base(msg) { }
	}
}

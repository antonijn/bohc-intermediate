using System;

namespace Bohc.Parsing
{
	public enum TokenType
	{
		UNIDENTIFIED, // spaces, etc

		IDENTIFIER,

		NEWLINE,

		DEC_INTEGER,
		HEX_INTEGER,
		OCT_INTEGER,
		BIN_INTEGER,
		FLOAT,
		DOUBLE,
		DECIMAL,
		BOOLEAN,
		STRING,
		CHAR,

		OPERATOR,

		NULL,
		NEW,
		COMMA,

		MODIFIER,
		DIRECTIVE,
		PRIMITIVE,
		CLASS_ENUM_STRUCT,
		IF_OR_LOOP, // if, for, while, do, try, catch, finally
		CONTROL_FLOW, // break, continue, return, throw

		SEMICOLON,

		BRACKET,
		CURLY_BRACKET,
		SQUARE_BRACKET,
	}
}


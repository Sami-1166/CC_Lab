using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

public enum TokenType
{
    Keyword,
    Identifier,
    Operator,
    DataType,
    Literal,
    StringLiteral,
    Comment,
    Punctuation,
    EndOfFile,
    Unknown
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public Token(TokenType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"[{Type}] {Value} (Line: {Line}, Column: {Column})";
    }
}

public class LexicalAnalyzer
{
    private static readonly string[] Keywords = {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach",
        "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
        "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
        "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    private static readonly string[] DataTypes = {
        "int", "long", "float", "double", "decimal", "bool", "char", "string", "object"
    };

    private static readonly string[] Operators = {
        "+", "-", "*", "/", "%", "++", "--", "=", "==", "+=", "-=", "*=", "/=", "%=",
        "&&", "||", "!", ">", "<", ">=", "<=", "!="
    };

    private static readonly string[] Punctuation = {
        "{", "}", "(", ")", "[", "]", ";", ".", ",", ":", "?"
    };

    private static readonly Regex IdentifierRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
    private static readonly Regex IntegerLiteralRegex = new Regex(@"^\d+$");
    private static readonly Regex FloatLiteralRegex = new Regex(@"^\d+\.\d+$");
    private static readonly Regex StringLiteralRegex = new Regex(@"^""[^""\\]*(\\.[^""\\]*)*""$");
    private static readonly Regex CommentRegex = new Regex(@"//.*?$|/\*[\s\S]*?\*/", RegexOptions.Multiline);

    private string _input;
    private int _position;
    private int _line;
    private int _column;

    public LexicalAnalyzer(string input)
    {
        _input = input;
        _position = 0;
        _line = 1;
        _column = 1;
    }

    public List<Token> Analyze()
    {
        List<Token> tokens = new List<Token>();
        while (_position < _input.Length)
        {
            char currentChar = _input[_position];
            if (char.IsWhiteSpace(currentChar))
            {
                HandleWhiteSpace();
            }
            else if (currentChar == '/')
            {
                HandleComment(tokens);
            }
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                HandleIdentifierOrKeyword(tokens);
            }
            else if (char.IsDigit(currentChar))
            {
                HandleNumber(tokens);
            }
            else if (currentChar == '"')
            {
                HandleStringLiteral(tokens);
            }
            else if (IsOperator(currentChar))
            {
                HandleOperator(tokens);
            }
            else if (IsPunctuation(currentChar))
            {
                HandlePunctuation(tokens);
            }
            else
            {
                HandleUnknown(tokens);
            }
        }

        tokens.Add(new Token(TokenType.EndOfFile, "<EOF>", _line, _column));
        return tokens;
    }

    private void HandleWhiteSpace()
    {
        char currentChar = _input[_position];
        if (currentChar == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }
        _position++;
    }

    private void HandleComment(List<Token> tokens)
    {
        if (_input[_position + 1] == '/')
        {
            // Single line comment
            int startPos = _position;
            while (_position < _input.Length && _input[_position] != '\n')
            {
                _position++;
            }
            tokens.Add(new Token(TokenType.Comment, _input.Substring(startPos, _position - startPos), _line, _column));
        }
        else if (_input[_position + 1] == '*')
        {
            // Multi-line comment
            int startPos = _position;
            _position += 2; // Skip '/*'
            while (_position < _input.Length && (_input[_position] != '*' || _input[_position + 1] != '/'))
            {
                if (_input[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
                _position++;
            }
            _position += 2; // Skip '*/'
            tokens.Add(new Token(TokenType.Comment, _input.Substring(startPos, _position - startPos), _line, _column));
        }
        else
        {
            HandleUnknown(tokens);
        }
    }

    private void HandleIdentifierOrKeyword(List<Token> tokens)
    {
        int startPos = _position;
        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            _position++;
        }

        string value = _input.Substring(startPos, _position - startPos);
        TokenType type = Array.Exists(Keywords, keyword => keyword == value) ? TokenType.Keyword : TokenType.Identifier;

        tokens.Add(new Token(type, value, _line, _column));
    }

    private void HandleNumber(List<Token> tokens)
    {
        int startPos = _position;
        while (_position < _input.Length && char.IsDigit(_input[_position]))
        {
            _position++;
        }

        if (_position < _input.Length && _input[_position] == '.')
        {
            _position++;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                _position++;
            }
            tokens.Add(new Token(TokenType.Literal, _input.Substring(startPos, _position - startPos), _line, _column));
        }
        else
        {
            tokens.Add(new Token(TokenType.Literal, _input.Substring(startPos, _position - startPos), _line, _column));
        }
    }

    private void HandleStringLiteral(List<Token> tokens)
    {
        int startPos = _position;
        _position++; // Skip the initial "
        while (_position < _input.Length && _input[_position] != '"')
        {
            if (_input[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            _position++;
        }
        if (_position < _input.Length && _input[_position] == '"')
        {
            _position++; // Skip the closing "
            tokens.Add(new Token(TokenType.StringLiteral, _input.Substring(startPos, _position - startPos), _line, _column));
        }
        else
        {
            HandleUnknown(tokens);
        }
    }

    private void HandleOperator(List<Token> tokens)
    {
        char currentChar = _input[_position];
        foreach (var op in Operators)
        {
            if (_input.Substring(_position).StartsWith(op))
            {
                tokens.Add(new Token(TokenType.Operator, op, _line, _column));
                _position += op.Length;
                return;
            }
        }
        HandleUnknown(tokens);
    }

    private void HandlePunctuation(List<Token> tokens)
    {
        char currentChar = _input[_position];
        foreach (var punct in Punctuation)
        {
            if (_input[_position] == punct[0])
            {
                tokens.Add(new Token(TokenType.Punctuation, punct, _line, _column));
                _position++;
                return;
            }
        }
    }

    private void HandleUnknown(List<Token> tokens)
    {
        tokens.Add(new Token(TokenType.Unknown, _input[_position].ToString(), _line, _column));
        _position++;
    }

    private bool IsOperator(char c)
    {
        return "+-*/%=&|^!<>".IndexOf(c) >= 0;
    }

    private bool IsPunctuation(char c)
    {
        return "{}()[];,.".IndexOf(c) >= 0;
    }
}

public class AST
{
    public string NodeType { get; set; }
    public string Value { get; set; }
    public List<AST> Children { get; set; } = new List<AST>();

    public AST(string nodeType, string value = "")
    {
        NodeType = nodeType;
        Value = value;
    }

    public void AddChild(AST child)
    {
        Children.Add(child);
    }

    public void Print(int indent = 0)
    {
        Console.WriteLine(new string(' ', indent * 2) + NodeType + (string.IsNullOrEmpty(Value) ? "" : ": " + Value));
        foreach (var child in Children)
        {
            child.Print(indent + 1);
        }
    }
}

public class Parser
{
    private List<Token> _tokens;
    private int _currentIndex = 0;


    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

public AST Parse()
    {
        try
        {
            return ParseCompilationUnit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private AST ParseCompilationUnit()
    {
        if (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type == TokenType.Keyword && _tokens[_currentIndex].Value == "class")
        {
            var node = new AST("CompilationUnit");
            node.AddChild(ParseClassDeclaration());

            while (_currentIndex < _tokens.Count && (_tokens[_currentIndex].Type == TokenType.Keyword && (_tokens[_currentIndex].Value == "class" || _tokens[_currentIndex].Value == "struct")))
            {
                node.AddChild(ParseTypeDeclaration());
            }

            return node;
        }

        throw new Exception($"Syntax error: Expected 'class', found {_tokens[_currentIndex].Value} at Line: {_tokens[_currentIndex].Line}, Column: {_tokens[_currentIndex].Column}");
    }

    private AST ParseClassDeclaration()
    {
        var node = new AST("ClassDeclaration");
        ConsumeToken(TokenType.Keyword, "class");

        var identifier = ConsumeToken(TokenType.Identifier);
        node.AddChild(new AST("class", identifier.Value));

        ConsumeToken(TokenType.Punctuation, "{");

        while (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type != TokenType.Punctuation && _tokens[_currentIndex].Value != "}")
        {
            node.AddChild(ParseTypeDeclaration());
        }

        ConsumeToken(TokenType.Punctuation, "}");
        return node;
    }

    private AST ParseTypeDeclaration()
    {
        var node = new AST("MethodDeclaration");
        // Check if we have access modifiers (public, private, etc.)
        if (_tokens[_currentIndex].Type == TokenType.Keyword && IsAccessModifier(_tokens[_currentIndex].Value))
        {
            // Consume the access modifier (e.g., public, private)
            var accessModifier = ConsumeToken(TokenType.Keyword);
            node.AddChild(new AST("AccessModifier", accessModifier.Value));
        }

        // Check if the 'static' keyword is present
        if (_tokens[_currentIndex].Type == TokenType.Keyword && _tokens[_currentIndex].Value == "static" ||
            _tokens[_currentIndex].Value == "const" || _tokens[_currentIndex].Value == "readonly" ||
            _tokens[_currentIndex].Value == "abtract" || _tokens[_currentIndex].Value == "sealed")
        {
            // Consume the static keyword
            var modifierKeyword = ConsumeToken(TokenType.Keyword);
            node.AddChild(new AST("Modifier", modifierKeyword.Value));

        }

        // Handle basic types (int, string, bool, etc.)
        if (_tokens[_currentIndex].Type == TokenType.Keyword && (_tokens[_currentIndex].Value == "int" || _tokens[_currentIndex].Value == "string" || _tokens[_currentIndex].Value == "bool") || _tokens[_currentIndex].Value == "void")
        {
            return ParseMethodDeclaration(node);
        }

        // Handle other data types (via IsDataType method)
        if (_tokens[_currentIndex].Type == TokenType.Keyword && IsDataType(_tokens[_currentIndex].Value))
        {
            return ParseLocalVariableDeclaration();
        }

        // If no match, throw an error
        throw new Exception($"Syntax error: Unexpected token {_tokens[_currentIndex].Value} at Line: {_tokens[_currentIndex].Line}, Column: {_tokens[_currentIndex].Column}");
    }

    private bool IsDataType(string tokenValue)
    {
        // Check if the token is one of the basic data types (e.g., int, double, string)
        return tokenValue == "int" || tokenValue == "double" || tokenValue == "string" ||
               tokenValue == "bool" || tokenValue == "char" || tokenValue == "long";
    }


    private AST ParseParameter()
    {
        var node = new AST("Parameter");

        // Consume the type of the parameter (e.g., int, string, etc.)
        var type = ConsumeToken(TokenType.Keyword);
        node.AddChild(new AST("Type", type.Value));

        // Consume the parameter name (e.g., variable name)
        var name = ConsumeToken(TokenType.Identifier);
        node.AddChild(new AST("Name", name.Value));

        return node;
    }


    private AST ParseMethodDeclaration(AST node)
    {

        // Handle access modifiers (e.g., public, private, protected)
        if (_tokens[_currentIndex].Type == TokenType.Keyword &&
            (IsAccessModifier(_tokens[_currentIndex].Value)))
        {
            var accessModifier = ConsumeToken(TokenType.Keyword);
            node.AddChild(new AST("AccessModifier", accessModifier.Value));
        }

        // Consume return type (e.g., void, int, double, etc.)
        var returnType = ConsumeToken(TokenType.Keyword);
        node.AddChild(new AST("ReturnType", returnType.Value));

        // Consume the method name
        var methodName = ConsumeToken(TokenType.Identifier);
        node.AddChild(new AST("MethodName", methodName.Value));

        // Handle method parameters
        ConsumeToken(TokenType.Punctuation, "(");
        while (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type != TokenType.Punctuation && _tokens[_currentIndex].Value != ")")
        {
            node.AddChild(ParseParameter());
            if (_tokens[_currentIndex].Type == TokenType.Punctuation && _tokens[_currentIndex].Value == ",")
            {
                ConsumeToken(TokenType.Punctuation, ",");
            }
        }
        ConsumeToken(TokenType.Punctuation, ")");

        // Parse method body (block)
        node.AddChild(ParseBlock());

        return node;
    }


    private bool IsAccessModifier(string tokenValue)
    {
        // Check if the token is an access modifier (public, private, etc.)
        return tokenValue == "public" || tokenValue == "private" || tokenValue == "protected" || tokenValue == "internal";
    }

    private AST ParseLocalVariableDeclaration()
    {
        var node = new AST("LocalVariableDeclaration");

        // Handle data types like int, double, string, etc.
        var dataType = ConsumeToken(TokenType.Keyword);
        node.AddChild(new AST("DataType", dataType.Value));

        // Consume the variable name
        var varName = ConsumeToken(TokenType.Identifier);
        node.AddChild(new AST("VariableName", varName.Value));

        // Handle initialization
        ConsumeToken(TokenType.Operator, "=");
        var literal = ParseLiteral();
        node.AddChild(literal);

        // End of statement
        ConsumeToken(TokenType.Punctuation, ";");

        return node;
    }

    private AST ParseBlock()
    {
        var node = new AST("Block");
        ConsumeToken(TokenType.Punctuation, "{");

        while (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type != TokenType.Punctuation && _tokens[_currentIndex].Value != "}")
        {
            if (_tokens[_currentIndex].Type != TokenType.Comment)
            {
                node.AddChild(ParseStatement());
            }
            else
            {
                ConsumeToken(TokenType.Comment);  // Ignore comment tokens
            }
        }

        ConsumeToken(TokenType.Punctuation, "}");
        return node;
    }

    private AST ParseStatement()
    {
        if (_tokens[_currentIndex].Type == TokenType.Keyword && (_tokens[_currentIndex].Value == "int" || _tokens[_currentIndex].Value == "double" || _tokens[_currentIndex].Value == "string"))
        {
            return ParseLocalVariableDeclaration();
        }

        if (_tokens[_currentIndex].Type == TokenType.Identifier)
        {
            return ParseExpressionStatement();
        }

        throw new Exception($"Syntax error: Unexpected token {_tokens[_currentIndex].Value} at Line: {_tokens[_currentIndex].Line}, Column: {_tokens[_currentIndex].Column}");
    }

    private AST ParseExpressionStatement()
    {
        var node = new AST("ExpressionStatement");

        // Handle identifiers and method calls
        var identifier = ConsumeToken(TokenType.Identifier);
        var identifierNode = new AST("Identifier", identifier.Value);

        if (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type == TokenType.Punctuation && _tokens[_currentIndex].Value == ".")
        {
            ConsumeToken(TokenType.Punctuation, ".");
            var methodName = ConsumeToken(TokenType.Identifier);
            var methodNode = new AST("MethodCall", methodName.Value);
            identifierNode.AddChild(methodNode);

            ConsumeToken(TokenType.Punctuation, "(");
            while (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type != TokenType.Punctuation && _tokens[_currentIndex].Value != ")")
            {
                methodNode.AddChild(ParseExpression());
                if (_tokens[_currentIndex].Type == TokenType.Punctuation && _tokens[_currentIndex].Value == ",")
                {
                    ConsumeToken(TokenType.Punctuation, ",");
                }
            }
            ConsumeToken(TokenType.Punctuation, ")");
        }

        node.AddChild(identifierNode);

        // End of statement
        ConsumeToken(TokenType.Punctuation, ";");

        return node;
    }


    private AST ParseExpression()
    {
        var token = _tokens[_currentIndex];

        if (token.Type == TokenType.Identifier)
        {
            ConsumeToken(TokenType.Identifier);
            return new AST("Identifier", token.Value);
        }
        else if (token.Type == TokenType.Literal)
        {
            ConsumeToken(TokenType.Literal);
            return new AST("IntegerLiteral", token.Value);
        }
        else if (token.Type == TokenType.StringLiteral)
        {
            ConsumeToken(TokenType.StringLiteral);
            return new AST("StringLiteral", token.Value);
        }
        else
        {
            throw new Exception($"Unexpected token {token.Value} at Line: {token.Line}, Column: {token.Column}");
        }
    }


    private AST ParseLiteral()
    {
        var token = _tokens[_currentIndex];
        if (token.Type == TokenType.Literal)
        {
            // Handle numeric literals (e.g., 10, 5.5)
            var node = new AST("LiteralExpression", token.Value);
            _currentIndex++;
            return node;
        }
        else if (token.Type == TokenType.StringLiteral)
        {
            // Handle string literals (e.g., "Hello, World!")
            var node = new AST("StringLiteral", token.Value);
            _currentIndex++;
            return node;
        }

        throw new Exception($"Syntax error: Expected literal, found {token.Value} at Line: {token.Line}, Column: {token.Column}");
    }

    private Token ConsumeToken(TokenType type, string value = null)
    {
        if (_currentIndex < _tokens.Count && _tokens[_currentIndex].Type == type && (value == null || _tokens[_currentIndex].Value == value))
        {
            return _tokens[_currentIndex++];
        }
        throw new Exception($"Syntax error: Expected {type} but found {_tokens[_currentIndex].Value} at Line: {_tokens[_currentIndex].Line}, Column: {_tokens[_currentIndex].Column}");
    }
}


public class Program
{
    public static void Main()
    {
        string code = @"
class Program
{
   public static void Main()
   {
     // This is a comment
     /* This is a 
         multiline comment */
     int x = 10;
     double y = 5.5;
     string message = ""Hello, World!"";
     Console.WriteLine(message);
   }
}
";
        //string code = Console.ReadLine();

        LexicalAnalyzer lexer = new LexicalAnalyzer(code);
        List<Token> tokens = lexer.Analyze();
        Console.WriteLine("--------------------Lexer Phase------------------");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        // Step 2: Initialize the parser with the generated tokens.
        Parser parser = new Parser(tokens);

        // Step 3: Parse the tokens to generate the Abstract Syntax Tree (AST).
        AST ast = parser.Parse();

        // Step 4: Display the AST or handle parsing errors.
        if (ast != null)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------Parser Phase------------------");
            Console.WriteLine("AST:");
            ast.Print();
        }
        else
        {
            Console.WriteLine("Parsing failed.");
        }
        Console.ReadKey();
    }
}
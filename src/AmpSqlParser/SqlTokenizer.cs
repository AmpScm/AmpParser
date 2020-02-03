using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlTokenizer : AmpTokenizer<SqlToken, SqlKind>
    {
        public SqlTokenizer(SqlReader reader)
        {
            Reader = reader;
        }

        readonly StringBuilder Buffer = new StringBuilder();

        public SqlReader Reader { get; }

        public SqlDialect Dialect { get; set; }

        protected override IEnumerator<SqlToken> GetTokens()
        {
            int cR;

            while (0 <= (cR = Reader.Read()))
            {
                SqlPosition leadStart = Reader.GetPosition();

                ScanLeadingTrivia(ref cR, out var leadingTrivia);
                if (cR < 0)
                {
                    if (leadingTrivia?.Count > 0)
                        yield return new SqlToken(SqlKind.EndOfStream, "", Reader.GetPosition(), leadingTrivia, trailing: null);
                    yield break;
                }

                char c = (char)cR;

                SqlPosition start = (leadingTrivia?.Count > 0) ? Reader.GetPosition() : leadStart;
                SqlKind kind;
                switch (c)
                {
                    case '\'':
                    case '\"':
                        Buffer.Append(c);

                        kind = (c == '\'') ? SqlKind.IncompleteStringToken : SqlKind.IncompleteQuotedIdentifierToken;

                        while (Reader.Peek() > 0)
                        {
                            cR = Reader.Read();
                            Buffer.Append((char)cR);

                            if (cR == c)
                            {
                                if (Reader.Peek() != c)
                                {
                                    kind = (c == '\'') ? SqlKind.StringToken : SqlKind.QuotedIdentifierToken;
                                    break;
                                }

                                Buffer.Append((char)Reader.Read());
                            }
                        }
                        break;
                    case '[' when IsSqlServer || IsSqlite:
                        Buffer.Append(c);

                        kind = SqlKind.IncompleteQuotedIdentifierToken;

                        while ((cR = Reader.Peek()) > 0)
                        {
                            if (char.IsWhiteSpace((char)cR))
                                break; // Turn into error and continue parsing

                            cR = Reader.Read();
                            Buffer.Append((char)cR);

                            if (cR == ']')
                            {
                                kind = SqlKind.QuotedIdentifierToken;
                                break;
                            }
                        }
                        break;
                    case '!' when Reader.Peek() == '=':
                    case '^' when Reader.Peek() == '=' && IsOracle:
                    case '<' when Reader.Peek() == '>':
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.NotEqualToken;
                        break;
                    case '<': // <, <=, <> and <<
                    case '>': // >, >= and >>
                        Buffer.Append(c);
                        if (Reader.Peek() == '=' || Reader.Peek() == c || (c == '<' && Reader.Peek() == '>'))
                        {
                            Buffer.Append((char)Reader.Read());
                        }
                        switch (Buffer.ToString())
                        {
                            case "<":
                                kind = SqlKind.LessThanToken;
                                break;
                            case ">":
                                kind = SqlKind.GreaterThanToken;
                                break;
                            case "<=":
                                kind = SqlKind.LessThanOrEqualToken;
                                break;
                            case ">=":
                                kind = SqlKind.GreaterThanOrEqualToken;
                                break;
                            case "<<":
                                kind = SqlKind.ShiftLeftToken;
                                break;
                            case ">>":
                                kind = SqlKind.ShiftRightToken;
                                break;
                            case "<>":
                                kind = SqlKind.NotEqualToken;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    case '|' when Reader.Peek() == '|':
                        Buffer.Append("||");
                        Reader.Read();
                        kind = SqlKind.ConcatToken;
                        break;
                    case ';':
                        Buffer.Append(';');
                        kind = SqlKind.SemiColonOperatorToken;
                        break;
                    case ':':
                        Buffer.Append(':');
                        kind = SqlKind.ColonOperatorToken;
                        break;
                    case ',':
                        Buffer.Append(',');
                        kind = SqlKind.CommaToken;
                        break;
                    case '.':
                        Buffer.Append('.');
                        kind = SqlKind.DotToken;
                        break;
                    case '(' when IsOracle && Reader.Peek() == '+' && Reader.Peek(1) == ')':
                        Reader.Read(); // '+'
                        Reader.Read(); // ')'
                        Buffer.Append("(+)");                        
                        kind = SqlKind.OuterJoinToken;
                        break;
                    case '(':
                        Buffer.Append('(');
                        kind = SqlKind.OpenParenToken;
                        break;
                    case ')':
                        Buffer.Append(')');
                        kind = SqlKind.CloseParenToken;
                        break;
                    case '=' when IsSqlServer && Reader.Peek() == '=':
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.EqualOperatorToken;
                        break;
                    case '=' when IsOracle && Reader.Peek() == '>':
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.GreaterThanOrEqualToken;
                        break;
                    case '=' when Reader.Peek() == '<' && IsOracle:
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.LessThanOrEqualToken;
                        break;
                    case '=':
                        Buffer.Append('=');
                        kind = SqlKind.EqualOperatorToken;
                        break;
                    case '[':
                        Buffer.Append('[');
                        kind = SqlKind.OpenBracket;
                        break;
                    case ']':
                        Buffer.Append(']');
                        kind = SqlKind.CloseBracket;
                        break;
                    case '&':
                        Buffer.Append('&');
                        kind = SqlKind.AmpersandToken;
                        break;
                    case '?' when Reader.Peek() == '?' && Reader.Peek(1) == '(':
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.OpenBracket;
                        break;
                    case '?' when Reader.Peek() == '?' && Reader.Peek(1) == ')':
                        Buffer.Append(c);
                        Buffer.Append((char)Reader.Read());
                        Buffer.Append((char)Reader.Read());
                        kind = SqlKind.CloseBracket;
                        break;
                    case '*':
                        Buffer.Append('*');
                        kind = SqlKind.AsteriksOperatorToken;
                        break;
                    case '+':
                        Buffer.Append('+');
                        kind = SqlKind.PlusOperatorToken;
                        break;
                    case '-':
                        Buffer.Append('-');
                        kind = SqlKind.MinusOperatorToken;
                        break;
                    case '/':
                        Buffer.Append('/');
                        kind = SqlKind.DivOperatorToken;
                        break;
                    case '%':
                        Buffer.Append('%');
                        kind = SqlKind.PercentOperatorToken;
                        break;
                    case '~' when IsSqlite: // Sqlite only?
                        Buffer.Append('~');
                        kind = SqlKind.TildeOperatorToken;
                        break;
                    case 'N' when IsOracle && Reader.Peek() == '\'':
                        Buffer.Append(c);
                        Reader.Read();
                        cR = c = '\'';
                        goto case '\'';
                    default:
                        if (char.IsLetter(c) || c == '_')
                        {
                            Buffer.Append(c);

                            while ((cR = Reader.Peek()) > 0
                                && (char.IsLetterOrDigit((char)cR) || cR == '_') || (cR == '$' && IsOracle))
                            {
                                Buffer.Append((char)Reader.Read());
                            }
                            kind = TokenForIdentifier(Buffer.ToString()) ?? SqlKind.IdentifierToken;
                        }
                        else if (char.IsDigit(c) || (c == '-' && Reader.Peek() > 0 && char.IsDigit((char)Reader.Peek())))
                        {
                            Buffer.Append(c);

                            bool gotDot = false;
                            bool gotE = false;

                            while ((cR = Reader.Peek()) > 0
                                  && (char.IsDigit((char)cR)
                                      || (cR == 'E' && !gotE && NumberFollows(1))
                                      || (cR == 'e' && !gotE && NumberFollows(1))
                                      || (cR == '.' && !gotE && !gotDot)))
                            {
                                Buffer.Append((char)Reader.Read());
                                if (cR == '.')
                                    gotDot = true;
                                else if (cR == 'E' || cR == 'e')
                                {
                                    gotE = true;
                                    if (Reader.Peek() == '-')
                                        Buffer.Append((char)Reader.Read());
                                }
                            }

                            kind = (gotDot || gotE) ? SqlKind.DoubleValueToken : SqlKind.NumericValueToken;
                        }
                        else
                        {
                            Buffer.Append(c);
                            kind = SqlKind.UnknownCharToken;
                        }
                        break;
                }
                string text = Buffer.ToString();
                Buffer.Clear();

                var end = Reader.GetPosition();
                ScanTrailingTrivia(out var trailingTrivia);

                yield return new SqlToken(kind, text, start, leadingTrivia, trailingTrivia)
                {
                    IsError = IsErrorKind(kind)
                };
            };
        }

        private bool IsErrorKind(SqlKind kind)
        {
            switch(kind)
            {
                case SqlKind.IncompleteQuotedIdentifierToken:
                case SqlKind.IncompleteStringToken:
                    return true;
            }
            return false;
        }

        private bool NumberFollows(int v)
        {
            int c = Reader.Peek(v);
            if (c >= 0 && char.IsDigit((char)c))
                return true;
            else if (c != '-')
                return false;
            else
                return (Reader.Peek(v + 1) >= 0 && char.IsDigit((char)Reader.Peek(v + 1)));
        }

        private void ScanLeadingTrivia(ref int cR, out List<SqlTrivia> leadingTrivia)
        {
            char c = (char)cR;
            leadingTrivia = new List<SqlTrivia>();

            for (bool moreTrivia = true; moreTrivia;)
            {
                if (char.IsWhiteSpace(c))
                {
                    while (true)
                    {
                        this.Buffer.Append(c);

                        cR = Reader.Peek();
                        if (cR < 0 || !char.IsWhiteSpace((char)cR))
                            break;

                        c = (char)Reader.Read();
                    }
                    leadingTrivia.Add(new SqlTrivia(SqlTriviaKind.Whitespace, Buffer.ToString()));
                    Buffer.Clear();
                }
                else if (c == '/' && Reader.Peek() == '*')
                {
                    Buffer.Append("/*");
                    Reader.Read(); // '*'

                    while (true)
                    {
                        cR = Reader.Read();
                        if (cR < 0)
                            break;
                        c = (char)cR;
                        Buffer.Append(c);

                        if (c == '*' && Reader.Peek() == '/')
                        {
                            Buffer.Append((char)Reader.Read()); // '/'
                            break;
                        }
                    }
                    leadingTrivia.Add(new SqlTrivia(SqlTriviaKind.BlockComment, Buffer.ToString()));
                    Buffer.Clear();
                }
                else if (c == '-' && Reader.Peek() == '-')
                {
                    Buffer.Append("-");

                    while (Reader.Peek() > 0 && Reader.Peek() != '\n')
                    {
                        Buffer.Append((char)Reader.Read());
                    }
                    leadingTrivia.Add(new SqlTrivia(SqlTriviaKind.LineComment, Buffer.ToString()));
                    Buffer.Clear();
                }
                else
                    moreTrivia = false;

                if (moreTrivia)
                {
                    cR = Reader.Read();
                    c = (char)cR;
                }
            }

            if (leadingTrivia.Count == 0)
                leadingTrivia = null;
        }

        private void ScanTrailingTrivia(out List<SqlTrivia> trailingTrivia)
        {
            trailingTrivia = null;

            int cR = Reader.Peek();
            if (cR < 0)
                return;

            for (bool moreTrivia = true; moreTrivia;)
            {
                if (char.IsWhiteSpace((char)cR))
                {
                    while (cR >= 0 && char.IsWhiteSpace((char)cR))
                    {
                        Buffer.Append((char)Reader.Read());

                        if (cR == '\n')
                        {
                            moreTrivia = false; // End of line. Additional trivia is for next token
                            break;
                        }

                        cR = Reader.Peek();
                    }

                    if (trailingTrivia == null)
                        trailingTrivia = new List<SqlTrivia>();

                    trailingTrivia.Add(new SqlTrivia(SqlTriviaKind.Whitespace, Buffer.ToString()));
                    Buffer.Clear();
                }
                else if (cR == '-' && Reader.Peek(1) == '-')
                {
                    Buffer.Append("--");
                    Reader.Read(); // '-'
                    Reader.Read(); // another '-'

                    cR = Reader.Read();
                    while (cR > 0)
                    {
                        Buffer.Append((char)cR);

                        if (cR == '\n')
                        {
                            moreTrivia = false; // End of line. Additional trivia is for next token
                            break;
                        }
                        cR = Reader.Read();
                    }

                    if (trailingTrivia == null)
                        trailingTrivia = new List<SqlTrivia>();

                    trailingTrivia.Add(new SqlTrivia(SqlTriviaKind.LineComment, Buffer.ToString()));
                    Buffer.Clear();
                }
                else if (cR == '/' && Reader.Peek(1) == '*')
                {
                    Buffer.Append("/*");
                    Reader.Read(); // '/'
                    Reader.Read(); // '*'

                    while (true)
                    {
                        cR = Reader.Read();
                        if (cR < 0)
                            break;
                        char c = (char)cR;
                        Buffer.Append(c);

                        if (c == '*' && Reader.Peek() == '/')
                        {
                            Buffer.Append((char)Reader.Read()); // '/'
                            break;
                        }
                    }
                    if (trailingTrivia == null)
                        trailingTrivia = new List<SqlTrivia>();

                    trailingTrivia.Add(new SqlTrivia(SqlTriviaKind.BlockComment, Buffer.ToString()));
                    Buffer.Clear();
                }
                else
                    moreTrivia = false;

                if (moreTrivia)
                    cR = Reader.Peek();
            }


        }

        bool IsOracle => (Dialect == SqlDialect.Oracle);
        bool IsSqlServer => (Dialect == SqlDialect.SqlServer);
        bool IsSqlite => (Dialect == SqlDialect.SqLite);

        protected override SqlKind? TokenForIdentifier(string value)
        {
            switch (value.ToUpperInvariant())
            {
                case "ABSOLUTE": // when IsSql1999:
                    return SqlKind.AbsoluteToken;
                case "ACTION": // when IsSql1999:
                    return SqlKind.ActionToken;
                case "ADD": // when IsSql1999:
                    return SqlKind.AddToken;
                case "ADMIN": // when IsSql1999:
                    return SqlKind.AdminToken;
                case "AFTER": // when IsSql1999:
                    return SqlKind.AfterToken;
                case "AGGREGATE": // when IsSql1999:
                    return SqlKind.AggregateToken;
                case "ALIAS": // when IsSql1999:
                    return SqlKind.Aliastoken;
                case "ALL": // when IsSql1999:
                    return SqlKind.AllToken;
                case "ALLOCATE": // when IsSql1999:
                    return SqlKind.AllocateToken;
                case "ALTER": // when IsSql1999:
                    return SqlKind.AlterToken;
                case "AND": // when IsSql1999:
                    return SqlKind.AndToken;
                case "ANY": // when IsSql1999:
                    return SqlKind.AnyToken;
                case "ARE": // when IsSql1999:
                    return SqlKind.AreToken;
                case "ARRAY": // when IsSql1999:
                    return SqlKind.ArrayToken;
                case "AS": // when IsSql1999:
                    return SqlKind.AsToken;
                case "ASC": // when IsSql1999:
                    return SqlKind.AscToken;
                case "ASSERTION": // when IsSql1999:
                    return SqlKind.AssertionToken;
                case "AT": // when IsSql1999:
                    return SqlKind.AtToken;
                case "AUTHORIZATION": // when IsSql1999:
                    return SqlKind.AuthorizationToken;
                case "BEFORE": // when IsSql1999:
                    return SqlKind.BeforeToken;
                case "BEGIN": // when IsSql1999:
                    return SqlKind.BeginToken;
                case "BINARY": // when IsSql1999:
                    return SqlKind.BinaryToken;
                case "BIT": // when IsSql1999:
                    return SqlKind.BitToken;
                case "BLOB": // when IsSql1999:
                    return SqlKind.BlobToken;
                case "BOOLEAN": // when IsSql1999:
                    return SqlKind.BooleanToken;
                case "BOTH": // when IsSql1999:
                    return SqlKind.BothToken;
                case "BREADTH": // when IsSql1999:
                    return SqlKind.BreadthToken;
                case "BY": // when IsSql1999:
                    return SqlKind.ByToken;
                case "CALL": // when IsSql1999:
                    return SqlKind.CallToken;
                case "CASCADE": // when IsSql1999:
                    return SqlKind.CascadeToken;
                case "CASCADED": // when IsSql1999:
                    return SqlKind.CascadedToken;
                case "CASE": // when IsSql1999:
                    return SqlKind.CaseToken;
                case "CAST": // when IsSql1999:
                    return SqlKind.CastToken;
                case "CATALOG": // when IsSql1999:
                    return SqlKind.CatalogToken;
                case "CHAR": // when IsSql1999:
                    return SqlKind.CharToken;
                case "CHARACTER": // when IsSql1999:
                    return SqlKind.CharacterToken;
                case "CHECK": // when IsSql1999:
                    return SqlKind.CheckToken;
                case "CLASS": // when IsSql1999:
                    return SqlKind.ClassToken;
                case "CLOB": // when IsSql1999:
                    return SqlKind.ClobToken;
                case "CLOSE": // when IsSql1999:
                    return SqlKind.CloseToken;
                case "COLLATE": // when IsSql1999:
                    return SqlKind.Collatetoken;
                case "COLLATION": // when IsSql1999:
                    return SqlKind.CollationToken;
                case "COLUMN": // when IsSql1999:
                    return SqlKind.ColumnToken;
                case "COMMIT": // when IsSql1999:
                    return SqlKind.CommitToken;
                case "COMPLETION": // when IsSql1999:
                    return SqlKind.CompletionToken;
                case "CONNECT": // when IsSql1999:
                    return SqlKind.ConnectToken;
                case "CONNECTION": // when IsSql1999:
                    return SqlKind.ConnectionToken;
                case "CONSTRAINT": // when IsSql1999:
                    return SqlKind.ConstraintToken;
                case "CONSTRAINTS": // when IsSql1999:
                    return SqlKind.ConstraintsToken;
                case "CONSTRUCTOR": // when IsSql1999:
                    return SqlKind.ConstructorToken;
                case "CONTINUE": // when IsSql1999:
                    return SqlKind.ContinueToken;
                case "CORRESPONDING": // when IsSql1999:
                    return SqlKind.CorrespondingToken;
                case "CREATE": // when IsSql1999:
                    return SqlKind.CreateToken;
                case "CROSS": // when IsSql1999:
                    return SqlKind.CrossToken;
                case "CUBE": // when IsSql1999:
                    return SqlKind.CubeToken;
                case "CURRENT": // when IsSql1999:
                    return SqlKind.CurrentToken;
                case "CURRENT_DATE": // when IsSql1999:
                    return SqlKind.CurrentDateToken;
                case "CURRENT_PATH": // when IsSql1999:
                    return SqlKind.CurrentPathToken;
                case "CURRENT_ROLE": // when IsSql1999:
                    return SqlKind.CurrentRoleToken;
                case "CURRENT_TIME": // when IsSql1999:
                    return SqlKind.CurrentTimeToken;
                case "CURRENT_TIMESTAMP": // when IsSql1999:
                    return SqlKind.CurrentTimestampToken;
                case "CURRENT_USER": // when IsSql1999:
                    return SqlKind.CurrentUserToken;
                case "CURSOR": // when IsSql1999:
                    return SqlKind.CursorToken;
                case "CYCLE": // when IsSql1999:
                    return SqlKind.CycleToken;
                case "DATA": // when IsSql1999:
                    return SqlKind.DataToken;
                case "DATE": // when IsSql1999:
                    return SqlKind.DateToken;
                case "DAY": // when IsSql1999:
                    return SqlKind.DayToken;
                case "DEALLOCATE": // when IsSql1999:
                    return SqlKind.DeallocateToken;
                case "DEC": // when IsSql1999:
                    return SqlKind.DecToken;
                case "DECIMAL": // when IsSql1999:
                    return SqlKind.DecimalToken;
                case "DECLARE": // when IsSql1999:
                    return SqlKind.DeclareToken;
                case "DEFAULT": // when IsSql1999:
                    return SqlKind.DefaultToken;
                case "DEFERRABLE": // when IsSql1999:
                    return SqlKind.DeferrableToken;
                case "DEFERRED": // when IsSql1999:
                    return SqlKind.DeferredToken;
                case "DELETE": // when IsSql1999:
                    return SqlKind.DeleteToken;
                case "DEPTH": // when IsSql1999:
                    return SqlKind.DepthToken;
                case "DEREF": // when IsSql1999:
                    return SqlKind.DerefToken;
                case "DESC": // when IsSql1999:
                    return SqlKind.DescToken;
                case "DESCRIBE": // when IsSql1999:
                    return SqlKind.DescribeToken;
                case "DESCRIPTOR": // when IsSql1999:
                    return SqlKind.DescriptorToken;
                case "DESTROY": // when IsSql1999:
                    return SqlKind.DestroyToken;
                case "DESTRUCTOR": // when IsSql1999:
                    return SqlKind.DestructorToken;
                case "DETERMINISTIC": // when IsSql1999:
                    return SqlKind.DeterministicToken;
                case "DICTIONARY": // when IsSql1999:
                    return SqlKind.DictionaryToken;
                case "DIAGNOSTICS": // when IsSql1999:
                    return SqlKind.DiagnosticsToken;
                case "DISCONNECT": // when IsSql1999:
                    return SqlKind.DisconnectToken;
                case "DISTINCT": // when IsSql1999:
                    return SqlKind.DistinctToken;
                case "DOMAIN": // when IsSql1999:
                    return SqlKind.DomainToken;
                case "DOUBLE": // when IsSql1999:
                    return SqlKind.DoubleToken;
                case "DROP": // when IsSql1999:
                    return SqlKind.DropToken;
                case "DYNAMIC": // when IsSql1999:
                    return SqlKind.DynamicToken;
                case "EACH": // when IsSql1999:
                    return SqlKind.EachToken;
                case "ELSE": // when IsSql1999:
                    return SqlKind.ElseToken;
                case "END": // when IsSql1999:
                    return SqlKind.EndToken;
                // "END-EXEC" is part of Sql1999
                case "EQUALS": // when IsSql1999:
                    return SqlKind.EqualsToken;
                case "ESCAPE": // when IsSql1999:
                    return SqlKind.EscapeToken;
                case "EVERY": // when IsSql1999:
                    return SqlKind.EveryToken;
                case "EXCEPT": // when IsSql1999:
                    return SqlKind.ExceptToken;
                case "EXCEPTION": // when IsSql1999:
                    return SqlKind.ExceptionToken;
                case "EXECUTE": // when IsSql1999:
                    return SqlKind.ExecuteToken;
                case "EXTERNAL": // when IsSql1999:
                    return SqlKind.ExternalToken;
                case "FALSE": // when IsSql1999:
                    return SqlKind.FalseToken;
                case "FETCH": // when IsSql1999:
                    return SqlKind.FetchToken;
                case "FIRST": // when IsSql1999:
                    return SqlKind.FirstToken;
                case "FLOAT": // when IsSql1999:
                    return SqlKind.FloatToken;
                case "FOR": // when IsSql1999:
                    return SqlKind.ForToken;
                case "FOREIGN": // when IsSql1999:
                    return SqlKind.ForeignToken;
                case "FOUND": // when IsSql1999:
                    return SqlKind.FoundToken;
                case "FROM": // when IsSql1999:
                    return SqlKind.FromToken;
                case "FREE": // when IsSql1999:
                    return SqlKind.FreeToken;
                case "FULL": // when IsSql1999:
                    return SqlKind.FullToken;
                case "FUNCTION" when !IsOracle: // when IsSql1999:
                    return SqlKind.FunctionToken;
                case "GENERAL": // when IsSql1999:
                    return SqlKind.GeneralToken;
                case "GET": // when IsSql1999:
                    return SqlKind.GetToken;
                case "GLOBAL": // when IsSql1999:
                    return SqlKind.GlobalToken;
                case "GO": // when IsSql1999:
                    return SqlKind.GoToken;
                case "GOTO": // when IsSql1999:
                    return SqlKind.GotoToken;
                case "GRANT": // when IsSql1999:
                    return SqlKind.GrantToken;
                case "GROUP": // when IsSql1999:
                    return SqlKind.GroupToken;
                case "GROUPING": // when IsSql1999:
                    return SqlKind.GroupingToken;
                case "HAVING": // when IsSql1999:
                    return SqlKind.HavingToken;
                case "HOST": // when IsSql1999:
                    return SqlKind.HostToken;
                case "HOUR": // when IsSql1999:
                    return SqlKind.HourToken;
                case "IDENTITY": // when IsSql1999:
                    return SqlKind.IdentityToken;
                case "IGNORE": // when IsSql1999:
                    return SqlKind.IgnoreToken;
                case "IMMEDIATE": // when IsSql1999:
                    return SqlKind.ImmediateToken;
                case "IN": // when IsSql1999:
                    return SqlKind.InToken;
                case "INDICATOR": // when IsSql1999:
                    return SqlKind.IndicatorToken;
                case "INITIALIZE": // when IsSql1999:
                    return SqlKind.InitializeToken;
                case "INITIALLY": // when IsSql1999:
                    return SqlKind.InitiallyToken;
                case "INNER": // when IsSql1999:
                    return SqlKind.InnerToken;
                case "INOUT": // when IsSql1999:
                    return SqlKind.InOutToken;
                case "INPUT": // when IsSql1999:
                    return SqlKind.InputToken;
                case "INSERT": // when IsSql1999:
                    return SqlKind.InsertToken;
                case "INT": // when IsSql1999:
                    return SqlKind.IntToken;
                case "INTEGER": // when IsSql1999:
                    return SqlKind.IntegerToken;
                case "INTERSECT": // when IsSql1999:
                    return SqlKind.IntersectToken;
                case "INTERVAL": // when IsSql1999:
                    return SqlKind.IntervalToken;
                case "INTO": // when IsSql1999:
                    return SqlKind.IntoToken;
                case "IS": // when IsSql1999:
                    return SqlKind.IsToken;
                case "ISOLATION": // when IsSql1999:
                    return SqlKind.IsolationToken;
                case "ITERATE": // when IsSql1999:
                    return SqlKind.IterateToken;
                case "JOIN": // when IsSql1999:
                    return SqlKind.JoinToken;
                case "KEY" when !IsOracle: // when IsSql1999:
                    return SqlKind.KeyToken;
                case "LANGUAGE": // when IsSql1999:
                    return SqlKind.LanguageToken;
                case "LARGE": // when IsSql1999:
                    return SqlKind.LargeToken;
                case "LAST": // when IsSql1999:
                    return SqlKind.LastToken;
                case "LATERAL": // when IsSql1999:
                    return SqlKind.LateralToken;
                case "LEADING": // when IsSql1999:
                    return SqlKind.LeadingToken;
                case "LEFT": // when IsSql1999:
                    return SqlKind.LeftToken;
                case "LESS": // when IsSql1999:
                    return SqlKind.LessToken;
                case "LEVEL": // when IsSql1999:
                    return SqlKind.LevelToken;
                case "LIKE": // when IsSql1999:
                    return SqlKind.LikeToken;
                case "LIMIT": // when IsSql1999:
                    return SqlKind.LimitToken;
                case "LOCAL": // when IsSql1999:
                    return SqlKind.LocalToken;
                case "LOCALTIME": // when IsSql1999:
                    return SqlKind.LocalTimeToken;
                case "LOCALTIMESTAMP": // when IsSql1999:
                    return SqlKind.LocalTimestampToken;
                case "LOCATOR": // when IsSql1999:
                    return SqlKind.LocatorToken;
                case "MAP": // when IsSql1999:
                    return SqlKind.MapToken;
                case "MATCH": // when IsSql1999:
                    return SqlKind.MatchToken;
                case "MINUTE": // when IsSql1999:
                    return SqlKind.MinuteToken;
                case "MODIFIES": // when IsSql1999:
                    return SqlKind.ModifiesToken;
                case "MODIFY": // when IsSql1999:
                    return SqlKind.ModifyToken;
                case "MODULE": // when IsSql1999:
                    return SqlKind.ModuleToken;
                case "MONTH": // when IsSql1999:
                    return SqlKind.Monthtoken;
                case "NAMES": // when IsSql1999:
                    return SqlKind.NamesToken;
                case "NATIONAL": // when IsSql1999:
                    return SqlKind.NationalToken;
                case "NATURAL": // when IsSql1999:
                    return SqlKind.NaturalToken;
                case "NCHAR": // when IsSql1999:
                    return SqlKind.NCharToken;
                case "NCLOB": // when IsSql1999:
                    return SqlKind.NClobToken;
                case "NEW": // when IsSql1999:
                    return SqlKind.NewToken;
                case "NEXT": // when IsSql1999:
                    return SqlKind.NextToken;
                case "NO": // when IsSql1999:
                    return SqlKind.NoToken;
                case "NONE": // when IsSql1999:
                    return SqlKind.NoneToken;
                case "NOT": // when IsSql1999:
                    return SqlKind.NotToken;
                case "NULL": // when IsSql1999:
                    return SqlKind.NullToken;
                case "NUMERIC": // when IsSql1999:
                    return SqlKind.NumericToken;
                case "OBJECT": // when IsSql1999:
                    return SqlKind.ObjectToken;
                case "OF": // when IsSql1999:
                    return SqlKind.OfToken;
                case "OFF": // when IsSql1999:
                    return SqlKind.OffToken;
                case "OLD": // when IsSql1999:
                    return SqlKind.OldToken;
                case "ON": // when IsSql1999:
                    return SqlKind.OnToken;
                case "ONLY": // when IsSql1999:
                    return SqlKind.OnlyToken;
                case "OPEN": // when IsSql1999:
                    return SqlKind.OpenToken;
                case "OPERATION": // when IsSql1999:
                    return SqlKind.OperationToken;
                case "OPTION": // when IsSql1999:
                    return SqlKind.OptionToken;
                case "OR": // when IsSql1999:
                    return SqlKind.OrToken;
                case "ORDER": // when IsSql1999:
                    return SqlKind.OrderToken;
                case "ORDINALITY": // when IsSql1999:
                    return SqlKind.OrdinalityToken;
                case "OUT": // when IsSql1999:
                    return SqlKind.OutToken;
                case "OUTER": // when IsSql1999:
                    return SqlKind.OuterToken;
                case "OUTPUT": // when IsSql1999:
                    return SqlKind.OutputToken;
                case "PAD": // when IsSql1999:
                    return SqlKind.PadToken;
                case "PARAMETER": // when IsSql1999:
                    return SqlKind.ParameterToken;
                case "PARAMETERS": // when IsSql1999:
                    return SqlKind.ParametersToken;
                case "PARTIAL": // when IsSql1999:
                    return SqlKind.PartialToken;
                case "PATH": // when IsSql1999:
                    return SqlKind.PathToken;
                case "POSTFIX": // when IsSql1999:
                    return SqlKind.PostfixToken;
                case "PRECISION" when !IsOracle: // when IsSql1999:
                    return SqlKind.PrecisionToken;
                case "PREFIX": // when IsSql1999:
                    return SqlKind.PrefixToken;
                case "PREORDER": // when IsSql1999:
                    return SqlKind.PreOrderToken;
                case "PREPARE": // when IsSql1999:
                    return SqlKind.PrepareToken;
                case "PRESERVE": // when IsSql1999:
                    return SqlKind.PreserveToken;
                case "PRIMARY": // when IsSql1999:
                    return SqlKind.PrimaryToken;
                case "PRIOR": // when IsSql1999:
                    return SqlKind.PriorToken;
                case "PRIVILEGES": // when IsSql1999:
                    return SqlKind.PrivilegesToken;
                case "PROCEDURE": // when IsSql1999:
                    return SqlKind.ProcedureToken;
                case "PUBLIC": // when IsSql1999:
                    return SqlKind.PublicToken;
                case "READ": // when IsSql1999:
                    return SqlKind.ReadToken;
                case "READS": // when IsSql1999:
                    return SqlKind.ReadsToken;
                case "REAL": // when IsSql1999:
                    return SqlKind.RealToken;
                case "RECURSIVE": // when IsSql1999:
                    return SqlKind.RecursiveToken;
                case "REF": // when IsSql1999:
                    return SqlKind.RefToken;
                case "REFERENCES": // when IsSql1999:
                    return SqlKind.ReferencesToken;
                case "REFERENCING": // when IsSql1999:
                    return SqlKind.ReferencingToken;
                case "RELATIVE": // when IsSql1999:
                    return SqlKind.RelativeToken;
                case "RESTRICT": // when IsSql1999:
                    return SqlKind.RestrictToken;
                case "RESULT": // when IsSql1999:
                    return SqlKind.ResultToken;
                case "RETURN": // when IsSql1999:
                    return SqlKind.ReturnToken;
                case "RETURNS": // when IsSql1999:
                    return SqlKind.ReturnsToken;
                case "REVOKE": // when IsSql1999:
                    return SqlKind.RevokeToken;
                case "RIGHT": // when IsSql1999:
                    return SqlKind.RightToken;
                case "ROLE": // when IsSql1999:
                    return SqlKind.RoleToken;
                case "ROLLBACK": // when IsSql1999:
                    return SqlKind.RollBackToken;
                case "ROLLUP": // when IsSql1999:
                    return SqlKind.RollUpToken;
                case "ROUTINE": // when IsSql1999:
                    return SqlKind.RoutineToken;
                case "ROW": // when IsSql1999:
                    return SqlKind.RowToken;
                case "ROWS": // when IsSql1999:
                    return SqlKind.RowsToken;
                case "SAVEPOINT": // when IsSql1999:
                    return SqlKind.SavepointToken;
                case "SCHEMA": // when IsSql1999:
                    return SqlKind.SchemaToken;
                case "SCROLL": // when IsSql1999:
                    return SqlKind.ScrollToken;
                case "SCOPE": // when IsSql1999:
                    return SqlKind.ScopeToken;
                case "SEARCH": // when IsSql1999:
                    return SqlKind.SearchToken;
                case "SECOND": // when IsSql1999:
                    return SqlKind.SecondToken;
                case "SECTION" when !IsOracle: // when IsSql1999:
                    return SqlKind.SectionToken;
                case "SELECT": // when IsSql1999:
                    return SqlKind.SelectToken;
                case "SEQUENCE": // when IsSql1999:
                    return SqlKind.SequenceToken;
                case "SESSION": // when IsSql1999:
                    return SqlKind.SessionToken;
                case "SESSION_USER": // when IsSql1999:
                    return SqlKind.SessionUserToken;
                case "SET": // when IsSql1999:
                    return SqlKind.SetToken;
                case "SETS": // when IsSql1999:
                    return SqlKind.SetsToken;
                case "SIZE": // when IsSql1999:
                    return SqlKind.SizeToken;
                case "SMALLINT": // when IsSql1999:
                    return SqlKind.SmallIntToken;
                case "SOME": // when IsSql1999:
                    return SqlKind.SomeToken;
                case "SPACE": // when IsSql1999:
                    return SqlKind.SpaceToken;
                case "SPECIFIC": // when IsSql1999:
                    return SqlKind.SpecificToken;
                case "SPECIFICTYPE": // when IsSql1999:
                    return SqlKind.SpecificTypeToken;
                case "SQL": // when IsSql1999:
                    return SqlKind.SqlToken;
                case "SQLEXCEPTION": // when IsSql1999:
                    return SqlKind.SqlExceptionToken;
                case "SQLSTATE": // when IsSql1999:
                    return SqlKind.SqlStateToken;
                case "SQLWARNING": // when IsSql1999:
                    return SqlKind.SqlWarningToken;
                case "START": // when IsSql1999:
                    return SqlKind.StartToken;
                case "STATE" when !IsOracle: // when IsSql1999:
                    return SqlKind.StateToken;
                case "STATEMENT": // when IsSql1999:
                    return SqlKind.StatementToken;
                case "STATIC": // when IsSql1999:
                    return SqlKind.StaticToken;
                case "STRUCTURE": // when IsSql1999:
                    return SqlKind.StructureToken;
                case "SYSTEM_USER": // when IsSql1999:
                    return SqlKind.SystemUserToken;
                case "TABLE": // when IsSql1999:
                    return SqlKind.TableToken;
                case "TEMPORARY": // when IsSql1999:
                    return SqlKind.TemporaryToken;
                case "TERMINATE": // when IsSql1999:
                    return SqlKind.TerminateToken;
                case "THAN": // when IsSql1999:
                    return SqlKind.ThanToken;
                case "THEN": // when IsSql1999:
                    return SqlKind.ThenToken;
                case "TIME": // when IsSql1999:
                    return SqlKind.TimeToken;
                case "TIMESTAMP": // when IsSql1999:
                    return SqlKind.TimestampToken;
                case "TIMEZONE_HOUR": // when IsSql1999:
                    return SqlKind.TimeZoneHourToken;
                case "TIMEZONE_MINUTE": // when IsSql1999:
                    return SqlKind.TimeZoneMinuteToken;
                case "TO": // when IsSql1999:
                    return SqlKind.ToToken;
                case "TRAILING": // when IsSql1999:
                    return SqlKind.TrailingToken;
                case "TRANSACTION": // when IsSql1999:
                    return SqlKind.TransactionToken;
                case "TRANSLATION": // when IsSql1999:
                    return SqlKind.TranslationToken;
                case "TREAT": // when IsSql1999:
                    return SqlKind.TreatToken;
                case "TRIGGER": // when IsSql1999:
                    return SqlKind.TriggerToken;
                case "TRUE": // when IsSql1999:
                    return SqlKind.TrueToken;
                case "UNDER": // when IsSql1999:
                    return SqlKind.UnderToken;
                case "UNION": // when IsSql1999:
                    return SqlKind.UnionToken;
                case "UNIQUE": // when IsSql1999:
                    return SqlKind.UniqueToken;
                case "UNKNOWN": // when IsSql1999:
                    return SqlKind.UnknownToken;
                case "UNNEST": // when IsSql1999:
                    return SqlKind.UnNestToken;
                case "UPDATE": // when IsSql1999:
                    return SqlKind.UpdateToken;
                case "USAGE": // when IsSql1999:
                    return SqlKind.UsageToken;
                case "USER": // when IsSql1999:
                    return SqlKind.UserToken;
                case "USING": // when IsSql1999:
                    return SqlKind.UsingToken;
                case "VALUE" when !IsOracle: // when IsSql1999:
                    return SqlKind.ValueToken;
                case "VALUES": // when IsSql1999:
                    return SqlKind.ValuesToken;
                case "VARCHAR": // when IsSql1999:
                    return SqlKind.VarCharToken;
                case "VARIABLE": // when IsSql1999:
                    return SqlKind.VariableToken;
                case "VARYING": // when IsSql1999:
                    return SqlKind.VaryingToken;
                case "VIEW": // when IsSql1999:
                    return SqlKind.ViewToken;
                case "WHEN": // when IsSql1999:
                    return SqlKind.WhenToken;
                case "WHENEVER": // when IsSql1999:
                    return SqlKind.WheneverToken;
                case "WHERE": // when IsSql1999:
                    return SqlKind.WhereToken;
                case "WITH": // when IsSql1999:
                    return SqlKind.WithToken;
                case "WITHOUT": // when IsSql1999:
                    return SqlKind.WithoutToken;
                case "WORK": // when IsSql1999:
                    return SqlKind.WorkToken;
                case "WRITE": // when IsSql1999:
                    return SqlKind.WriteToken;
                case "YEAR": // when IsSql1999:
                    return SqlKind.YearToken;
                case "ZONE" when !IsOracle:// when IsSql1999:
                    return SqlKind.ZoneToken;

                // SQLServer 2019
                case "FILE" when IsSqlServer:
                    return SqlKind.FileToken;
                case "RAISERROR" when IsSqlServer:
                    return SqlKind.RaiseErrorToken;
                case "FILLFACTOR" when IsSqlServer:
                    return SqlKind.FillFactorToken;
                case "READTEXT" when IsSqlServer:
                    return SqlKind.ReadTextToken;
                case "RECONFIGURE" when IsSqlServer:
                    return SqlKind.ReconfigureToken;
                case "FREETEXT" when IsSqlServer:
                    return SqlKind.FreeTextToken;
                case "FREETEXTTABLE" when IsSqlServer:
                    return SqlKind.FreeTextTableToken;
                case "REPLICATION" when IsSqlServer:
                    return SqlKind.ReplicationToken;
                case "BACKUP" when IsSqlServer:
                    return SqlKind.BackupToken;
                case "RESTORE" when IsSqlServer:
                    return SqlKind.RestoreToken;
                case "BETWEEN" when IsSqlServer:
                    return SqlKind.BetweenToken;
                case "BREAK" when IsSqlServer:
                    return SqlKind.BreakToken;
                case "REVERT" when IsSqlServer:
                    return SqlKind.RevertToken;
                case "BROWSE" when IsSqlServer:
                    return SqlKind.BrowseToken;
                case "BULK" when IsSqlServer:
                    return SqlKind.BulkToken;
                case "HOLDLOCK" when IsSqlServer:
                    return SqlKind.HoldLockToken;
                case "ROWCOUNT" when IsSqlServer:
                    return SqlKind.RowCountToken;
                case "ROWGUIDCOL" when IsSqlServer:
                    return SqlKind.RowGuidColToken;
                case "IDENTITY_INSERT" when IsSqlServer:
                    return SqlKind.IdentityInsertToken;
                case "RULE" when IsSqlServer:
                    return SqlKind.RuleToken;
                case "CHECKPOINT" when IsSqlServer:
                    return SqlKind.CheckpointToken;
                case "IDENTITYCOL" when IsSqlServer:
                    return SqlKind.IdentityColToken;
                case "SAVE" when IsSqlServer:
                    return SqlKind.SaveToken;
                case "IF" when IsSqlServer:
                    return SqlKind.IfToken;
                case "CLUSTERED" when IsSqlServer:
                    return SqlKind.ClusteredToken;
                case "SECURITYAUDIT" when IsSqlServer:
                    return SqlKind.SecurityAuditToken;
                case "COALESCE" when IsSqlServer:
                    return SqlKind.CoalesceToken;
                case "INDEX" when IsSqlServer:
                    return SqlKind.IndexToken;
                case "SEMANTICKEYPHRASETABLE" when IsSqlServer:
                    return SqlKind.SemanticKeyPhraseTableToken;
                case "SEMANTICSIMILARITYDETAILSTABLE" when IsSqlServer:
                    return SqlKind.SemanticSimilarityDetailsTableToken;
                case "SEMANTICSIMILARITYTABLE" when IsSqlServer:
                    return SqlKind.SemanticSimilarityTableToken;
                case "COMPUTE" when IsSqlServer:
                    return SqlKind.ComputeToken;
                case "CONTAINS" when IsSqlServer:
                    return SqlKind.ContainsToken;
                case "SETUSER" when IsSqlServer:
                    return SqlKind.SetUserToken;
                case "CONTAINSTABLE" when IsSqlServer:
                    return SqlKind.ContainsTableToken;
                case "SHUTDOWN" when IsSqlServer:
                    return SqlKind.ShutdownToken;
                case "KILL" when IsSqlServer:
                    return SqlKind.KillToken;
                case "CONVERT" when IsSqlServer:
                    return SqlKind.ConvertToken;
                case "STATISTICS" when IsSqlServer:
                    return SqlKind.StatisticsToken;
                case "LINENO" when IsSqlServer:
                    return SqlKind.LineNoToken;
                case "LOAD" when IsSqlServer:
                    return SqlKind.LoadToken;
                case "TABLESAMPLE" when IsSqlServer:
                    return SqlKind.TableSampleToken;
                case "MERGE" when IsSqlServer:
                    return SqlKind.MergeToken;
                case "TEXTSIZE" when IsSqlServer:
                    return SqlKind.TextSizeToken;
                case "NOCHECK" when IsSqlServer:
                    return SqlKind.NoCheckToken;
                case "NONCLUSTERED" when IsSqlServer:
                    return SqlKind.NonClusteredToken;
                case "TOP" when IsSqlServer:
                    return SqlKind.TopToken;
                case "TRAN" when IsSqlServer:
                    return SqlKind.TranToken;
                case "DATABASE" when IsSqlServer:
                    return SqlKind.DatabaseToken;
                case "DBCC" when IsSqlServer:
                    return SqlKind.DbccToken;
                case "NULLIF" when IsSqlServer:
                    return SqlKind.NullIfToken;
                case "TRUNCATE" when IsSqlServer:
                    return SqlKind.TruncateToken;
                case "TRY_CONVERT" when IsSqlServer:
                    return SqlKind.TryConvertToken;
                case "OFFSETS" when IsSqlServer:
                    return SqlKind.OffsetsToken;
                case "TSEQUAL" when IsSqlServer:
                    return SqlKind.TSequalToken;
                case "DENY" when IsSqlServer:
                    return SqlKind.DenyToken;
                case "OPENDATASOURCE" when IsSqlServer:
                    return SqlKind.OpenDataSourceToken;
                case "UNPIVOT" when IsSqlServer:
                    return SqlKind.UniPivotToken;
                case "DISK" when IsSqlServer:
                    return SqlKind.DiskToken;
                case "OPENQUERY" when IsSqlServer:
                    return SqlKind.OpenQueryToken;
                case "OPENROWSET" when IsSqlServer:
                    return SqlKind.OpenRowSetToken;
                case "UPDATETEXT" when IsSqlServer:
                    return SqlKind.UpdateTextToken;
                case "DISTRIBUTED" when IsSqlServer:
                    return SqlKind.DistributedToken;
                case "OPENXML" when IsSqlServer:
                    return SqlKind.OpenXmlToken;
                case "USE" when IsSqlServer:
                    return SqlKind.UseToken;
                case "DUMP" when IsSqlServer:
                    return SqlKind.DumpToken;
                case "OVER" when IsSqlServer:
                    return SqlKind.OverToken;
                case "WAITFOR" when IsSqlServer:
                    return SqlKind.WaitForToken;
                case "ERRLVL" when IsSqlServer:
                    return SqlKind.ErrorLvlToken;
                case "PERCENT" when IsSqlServer:
                    return SqlKind.PercentToken;
                case "PIVOT" when IsSqlServer:
                    return SqlKind.PivotToken;
                case "PLAN" when IsSqlServer:
                    return SqlKind.PlanToken;
                case "WHILE" when IsSqlServer:
                    return SqlKind.WhileToken;
                case "EXEC" when IsSqlServer:
                    return SqlKind.ExecToken;
                case "WITHIN" when IsSqlServer:
                    return SqlKind.WithinToken;
                case "EXISTS" when IsSqlServer:
                    return SqlKind.ExistsToken;
                case "PRINT" when IsSqlServer:
                    return SqlKind.PrintToken;
                case "WRITETEXT" when IsSqlServer:
                    return SqlKind.WriteTextToken;
                case "EXIT" when IsSqlServer:
                    return SqlKind.ExitToken;

                case "BETWEEN" when IsOracle:
                    return SqlKind.BetweenToken;
                case "EXISTS" when IsOracle:
                    return SqlKind.ExistsToken;
                case "INDEX" when IsOracle:
                    return SqlKind.IndexToken;

                // Generic?
                case "SYSDATE" when IsOracle:
                    return SqlKind.SysDateToken;
                // ORACLE
                case "MINUS" when IsOracle:
                    return SqlKind.MinusSetToken;
                case "REPLACE" when IsOracle:
                    return SqlKind.ReplaceToken;

                case "EDITIONABLE" when IsOracle:

                // O
                case "ACCESS" when IsOracle:
                    return SqlKind.AccessToken;
                case "EXCLUSIVE" when IsOracle:
                    return SqlKind.ExclusiveToken;
                case "NOAUDIT" when IsOracle:
                    return SqlKind.NoAuditToken;
                case "NOCOMPRESS" when IsOracle:
                    return SqlKind.NoCompressToken;

                case "FILE" when IsOracle:
                    return SqlKind.FileToken;
                case "NOTFOUND" when IsOracle:
                    return SqlKind.ShareToken;
                case "SHARE" when IsOracle:
                    return SqlKind.ShareToken;
                case "NOWAIT" when IsOracle:
                    return SqlKind.NoWaitToken;
                case "ARRAYLEN" when IsOracle:
                    return SqlKind.ArrayLenToken;
                case "NUMBER" when IsOracle:
                    return SqlKind.NumberToken;
                case "SQLBUF" when IsOracle:
                    return SqlKind.SqlBufToken;
                case "SUCCESSFUL" when IsOracle:
                    return SqlKind.SuccessfulToken;
                case "AUDIT" when IsOracle:
                    return SqlKind.AuditToken;
                case "OFFLINE" when IsOracle:
                    return SqlKind.OfflineToken;
                case "SYNONYM" when IsOracle:
                    return SqlKind.SynonymToken;
                case "IDENTIFIED" when IsOracle:
                    return SqlKind.IdentifiedToken;
                case "ONLINE" when IsOracle:
                    return SqlKind.OnlineToken;
                case "INCREMENT" when IsOracle:
                    return SqlKind.IncrementToken;
                case "CLUSTER" when IsOracle:
                    return SqlKind.ClusterToken;
                case "INITIAL" when IsOracle:
                    return SqlKind.InitialToken;
                case "PCTFREE" when IsOracle:
                    return SqlKind.PctFreeToken;
                case "UID" when IsOracle:
                    return SqlKind.UidToken;
                case "COMMENT" when IsOracle:
                    return SqlKind.CommentToken;
                case "COMPRESS" when IsOracle:
                    return SqlKind.CompressToken;
                case "RAW" when IsOracle:
                    return SqlKind.RawToken;
                case "RENAME" when IsOracle:
                    return SqlKind.RenameToken;
                case "VALIDATE" when IsOracle:
                    return SqlKind.ValidateToken;
                case "RESOURCE" when IsOracle:
                    return SqlKind.ResourceToken;
                case "LOCK" when IsOracle:
                    return SqlKind.LockToken;
                case "VARCHAR2" when IsOracle:
                    return SqlKind.VarChar2Token;

                case "LONG" when IsOracle:
                    return SqlKind.LongToken;
                case "ROWID" when IsOracle:
                    return SqlKind.RowIdToken;

                case "MAXEXTENTS" when IsOracle:
                    return SqlKind.MaxExtendsToken;
                case "ROWLABEL" when IsOracle:
                    return SqlKind.RowLabelToken;
                case "ROWNUM" when IsOracle:
                    return SqlKind.RowNumToken;
                case "MODE" when IsOracle:
                    return SqlKind.ModeToken;


                case "ABORT" when IsSqlite:
                    return SqlKind.AbortToken;
                case "ANALYZE" when IsSqlite:
                    return SqlKind.AnalyzeToken;
                case "ATTACH" when IsSqlite:
                    return SqlKind.AttachToken;
                case "AUTOINCREMENT" when IsSqlite:
                    return SqlKind.AutoIncrementToken;
                case "BETWEEN" when IsSqlite:
                    return SqlKind.BetweenToken;
                case "CONFLICT" when IsSqlite:
                    return SqlKind.ConflictToken;
                case "DATABASE" when IsSqlite:
                    return SqlKind.DatabaseToken;
                case "DETACH" when IsSqlite:
                    return SqlKind.DetachToken;
                case "DO" when IsSqlite:
                    return SqlKind.DoToken;
                case "EXCLUDE" when IsSqlite:
                    return SqlKind.ExcludeToken;
                case "EXCLUSIVE" when IsSqlite:
                    return SqlKind.ExclusiveToken;
                case "EXISTS" when IsSqlite:
                    return SqlKind.ExistsToken;
                case "EXPLAIN" when IsSqlite:
                    return SqlKind.ExplainToken;
                case "FAIL" when IsSqlite:
                    return SqlKind.FailToken;
                case "FILTER" when IsSqlite:
                    return SqlKind.FilterToken;
                case "FOLLOWING" when IsSqlite:
                    return SqlKind.FollowingToken;
                case "GLOB" when IsSqlite:
                    return SqlKind.GlobToken;
                case "GROUPS" when IsSqlite:
                    return SqlKind.GroupsToken;
                case "IF" when IsSqlite:
                    return SqlKind.IfToken;
                case "INDEX" when IsSqlite:
                    return SqlKind.IndexToken;
                case "INDEXED" when IsSqlite:
                    return SqlKind.IndexedToken;
                case "INSTEAD" when IsSqlite:
                    return SqlKind.InsteadToken;
                case "ISNULL" when IsSqlite:
                    return SqlKind.IsNullToken;
                case "NOTHING" when IsSqlite:
                    return SqlKind.NothingToken;
                case "NOTNULL" when IsSqlite:
                    return SqlKind.NotNullToken;
                case "NULLS" when IsSqlite:
                    return SqlKind.NullsToken;
                case "OFFSET" when IsSqlite:
                    return SqlKind.OffsetToken;
                case "OTHERS" when IsSqlite:
                    return SqlKind.OthersToken;
                case "OVER" when IsSqlite:
                    return SqlKind.OverToken;
                case "PARTITION" when IsSqlite:
                    return SqlKind.PartitionToken;
                case "PLAN" when IsSqlite:
                    return SqlKind.PlanToken;
                case "PRAGMA" when IsSqlite:
                    return SqlKind.PragmaToken;
                case "PRECEDING" when IsSqlite:
                    return SqlKind.PrecedingToken;
                case "QUERY" when IsSqlite:
                    return SqlKind.QueryToken;
                case "RAISE" when IsSqlite:
                    return SqlKind.RaiseToken;
                case "RANGE" when IsSqlite:
                    return SqlKind.RangeToken;
                case "REGEXP" when IsSqlite:
                    return SqlKind.RegExpToken;
                case "REINDEX" when IsSqlite:
                    return SqlKind.ReIndexToken;
                case "RELEASE" when IsSqlite:
                    return SqlKind.ReleaseToken;
                case "RENAME" when IsSqlite:
                    return SqlKind.RenameToken;
                case "REPLACE" when IsSqlite:
                    return SqlKind.ReplaceToken;
                case "TEMP" when IsSqlite:
                    return SqlKind.TempToken;
                case "TIES" when IsSqlite:
                    return SqlKind.TiesToken;
                case "UNBOUNDED" when IsSqlite:
                    return SqlKind.UnboundedToken;
                case "VACUUM" when IsSqlite:
                    return SqlKind.VacuumToken;
                case "VIRTUAL" when IsSqlite:
                    return SqlKind.VirtualToken;
                case "WINDOW" when IsSqlite:
                    return SqlKind.WindowToken;
                default:
                    return null;
            }
        }
    }
}

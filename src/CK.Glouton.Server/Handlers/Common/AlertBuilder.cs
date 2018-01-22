using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CK.Glouton.Server.Handlers.Common
{
    public static class AlertBuilder
    {
        private static readonly ParameterExpression Parameter = Expression.Parameter( typeof( AlertEntry ), "l" );

        private static readonly CKTraitContext TraitContext = new CKTraitContext( "AlertParsing", ';' );

        private static readonly Dictionary<Operation, Func<Expression, Expression, Expression>> Expressions = new Dictionary<Operation, Func<Expression, Expression, Expression>>
        {
            { Operation.EqualTo, Expression.Equal },
            { Operation.NotEqualTo, Expression.NotEqual },
            { Operation.GreaterThan, Expression.GreaterThan },
            { Operation.GreaterThanOrEqualTo, Expression.GreaterThanOrEqual },
            { Operation.LessThan, Expression.LessThan },
            { Operation.LessThanOrEqualTo, Expression.LessThanOrEqual },
            { Operation.Contains, (member, constant) => Expression.Call( member, typeof(string).GetMethod( "Contains" ), constant ) },
            { Operation.StartsWith, (member, constant) => Expression.Call( member, typeof(string).GetMethod( "StartsWith", new[] { typeof(string) } ), constant ) },
            { Operation.EndsWith, (member, constant) => Expression.Call( member, typeof(string).GetMethod( "EndsWith", new[] { typeof(string) } ), constant ) },
            { Operation.In, HasKey },
            { Operation.IsNotNull, (member, constant) => Expression.NotEqual( member, Expression.Constant(null) ) },
        };

        /// <summary>
        /// Refer to <see cref="EField"/> for indexes.
        /// </summary>
        private static readonly Operation[] AllowedOperations = {
            Operation.EqualTo | Operation.NotEqualTo | Operation.In,
            Operation.EqualTo | Operation.NotEqualTo | Operation.Contains | Operation.StartsWith | Operation.EndsWith,
            Operation.EqualTo | Operation.NotEqualTo | Operation.GreaterThan | Operation.GreaterThanOrEqualTo | Operation.LessThan | Operation.LessThanOrEqualTo,
            Operation.EqualTo
        };

        /// <summary>
        /// Indexes for <see cref="AllowedOperations"/>.
        /// </summary>
        private enum EField
        {
            Enum,
            String,
            Int,
            Trait
        }

        private static Expression HasKey( Expression member, Expression expression )
        {
            switch( expression )
            {
                // LogEntryType isn't a Flag enum.
                case ConstantExpression constant when constant.Value is LogEntryType:
                    return Expression.Equal( member, constant );

                // LogLevel is a Flag enum.
                case ConstantExpression constant when constant.Value is LogLevel:
                    var convertedConstant = Expression.Convert( constant, typeof( int ) );
                    return Expression.Equal( Expression.And( Expression.Convert( member, typeof( int ) ), convertedConstant ), convertedConstant );

                default:
                    throw new ArgumentException( nameof( expression ) );
            }
        }

        private static Operation ParseOperation( string value ) => Enum.TryParse( value, out Operation operation )
            ? operation
            : throw new ArgumentException( $"{nameof( operation )} {value} is invalid." );

        /// <summary>
        /// Build a <see cref="Func{TResult}"/> from a given <see cref="IExpressionModel"/> array.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Will be thrown if an enum cannot be parsed.</exception>
        /// <exception cref="InvalidOperationException">Will be thrown if an invalid field is encountered.</exception>
        public static Func<AlertEntry, bool> Build( this IExpressionModel[] @this )
        {
            Expression expression = null;
            foreach( var alertExpression in @this )
            {
                var member = alertExpression.Field.IndexOf( '.' ) == -1
                    ? Expression.Property( Parameter, alertExpression.Field )
                    : BuildMember( alertExpression.Field );

                ConstantExpression constant;
                EField field;

                switch( alertExpression.Field )
                {
                    case "LogType":
                        if( !Enum.TryParse( alertExpression.Body, out LogEntryType logEntryType ) || logEntryType == 0 )
                            throw new ArgumentException( $"{nameof( logEntryType )} {alertExpression.Body} is invalid." );
                        constant = Expression.Constant( logEntryType );
                        field = EField.Enum;
                        break;

                    case "LogLevel":
                        if( !Enum.TryParse( alertExpression.Body, out LogLevel logLevel ) || logLevel == 0 )
                            throw new ArgumentException( $"{nameof( logLevel )} {alertExpression.Body} is invalid." );
                        constant = Expression.Constant( logLevel );
                        field = EField.Enum;
                        break;

                    case "GroupDepth":
                    case "LineNumber":
                        constant = Expression.Constant( int.Parse( alertExpression.Body ) );
                        field = EField.Int;
                        break;

                    case "FileName":
                    case "AppName":
                    case "Text":
                    // TODO: Fix exceptions
                    case "Exception.Message":
                    case "Exception.StackTrace":
                        constant = Expression.Constant( alertExpression.Body );
                        field = EField.String;
                        break;

                    case "Tags":
                        constant = Expression.Constant( TraitContext.FindOrCreate( alertExpression.Body ) );
                        field = EField.Trait;
                        break;

                    default:
                        throw new InvalidOperationException
                            ( $"{nameof( alertExpression.Field )} {alertExpression.Field} is invalid." );
                }

                var operation = ParseOperation( alertExpression.Operation );
                if( !AllowedOperations[ (int)field ].HasFlag( operation ) )
                    throw new InvalidOperationException
                        ( $"{nameof( alertExpression.Operation )} {alertExpression.Operation} is invalid for field {alertExpression.Field}." );

                expression = expression == null
                    ? Expressions[ operation ].Invoke( member, constant )
                    : Expression.And( expression, Expressions[ operation ].Invoke( member, constant ) );
            }

            return Expression.Lambda<Func<AlertEntry, bool>>( expression, Parameter ).Compile();
        }

        private static MemberExpression BuildMember( string field )
        {
            var fields = field.Split( '.' );
            if( fields.Length == 1 )
                throw new ArgumentException( $"{nameof( field )} is invalid" );

            return fields.Aggregate<string, MemberExpression>( null, ( current, expression ) => Expression.Property( (Expression)current ?? Parameter, expression ) );
        }
    }
}
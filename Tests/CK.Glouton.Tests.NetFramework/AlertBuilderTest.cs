using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Service.Common;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class AlertBuilderTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
        }

        [Test]
        public void Build()
        {
            var model = new IExpressionModel[]
            {
                // LogType
                new ExpressionModelTest { Field = "LogType", Operation = "EqualTo", Body = "Line" },
                new ExpressionModelTest { Field = "LogType", Operation = "In", Body = "Line" },

                // Log Level
                new ExpressionModelTest { Field = "LogLevel", Operation = "EqualTo", Body = "Info" },
                new ExpressionModelTest { Field = "LogLevel", Operation = "In", Body = "Info" },

                // String Field
                new ExpressionModelTest { Field = "FileName", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "AppName", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Exception.Message", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Exception.StackTrace", Operation = "EqualTo", Body = "Test" },
                // Operations
                new ExpressionModelTest { Field = "Text", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "NotEqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "Contains", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "StartsWith", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "EndsWith", Body = "Test" },

                // Int Field
                new ExpressionModelTest { Field = "LineNumber", Operation = "EqualTo", Body = "42" },
                new ExpressionModelTest { Field = "GroupDepth", Operation = "EqualTo", Body = "42" },
                // Operations
                new ExpressionModelTest { Field = "LineNumber", Operation = "EqualTo", Body = "42" },
                new ExpressionModelTest { Field = "LineNumber", Operation = "NotEqualTo", Body = "42" },
                new ExpressionModelTest { Field = "LineNumber", Operation = "GreaterThan", Body = "42" },
                new ExpressionModelTest { Field = "LineNumber", Operation = "GreaterThanOrEqualTo", Body = "42" },
                new ExpressionModelTest { Field = "LineNumber", Operation = "LessThan", Body = "42" },
                new ExpressionModelTest { Field = "LineNumber", Operation = "LessThanOrEqualTo", Body = "42" },

                // CK Trait
                new ExpressionModelTest { Field = "Tags", Operation = "EqualTo", Body = "Test" }
            };

            Action build = () => model.Build();
            build.ShouldNotThrow();
        }

        internal class ExpressionModelTest : IExpressionModel
        {
            public string Field { get; set; }
            public string Operation { get; set; }
            public string Body { get; set; }
        }
    }
}

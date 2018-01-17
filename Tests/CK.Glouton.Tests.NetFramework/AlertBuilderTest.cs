using System;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Service.Common;
using FluentAssertions;
using NUnit.Framework;

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
                // TODO: Add all cases

                // Test all string fields
                new ExpressionModelTest { Field = "FileName", Operation = "EqualTo", Body = "Test" },
                //new ExpressionModelTest { Field = "AppName", Operation = "EqualTo", Body = "Test" }, // Issue ?
                new ExpressionModelTest { Field = "Text", Operation = "EqualTo", Body = "Test" },
                //new ExpressionModelTest { Field = "Tags", Operation = "EqualTo", Body = "Test" }, // Inside Issue
                new ExpressionModelTest { Field = "Exception.Message", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Exception.StackTrace", Operation = "EqualTo", Body = "Test" },


                // Test every operator on string fields:
                new ExpressionModelTest { Field = "Text", Operation = "EqualTo", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "Contains", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "StartsWith", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "EndsWith", Body = "Test" },
                new ExpressionModelTest { Field = "Text", Operation = "NotEqualTo", Body = "Test" }
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
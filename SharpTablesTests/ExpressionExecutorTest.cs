using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTables;
using System.Collections.Generic;

namespace SharpTablesTests
{
    [TestClass]
    public class ExpressionExecutorTest
    {
        [TestMethod]
        public void BasicMathOperations()
        {
            SharpTables.ExpressionExecutor executor = new ExpressionExecutor(
                "2 + 2 * 3",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual(8, executor.Execute().ToCellNumberValue().Value);

           executor = new ExpressionExecutor(
                "10 / 2",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual(5, executor.Execute().ToCellNumberValue().Value);
        }

        [TestMethod]
        public void StringsOperation()
        {
            SharpTables.ExpressionExecutor executor = new ExpressionExecutor(
                "\"Hello\" + \" \" + \"world!\" + 1",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual("Hello world!1", executor.Execute().ToCellStringValue().Value);

           executor = new ExpressionExecutor(
                "+\"Hello\"",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual(5, executor.Execute().ToCellNumberValue().Value);
        }

        [TestMethod]
        public void Functions()
        {
            SharpTables.ExpressionExecutor executor = new ExpressionExecutor(
                "min(-1, -2, -200, 10)",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual(-200, executor.Execute().ToCellNumberValue().Value);

           executor = new ExpressionExecutor(
                "inc(-10)",
                new Dictionary<string, string>(),
                new Dictionary<string, ICellValue>(),
                0
                );

            Assert.AreEqual(-9, executor.Execute().ToCellNumberValue().Value);
        }


    }
}

using CmdWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace CmdWrapperTest
{
    [TestClass]
    public class CommandExecutorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            const string commandAsString = "sc start dummy";
            ICommandExecutor executor = new CommandExecutor();

            // Act
            executor.ExecuteCommand(commandAsString);

            // Assert
            var output = executor.GetExecutionResult();
            Check.That(output).IsNotNull();
        }
    }
}

using CmdWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace CmdWrapperTest
{
    [TestClass]
    public class CommandExecutorTests
    {
        [TestMethod]
        public void ShouldExecuteCommand()
        {
            // Arrange
            const string command = "echo hello";
            ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEqualTo("hello");
        }
    }
}

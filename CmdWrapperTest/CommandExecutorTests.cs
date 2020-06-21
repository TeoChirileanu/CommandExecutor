using System.Threading.Tasks;
using CmdWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace CmdWrapperTest
{
    [TestClass]
    public class CommandExecutorTests
    {
        [TestMethod]
        public async Task ShouldExecuteCommand()
        {
            // Arrange
            const string command = "echo hello";
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEqualTo("hello");
        }
    }
}

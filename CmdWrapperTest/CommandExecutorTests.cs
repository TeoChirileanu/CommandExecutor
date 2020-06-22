using System;
using System.IO;
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
        public async Task ShouldExecuteStandaloneCommand()
        {
            // Arrange
            const string greeting = "hello";
            const string command = "echo " + greeting;
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEqualTo(greeting);
        }
        
        [TestMethod]
        public async Task ShouldExecutePollingCommand()
        {
            // Arrange
            const string command = "ping localhost";
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).Contains("Pinging", "Reply");
        }

        [TestMethod]
        public async Task ShouldExecuteAdminCommand()
        {
            // Arrange
            const string command = "sc stop dummy"; // warning: install it first
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsNotEmpty();
        }

        [TestMethod]
        public async Task ShouldExecuteSingleCommandInsideFile()
        {
            // Arrange
            const string file = "foo.bat";
            if (File.Exists(file)) File.Delete(file);
            await File.WriteAllTextAsync(file, "hostname");
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(file);

            // Assert
            Check.That(result.Contains(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase)).IsTrue();
            File.Delete(file);
        }
        
        [TestMethod]
        public async Task ShouldExecuteMultipleCommandsInsideFile()
        {
            // Arrange
            const string file = "bar.bat";
            if (File.Exists(file)) File.Delete(file);
            await File.WriteAllLinesAsync(file, new []{"ping localhost", "hostname", "echo hello"});
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(file);

            // Assert
            Check.That(result).Contains("Pinging", "Reply", Environment.MachineName.ToLower(), "hello");
            if (File.Exists(file)) File.Delete(file);
        }
    }
}

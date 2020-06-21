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
        public async Task ShouldEcho()
        {
            // Arrange
            const string command = "echo hello";
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEqualTo("hello");
        }

        [TestMethod]
        public async Task ShouldCreateFile()
        {
            // Arrange
            const string file = "foo";
            const string command = "echo hello > " + file;
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEmpty();
            Check.That(File.Exists(file)).IsTrue();
            
            // Clean
            File.Delete(file);
        }

        [TestMethod]
        public async Task ShouldExecuteCommandInsideFile()
        {
            // Arrange
            const string file = "foo";
            await File.WriteAllTextAsync(file, "hostname");
            using ICommandExecutor executor = new CommandExecutor();

            // Act
            var result = await executor.ExecuteCommand(file);

            // Assert
            Check.That(result.Contains(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase)).IsTrue();
            
            // Clean
            File.Delete(file);
        }
    }
}

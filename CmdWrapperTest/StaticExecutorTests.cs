using System;
using CmdWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace CmdWrapperTest
{
    [TestClass]
    public class StaticExecutorTests
    {
        [TestMethod]
        public void ShouldExecuteShortCommand()
        {
            // Arrange
            const string greeting = "hello";
            const string command = "echo " + greeting;

            // Act
            var result = StaticCommandExecutor.ExecuteCommand(command);

            // Assert
            Check.That(result).IsEqualTo(greeting);
        }
        
        [TestMethod]
        public void ShouldExecuteLongCommand()
        {
            // Arrange
            const string command = "ping localhost";

            // Act
            var result = StaticCommandExecutor.ExecuteCommand(command);

            // Assert
            Console.WriteLine(result);
        }
    }
}
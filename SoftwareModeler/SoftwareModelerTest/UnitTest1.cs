using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Area51.SoftwareModeler.ViewModels;
using Area51.SoftwareModeler.Views;

namespace SoftwareModelerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestStartup()
        {

//            AvalonTestRunner.RunInSTA(
//delegate
//{
    MainWindow window = new MainWindow();
            window.Visibility = Visibility.Visible;
    Assert.AreEqual(300, window.Height);
            //});
            //MainViewModel mvmModel = new MainViewModel();
            //mvmModel.NewClassCommand()
        }
    }
}

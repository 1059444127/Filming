using UIH.Mcsf.App.UnitTesting;
using UIH.Mcsf.Filming.Card;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.MvvmLight;

namespace UIH.Mcsf.Filming.FE.Test_UT
{


    /// <summary>
    ///This is a test class for RelayCommandTest and is intended
    ///to contain all RelayCommandTest Unit Tests
    ///</summary>
    [TestClass]
    public class RelayCommandTest
    {

        /// <summary>
        ///A test for RelayCommand Constructor Parameter.
        ///</summary>
        [TestMethod]
        public void RelayCommandConstructorParameterTest()
        {
            //AssertHelper.ExpectedException<ArgumentNullException>(() => { new RelayCommand(null, null); });
        }

        /// <summary>
        ///A test for CanExecute
        ///</summary>
        [TestMethod]
        public void CanExecuteTest()
        {
            bool executed = false;
            bool canExecute = false;

            var target = new RelayCommand(() => executed = false, () => canExecute);

            if (executed)
            {

            }

            Assert.IsFalse(target.CanExecute(null));
            canExecute = true;
            Assert.IsTrue(target.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(target, () => target.RaiseCanExecuteChanged());

            Assert.IsTrue(target.CanExecute(null));
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod]
        public void ExecuteTest()
        {
            bool executed = false;
            RelayCommand command = new RelayCommand(() => executed = true, null);
            command.Execute(null);

            Assert.IsTrue(executed);
        }

        public void ExecuteWidthParameterTest()
        {
            bool executed = false;
            object parameter = null;
            RelayCommand<object> command = new RelayCommand<object>(
                p =>
                    {
                        executed = true;
                        parameter = p;
                    }
                );

            object obj = new object();
            command.Execute(obj);

            Assert.IsTrue(executed);
            Assert.AreEqual(parameter, obj);
        }

        /// <summary>
        ///A test for RaiseCanExecuteChanged
        ///</summary>
        [TestMethod]
        public void RaiseCanExecuteChangedTest()
        {
            bool executed = false;
            bool canExecute = false;
            var command = new RelayCommand(() => executed = true, () => canExecute);
            command.Execute(null);

            Assert.IsFalse(executed);

            canExecute = true;

            command.RaiseCanExecuteChanged();
            AssertHelper.CanExecuteChangedEvent(command, () => command.RaiseCanExecuteChanged());

            command.Execute(null);

            Assert.IsTrue(executed);
        }
    }
}

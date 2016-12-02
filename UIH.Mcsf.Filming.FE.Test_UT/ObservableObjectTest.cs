using UIH.Mcsf.Filming.Card.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    /// <summary>
    ///This is a test class for ObservableObjectTest and is intended
    ///to contain all ObservableObjectTest Unit Tests
    ///</summary>
    [TestClass]
    public class ObservableObjectTest
    {

        private class MockObservableObject : ObservableObject
        {
            #region TestProperty1

            /// <summary>
            /// The <see cref="TestProperty1" /> property's name.
            /// </summary>
            public const string TestProperty1PropertyName = "TestProperty1";

            private bool _myProperty;

            /// <summary>
            /// Sets and gets the TestProperty1 property.
            /// Changes to that property's value raise the PropertyChanged event. 
            /// </summary>
            public bool TestProperty1
            {
                get
                {
                    return _myProperty;
                }

                set
                {
                    if (_myProperty == value)
                    {
                        return;
                    }

                    _myProperty = value;
                    RaisePropertyChanged(TestProperty1PropertyName);
                }
            }

            #endregion // TestProperty1
        }

        private class StubObservalbeObject : ObservableObject
        {
            #region Property1

            /// <summary>
            /// wrongly spell on purpose.
            /// </summary>
            public const string Property1PropertyName = "Property2";

            private bool _myProperty;

            /// <summary>
            /// Sets and gets the Property1 property.
            /// Changes to that property's value raise the PropertyChanged event. 
            /// </summary>
            public bool Property1
            {
                get
                {
                    return _myProperty;
                }

                set
                {
                    if (_myProperty == value)
                    {
                        return;
                    }

                    _myProperty = value;
                    RaisePropertyChanged(Property1PropertyName);
                }
            }

            #endregion // Property1
        }

        /// <summary>
        ///A test for ObservableOjbect Constructor
        ///</summary>
        [TestMethod]
        public void ObservableOjbectPropertyChangedTest()
        {
            var target = new MockObservableObject();
            bool propertyChanged = false;
            target.PropertyChanged += (o1, e1) =>
            {
                if (e1.PropertyName == MockObservableObject.TestProperty1PropertyName)
                {
                    propertyChanged = true;
                }
            };
            target.TestProperty1 = !target.TestProperty1;

            Assert.AreEqual(propertyChanged, true);
        }

        [TestMethod]
        public void ObservableOjbectPropertyChangedFailedTest()
        {
            var target = new StubObservalbeObject();

            bool propertyChanged = false;
            target.PropertyChanged += (o1, e1) =>
            {
                if (e1.PropertyName == MockObservableObject.TestProperty1PropertyName)
                {
                    propertyChanged = true;
                }
            };
            target.Property1 = !target.Property1;

            Assert.AreEqual(propertyChanged, false);
        }
    }
}

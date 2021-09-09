using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostsListener.Tests
{
    [TestClass]
    public class CrudTestBase<T>
    {
        private readonly Func<T> _factory;
        private readonly Func<IEnumerable<T>> _get;
        private readonly Action<T> _add;
        private readonly Action<T> _remove;

        public CrudTestBase(
            Func<T> factory,
            Func<IEnumerable<T>> get,
            Action<T> add,
            Action<T> remove)
        {
            _factory = factory;
            _get = get;
            _add = add;
            _remove = remove;
        }
        
        [TestMethod]
        public void TestAddSingle()
        {
            Clear();
            
            Assert.IsFalse(_get().Any());

            _add(_factory());

            Assert.AreEqual(1, _get().Count());
        }
        
        [TestMethod]
        public void TestAddRemoveSingle()
        {
            Clear();
            
            Assert.IsFalse(_get().Any());

            _add(_factory());

            Assert.AreEqual(1, _get().Count());

            _remove(_factory());
            
            Assert.IsFalse(_get().Any());
        }

        public void Clear()
        {
            foreach (T t in _get())
            {
                _remove(t);
            }
        }
    }
}
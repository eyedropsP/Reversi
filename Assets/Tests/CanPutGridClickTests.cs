using System.Collections;
using System.Collections.Generic;
using NormalReversi.Models.Interface;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CanPutGridClickTests
    {
        
        private IGridManager gridManager;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Debug.Log("TestStart");
        }
        
        // A Test behaves as an ordinary method
        [Test]
        public void CanPutGridClickTestsSimplePasses()
        {
            
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CanPutGridClickTestsWithEnumeratorPasses()
        {
            
            yield return null;
        }
    }
}

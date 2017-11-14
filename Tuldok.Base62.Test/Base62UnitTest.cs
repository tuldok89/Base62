using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tuldok.Base62;

namespace Tuldok.Base62.Test
{
    [TestClass]
    public class Base62UnitTest
    {
        RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        const int ByteLength = 20;
        const int MaxStringLength = 27;

        [TestMethod]
        public void TestEquality()
        {
            var bytes = new byte[ByteLength];
            random.GetBytes(bytes);

            var s = Base62Converter.Encode(bytes);
            var decode = Base62Converter.Decode(s.ToCharArray());
            var encode = Base62Converter.Encode(decode);

            Assert.AreEqual(s, encode);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    class Calculator
    {
        public int FirstNum { get; set; }
        public int SecondNum { get; set; }

        public int Add()
        {
            return FirstNum + SecondNum;
        }
    }
}

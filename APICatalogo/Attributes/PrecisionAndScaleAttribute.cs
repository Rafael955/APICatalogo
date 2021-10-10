using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Attributes
{
    public class PrecisionAndScaleAttribute : RegularExpressionAttribute
    {
        public PrecisionAndScaleAttribute(int precision, int scale) : base($@"^(0|-?\d{{0,{precision - scale}}}(\.\d{{0,{scale}}})?)$")
        {

        }
    }
}

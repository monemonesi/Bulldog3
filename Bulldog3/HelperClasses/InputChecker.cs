using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    public class InputChecker
    {
        private GH_Component ghc;

        public InputChecker(GH_Component _ghc)
        {
            ghc = _ghc;
        }

        public void DisplayIfConversionFailed(bool succesfullConversion)
        {
            if (!succesfullConversion)
            {
                ShowInputsError();
                return;
            }
        }

        private void ShowInputsError()
        {
            ghc.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
        }
    }
}

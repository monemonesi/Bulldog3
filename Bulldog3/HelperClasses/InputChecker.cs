using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// Check if the input of a given components can be elaborated and converted correctly
    /// </summary>
    public class InputChecker
    {
        private GH_Component ghc;

        public InputChecker(GH_Component _ghc)
        {
            ghc = _ghc;
        }

        /// <summary>
        /// Check if the convertion in succesful and call ShowInputsError if not.
        /// </summary>
        /// <param name="succesfullConversion"></param>
        public void StopIfConversionIsFailed(bool succesfullConversion)
        {
            if (!succesfullConversion)
            {
                ShowInputsError();
                return;
            }
        }

        /// <summary>
        /// Display the error message if the conversion failed
        /// </summary>
        private void ShowInputsError()
        {
            ghc.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
        }
    }
}

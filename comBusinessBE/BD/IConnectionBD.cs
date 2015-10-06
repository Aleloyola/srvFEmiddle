using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace comBusinessBE.BD
{
    public interface IConnectionBD
    {
        string select();
        bool existDoc(string sSIIDOC, string sSIINUM);
        bool updateOK(string sSIIDOC, string sSIINUM);
        bool updateError(string sSIIDOC, string sSIINUM, string sObservation);
    }
}

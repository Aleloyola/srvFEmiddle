using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace comBusinessBE.BI
{
    public interface ILog
    {
        void LogException(Object oEx);
        void LogError(Object oErr);
        void LogInfo(Object oInfo);
    }
}

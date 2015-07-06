using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Imtcspeechservice
{
    public interface speechinterface
    {
        void Add_Grammer(ref int result);

        Microsoft.Maps.MapControl.WPF.Location getlocation(string input, ref string results);

        void CalculateRouteLocation(string from, string to);
        
        void speechlibrray_windowclosing();

        string getdirection(string start, string end, ref string distance);

       
    }
}

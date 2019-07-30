// edtFTPnet
// 
// Copyright (C) 2012 Enterprise Distributed Technologies Ltd
// 
// www.enterprisedt.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// 
// Bug fixes, suggestions and comments should posted on 
// http://www.enterprisedt.com/forums/index.php
// 
// Change Log:
// 
// $Log: LogTag.cs,v $
// Revision 1.1  2012/11/14 04:35:43  hans
// Log tagging.
//

using System.Collections.Generic;

namespace EnterpriseDT.Util.Debug
{
    public interface ILogTag
    {
        string GetLogTag();
    }


    public class LogTag : ILogTag
    {
        private static object instanceCounterLock = new object();
        private static Dictionary<string, int> instanceCounters = new Dictionary<string, int>();
        private int instanceNumber;
        private string prefix;

        public LogTag(string prefix)
        {
            lock (instanceCounterLock)
            {
                if (instanceCounters.TryGetValue(prefix, out instanceNumber))
                    instanceNumber++;
                else
                    instanceNumber = 1;
                instanceCounters[prefix] = instanceNumber;
            }
            this.prefix = prefix;
        }

        public int Id
        {
            get { return instanceNumber; }
        }

        public virtual string Name
        {
            get
            {
                return prefix + "." + instanceNumber;
            }
        }

        public string GetLogTag()
        {
            return Name;
        }
    }
}

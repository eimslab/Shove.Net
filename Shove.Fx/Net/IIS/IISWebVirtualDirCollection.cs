using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shove._Net.IIS
{
    /// <summary>
    /// IISWebVirtualDirCollection
    /// </summary>
    public class IISWebVirtualDirCollection : CollectionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IISWebServer Parent = null;

        /// <summary>
        /// 
        /// </summary>
        public IISWebVirtualDir this[int Index]
        {
            get
            {
                return (IISWebVirtualDir)this.List[Index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IISWebVirtualDir this[string Name]
        {
            get
            {
                Name = Name.Trim();

                for (int i = 0; i < this.List.Count; i++)
                {
                    IISWebVirtualDir vd = (IISWebVirtualDir)this.List[i];

                    if (String.Compare(vd.Name.Trim(), Name, true) == 0)
                    {
                        return vd;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Parent"></param>
        public IISWebVirtualDirCollection(IISWebServer Parent)
        {
            this.Parent = Parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iisvd"></param>
        public void Add(IISWebVirtualDir iisvd)
        {
            this.List.Add(iisvd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iisvds"></param>
        public void AddRange(IISWebVirtualDir[] iisvds)
        {
            for (int i = 0; i <= iisvds.GetUpperBound(0); i++)
            {
                Add(iisvds[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iisvd"></param>
        public void Remove(IISWebVirtualDir iisvd)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if ((IISWebVirtualDir)this.List[i] == iisvd)
                {
                    this.List.RemoveAt(i);

                    break;
                }
            }
        }
    }
}

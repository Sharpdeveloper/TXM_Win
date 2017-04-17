using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    public interface IUpgrade
    {
         int Count { get; set; }
         string Name { get; set; }
         string Gername { get; set; }
    }
}

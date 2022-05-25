using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyUnity
{
    public interface ISortable
    {
        int Order { get; set; }
    }
}

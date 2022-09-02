using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalRbkm.Plugins
{
    public interface IPlugin
    {
        //string Name { get; }
        //string Title { get; }
        //Version Version { get; }
        //String Description { get; }
        void Execute(Object mainContainer, PluginInitializationData listData);
        void Dispose();
    }
}
